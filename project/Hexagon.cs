// Copyright 2021 Bob "Wombat" Hogg
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.



using System;
using System.Collections.Generic;

using Godot;

using static Godot.Colors;
using static Godot.JoystickList;
using static Godot.Mathf;

/**
 * Hexagon node class
 */
public class Hexagon : Node2D
{
    /**
     * The base color of this hexagon
     */
    public Color BaseColor { get; set; }

    /**
     * List of all edge colors for this hexagon
     */
    public List<Color> EdgeColors { get; set; }

    /**
     * If true, this hexagon is currently selected for rotation
     */
    public bool[] Selected = { false, false };

    /**
     * If true, this hexagon is set to be replaced
     */
    public bool MarkedForReplacement { get; set; }

    /**
     * If true, this hexagon is currently involved in a match
     */
    public bool Matched { get; set; }

    private int i;

    private int j;

    private DynamicFont RocketFont;

    private Timer HexRefreshTimer;

    private Timer ColorFlashTimer;

    /**
     * List of all possible options for edge colors
     */
    public static Color[] EdgeColorOptions { get; } = { Green, Purple, HotPink, Yellow };

    private static float SideLength = 50.0F;

    private static float Cos30 = (float)Cos(Deg2Rad(30));

    private static int EdgeThickness = 6; // just a coincidence that 6 was a nice edge size

    private static Random Rand = new Random();

    private static Color DefaultHexColor = Black;

    private static int DefaultRefreshTimeSeconds = 5;

    private static Color[] CursorColors = { Red, Green };

    /**
     * Wrapper class for a grid of hexagons
     */
    public class Grid : Node2D
    {
        /**
         * Signal issued when any hexagon in the grid is rotated
         */
        [Signal]
        public delegate void HexagonRotated(Hexagon rotatedHexagon, Godot.Collections.Array matchedHexagons, Godot.Collections.Dictionary<Color, int> matchedColors);

        /**
         * The underlying 2D array of hexagons
         */
        public Hexagon[,] Array { get; set; }

        private int NumRows;

        private int NumCols;

        private Hexagon[] SelectedHexagons = new Hexagon[Global.Is2Player ? 2 : 1];

        /**
         * Starts off a grid at the specified start point.
         * @param startPoint Top-left point of the grid
         * @param numRows Number of rows
         * @param numCols Number of columns
         */
        public Grid(Vector2 startPoint, int numRows, int numCols)
        {
            Position = startPoint;
            NumRows = numRows;
            NumCols = numCols;
        }

        /**
         * Handle input for mouse clicks and joystick buttons
         * @param inputEvent Input event
         */
        public override void _UnhandledInput(InputEvent inputEvent)
        {
            Hexagon affectedHexagon = null;
            Direction direction = Direction.LEFT;

            if(@inputEvent is InputEventJoypadButton eventControllerButton)
            {
                GetTree().SetInputAsHandled();
                // ensure we don't double up the actions
                if(@inputEvent.IsPressed())
                {
                    return;
                }

                int controllerIndex = eventControllerButton.Device;

                switch((JoystickList)eventControllerButton.ButtonIndex)
                {
                    case DpadUp:
                    case DpadDown:
                    case DpadLeft:
                    case DpadRight:
                        int[][] dirsToGo ={
                            new int[] { -1, 0 },
                            new int[] { 1, 0 },
                            new int[] { 0, -1 },
                            new int[] { 0, 1 },
                        };
                        int[] dir = dirsToGo[eventControllerButton.ButtonIndex - (int)DpadUp];
                        Hexagon currentlySelectedHexagon = SelectedHexagons[controllerIndex];
                        SelectedHexagons[controllerIndex].Selected[controllerIndex] = false;
                        int newI = SelectedHexagons[controllerIndex].i + dir[0];
                        if(newI < 0)
                        {
                            newI = 0;
                        }
                        else if(newI >= NumRows)
                        {
                            newI = NumRows - 1;
                        }
                        int newJ = SelectedHexagons[controllerIndex].j + dir[1];
                        if(newJ < 0)
                        {
                            newJ = 0;
                        }
                        else if(newJ >= NumCols)
                        {
                            newJ = NumCols - 1;
                        }
                        Hexagon newlySelectedHexagon = Array[newI, newJ];
                        newlySelectedHexagon.Selected[controllerIndex] = true;
                        SelectedHexagons[controllerIndex] = newlySelectedHexagon;
                        break;
                    case L:
                    case L2:
                        // Note: deliberately not supporting control stick / C stick triggers (L3 and R3 in Godot).
                        // They are way too easy to use intentionally.
                        HandleRotation(SelectedHexagons[controllerIndex], Direction.LEFT, controllerIndex);
                        break;
                    case R:
                    case R2:
                        HandleRotation(SelectedHexagons[controllerIndex], Direction.RIGHT, controllerIndex);
                        break;
                }
                return;
            }
            else if(@inputEvent is InputEventMouseButton eventMouseButton)
            {
                // ensure we don't double-rotate from a single click
                if(@inputEvent.IsPressed())
                {
                    return;
                }

                Vector2 clickPos = eventMouseButton.Position - Position; // offset because we were one diagonal hexagon down
                affectedHexagon = GetAffectedHexagon(clickPos);
                if(affectedHexagon == null)
                {
                    return;
                }

                GetTree().SetInputAsHandled();
                if(OS.HasTouchscreenUiHint())
                {
                    // on touch screen devices, a tap should be equivalent to a select, not a rotation
                    SelectedHexagons[0].Selected[0] = false;
                    SetSelectedHexagon(affectedHexagon.i, affectedHexagon.j, 0);
                    return;
                }
                else if((int)ButtonList.Right == eventMouseButton.ButtonIndex)
                {
                    direction = Direction.RIGHT;
                }
                HandleRotation(affectedHexagon, direction, 0);
            }
        }

        /**
         * Select the hexagon at the specified position
         * @param i Row to select
         * @param j Column to select
         * @param playerIndex Currently acting player
         */
        public void SetSelectedHexagon(int i, int j, int playerIndex)
        {
            Array[i, j].Selected[playerIndex] = true;
            SelectedHexagons[playerIndex] = Array[i, j];
        }

        /**
         * Select a number of hexagons in the grid for later replacement
         * @param numHexagonsToReplace The number of hexagons to select
         */
        public void SelectHexagonsForReplacement(int numHexagonsToReplace)
        {
            int n = 0;
            while(n < numHexagonsToReplace)
            {
                int randomRow = Rand.Next(0, NumRows);
                int randomCol = Rand.Next(0, NumCols);
                if(Array[randomRow, randomCol].MarkedForReplacement)
                {
                    continue;
                }
                Array[randomRow, randomCol].StartRefreshTimer();
                n++;
            }
        }

        /**
         * Force the currently selected hexagon to rotate
         * @param direction Which direction to rotate
         * @param playerIndex Currently active player
         */
        public void RotateSelected(Direction direction, int playerIndex)
        {
            HandleRotation(SelectedHexagons[playerIndex], direction, playerIndex);
        }

        private void HandleRotation(Hexagon affectedHexagon, Direction direction, int playerIndex)
        {
            // FIXME this is not handled right!
            affectedHexagon.Rot(direction);
            Color colorToFlash = DefaultHexColor;
            Godot.Collections.Dictionary<Color, int> matchedColors = null;
            HashSet<Hexagon> matchedHexagons = CheckMatches(affectedHexagon, out matchedColors);
            if(matchedColors.Keys.Count > 0)
            {
                foreach(var pair in matchedColors)
                {
                    colorToFlash = pair.Key;
                }
            }
            Hexagon[] hexagons = new Hexagon[matchedHexagons.Count];
            int i = 0;
            foreach(Hexagon hexagon in matchedHexagons)
            {
                if(OS.IsDebugBuild())
                {
                    GD.Print("Matched hex " + hexagon.i + ", " + hexagon.j);
                }
                hexagon.Matched = true;
                hexagon.AbortReplacement();
                hexagon.FlashColor(colorToFlash);
                hexagon.Refresh();
                hexagons[i++] = hexagon;
            }
            EmitSignal(nameof(HexagonRotated), affectedHexagon, hexagons, matchedColors);
        }

        private HashSet<Hexagon> CheckMatches(Hexagon affectedHexagon, out Godot.Collections.Dictionary<Color, int> matchedColors)
        {
            HashSet<Hexagon> matchedHexagons = new HashSet<Hexagon>();
            matchedColors = new Godot.Collections.Dictionary<Color, int>();
            int tre = 0;
            int bre = 1;
            int ble = 3;
            int tle = 4;
            int row = (int)affectedHexagon.i;
            int column = (int)affectedHexagon.j;

            // top-left
            if(row != 0 && column != 0)
            {
                Hexagon hexNW = Array[row - 1, column - 1];
                Hexagon hexN = Array[row - 1, column];
                Hexagon hexW = Array[row, column - 1];
                bool nwEq = affectedHexagon.EdgeColors[tle] == hexNW.EdgeColors[bre];
                bool nEq = affectedHexagon.EdgeColors[tle] == hexN.EdgeColors[ble];
                bool wEq = affectedHexagon.EdgeColors[tle] == hexW.EdgeColors[tre];
                if(nwEq && nEq && wEq)
                {
                    matchedHexagons.Add(affectedHexagon);
                    matchedHexagons.Add(hexNW);
                    matchedHexagons.Add(hexN);
                    matchedHexagons.Add(hexW);
                    // No need to += 1 here because this is definitely the first instance
                    matchedColors[affectedHexagon.EdgeColors[tle]] = 1;
                }
            }
            // top-right
            if(row != 0 && column != NumCols - 1)
            {
                Hexagon hexNE = Array[row - 1, column + 1];
                Hexagon hexN = Array[row - 1, column];
                Hexagon hexE = Array[row, column + 1];
                bool neEq = affectedHexagon.EdgeColors[tre] == hexNE.EdgeColors[ble];
                bool nEq = affectedHexagon.EdgeColors[tre] == hexN.EdgeColors[bre];
                bool eEq = affectedHexagon.EdgeColors[tre] == hexE.EdgeColors[tle];
                if(neEq && nEq && eEq)
                {
                    matchedHexagons.Add(affectedHexagon);
                    matchedHexagons.Add(hexNE);
                    matchedHexagons.Add(hexN);
                    matchedHexagons.Add(hexE);
                    if(matchedColors.ContainsKey(affectedHexagon.EdgeColors[tre]))
                    {
                        matchedColors[affectedHexagon.EdgeColors[tre]] += 1;
                    }
                    else
                    {
                        matchedColors.Add(affectedHexagon.EdgeColors[tre], 1);
                    }
                }
            }

            // bottom-right
            if(row != NumRows - 1 && column != NumCols - 1)
            {
                Hexagon hexSE = Array[row + 1, column + 1];
                Hexagon hexS = Array[row + 1, column];
                Hexagon hexE = Array[row, column + 1];
                bool seEq = affectedHexagon.EdgeColors[bre] == hexSE.EdgeColors[tle];
                bool sEq = affectedHexagon.EdgeColors[bre] == hexS.EdgeColors[tre];
                bool eEq = affectedHexagon.EdgeColors[bre] == hexE.EdgeColors[ble];
                if(seEq && sEq && eEq)
                {
                    matchedHexagons.Add(affectedHexagon);
                    matchedHexagons.Add(hexSE);
                    matchedHexagons.Add(hexS);
                    matchedHexagons.Add(hexE);
                    if(matchedColors.ContainsKey(affectedHexagon.EdgeColors[bre]))
                    {
                        matchedColors[affectedHexagon.EdgeColors[bre]] += 1;
                    }
                    else
                    {
                        matchedColors.Add(affectedHexagon.EdgeColors[bre], 1);
                    }
                }
            }

            // bottom-left
            if(row != NumRows - 1 && column != 0)
            {
                Hexagon hexSW = Array[row + 1, column - 1];
                Hexagon hexS = Array[row + 1, column];
                Hexagon hexW = Array[row, column - 1];
                bool swEq = affectedHexagon.EdgeColors[ble] == hexSW.EdgeColors[tre];
                bool sEq = affectedHexagon.EdgeColors[ble] == hexS.EdgeColors[tle];
                bool wEq = affectedHexagon.EdgeColors[ble] == hexW.EdgeColors[bre];
                if(swEq && sEq && wEq)
                {
                    matchedHexagons.Add(affectedHexagon);
                    matchedHexagons.Add(hexSW);
                    matchedHexagons.Add(hexS);
                    matchedHexagons.Add(hexW);
                    if(matchedColors.ContainsKey(affectedHexagon.EdgeColors[ble]))
                    {
                        matchedColors[affectedHexagon.EdgeColors[ble]] += 1;
                    }
                    else
                    {
                        matchedColors.Add(affectedHexagon.EdgeColors[ble], 1);
                    }
                }
            }

            return matchedHexagons;
        }

        private Hexagon GetAffectedHexagon(Vector2 clickPos)
        {
            int j = RoundToInt(clickPos.x / (2 * BigRadius() + 0.2F * EdgeThickness));
            int i = RoundToInt(clickPos.y / (2 * SmallRadius() + 0.8F * EdgeThickness));
            if(j >= NumCols || i >= NumRows)
            {
                if(OS.IsDebugBuild())
                {
                    GD.Print("Bad Click! i, j: " + i + ", " + j);
                    GD.Print("Click Position is " + clickPos);
                }
                return null;
            }

            return Array[i, j];
        }
    }

    public static Color RandomColor()
    {
        return EdgeColorOptions[Rand.Next(0, EdgeColorOptions.Length)];
    }

    /**
     * Do not use this constructor.
     * It only exists to allow for Mono to create temporary objects.
     */
    public Hexagon()
    {
        MarkedForReplacement = false;
        Selected = new bool[] { false, false };
        Matched = false;
    }

    /**
     * Sets up this hexagon
     * @param center Center position
     * @param baseColor Base color
     * @param edgeColors Edge colors
     */
    public Hexagon(Vector2 center, Color baseColor, List<Color> edgeColors)
    {
        Position = center;
        BaseColor = baseColor;
        EdgeColors = edgeColors;
        MarkedForReplacement = false;
        Selected = new bool[] { false, false };
        Matched = false;
    }

    /**
     * Loads the font for this hexagon.
     * Called when the hexagon enters the scene tree.
     */
    public override void _Ready()
    {
        base._Ready();
        RocketFont = Utils.GetRocketFont();
    }

    /**
     * Redraw every frame.
     * Called once per frame.
     * @param delta Elapsed time since last frame in seconds
     */
    public override void _Process(float delta)
    {
        Update();
    }

    /**
     * Draws this hexagon with direct geometry commands
     */
    public override void _Draw()
    {
        // remember to use relative coordinates for all calls here
        Vector2[] hexagonPoints = GetPoints();
        DrawColoredPolygon(hexagonPoints, GetHexColor());
        for(int i = 0; i < hexagonPoints.Length - 1; i++)
        {
            DrawLine(hexagonPoints[i], hexagonPoints[i + 1], EdgeColors[i], EdgeThickness);
        }
        DrawLine(hexagonPoints[hexagonPoints.Length - 1], hexagonPoints[0], EdgeColors[EdgeColors.Count - 1], EdgeThickness);
        if(Selected[0] && ShouldShowSelections())
        {
            DrawCircle(new Vector2(0, 0), 5, CursorColors[0]);
        }
        else if(Selected[1] && ShouldShowSelections())
        {
            DrawCircle(new Vector2(0, 0), 5, CursorColors[1]);
        }
        if(OS.IsDebugBuild())
        {
            DrawString(Utils.AnyFont, new Vector2(-25, 30), String.Format("({0},{1})", (int)Position.x, (int)Position.y));
        }
        if(HexRefreshTimer != null && HexRefreshTimer.TimeLeft > 0)
        {
            DrawString(RocketFont, new Vector2(-10, 0), String.Format("{0}", (int)HexRefreshTimer.TimeLeft + 1), Red);
        }
    }

    private Vector2[] GetPoints()
    {
        float centerX = 0;
        float centerY = 0;
        Vector2 p0 = new Vector2(0.5F * SideLength + centerX, centerY - Cos30 * SideLength);
        Vector2 p1 = new Vector2(SideLength + centerX, centerY);
        Vector2 p2 = new Vector2(0.5F * SideLength + centerX, Cos30 * SideLength + centerY);
        Vector2 p3 = new Vector2(centerX - 0.5F * SideLength, Cos30 * SideLength + centerY);
        Vector2 p4 = new Vector2(centerX - SideLength, centerY);
        Vector2 p5 = new Vector2(centerX - 0.5F * SideLength, centerY - Cos30 * SideLength);
        return new Vector2[] { p0, p1, p2, p3, p4, p5 };
    }

    /**
     * Rotate this hexagon in the specified direction
     * @param direction Direction to rotate
     */
    public void Rot(Direction direction)
    {
        if(direction == Direction.LEFT)
        {
            Color firstEdgeColor = EdgeColors[0];
            EdgeColors.RemoveAt(0);
            EdgeColors.Add(firstEdgeColor);
        }
        else
        {
            Color lastEdgeColor = EdgeColors[EdgeColors.Count - 1];
            EdgeColors.RemoveAt(EdgeColors.Count - 1);
            EdgeColors.Insert(0, lastEdgeColor);
        }
    }

    /**
     * Prevent this hexagon from being replaced
     */
    public void AbortReplacement()
    {
        MarkedForReplacement = false;
        if(HexRefreshTimer != null)
        {
            RemoveChild(HexRefreshTimer);
            HexRefreshTimer.QueueFree();
        }
        HexRefreshTimer = null;
    }

    /**
     * Start up a new refresh timer which will replace this hexagon after timeout
     */
    public void StartRefreshTimer()
    {
        if(HexRefreshTimer == null)
        {
            HexRefreshTimer = new Timer();
            AddChild(HexRefreshTimer, true);
            HexRefreshTimer.Connect("timeout", this, nameof(On_HexRefresh_Timer_Timeout));
        }
        if(OS.IsDebugBuild())
        {
            GD.Print("Marked " + i + ", " + j + " for replacement");
        }
        MarkedForReplacement = true;
        HexRefreshTimer.Start(DefaultRefreshTimeSeconds);
    }

    /**
     * Temporarily change the base color of this hexagon
     * @param colorToFlash The color to temporarily display
     */
    public void FlashColor(Color colorToFlash)
    {
        BaseColor = colorToFlash;
        if(ColorFlashTimer == null)
        {
            ColorFlashTimer = new Timer();
            AddChild(ColorFlashTimer, true);
            ColorFlashTimer.Connect("timeout", this, nameof(On_ColorFlash_Timer_Timeout));
            ColorFlashTimer.Start(1);
        }
    }

    private void On_HexRefresh_Timer_Timeout()
    {
        AbortReplacement();
        if(OS.IsDebugBuild())
        {
            GD.Print("Hex " + i + "," + j + " refreshed");
        }
        Refresh();
    }

    private void On_ColorFlash_Timer_Timeout()
    {
        if(ColorFlashTimer != null)
        {
            RemoveChild(ColorFlashTimer);
            ColorFlashTimer.QueueFree();
            ColorFlashTimer = null;
        }
        BaseColor = DefaultHexColor;
    }

    private void Refresh()
    {
        EdgeColors = RandomEdgeColors();
    }

    private Color GetHexColor()
    {
        if(MarkedForReplacement)
        {
            return DeepSkyBlue;
        }
        return BaseColor;
    }

    private bool ShouldShowSelections()
    {

        bool isControllerMode = Utils.IsControllerMode();
        bool hasTouch = OS.HasTouchscreenUiHint();
        return isControllerMode || hasTouch;
    }

    /**
     * Return the big ("out") radius of a hexagon
     */
    public static float BigRadius()
    {
        return SideLength;
    }

    /**
     * Return the small ("in") radius of a hexagon
     */
    public static float SmallRadius()
    {
        return Cos30 * BigRadius();
    }

    /**
     * Create a new hexagon with randomly chosen edge colors
     * @param center Center position
     * @param baseColor Base color
     */
    public static Hexagon RandomHexagon(Vector2 center, Color baseColor)
    {
        List<Color> edgeColors = RandomEdgeColors();
        return new Hexagon(center, baseColor, edgeColors);
    }

    /**
     * Create a new grid of hexagons, each of which has random edge colors
     * @param numRows Number of rows
     * @param numCols Number of columns
     * @param basePosition Position of the top-left hexagon in the grid
     * @param baseColor Base color of every hexagon
     */
    public static Grid RandomHexagonGrid(int numRows, int numCols, Vector2 basePosition, Color baseColor)
    {
        // FIXME: most of this stuff should move into the grid class
        Grid grid = new Grid(basePosition, numRows, numCols);
        Hexagon[,] array = new Hexagon[numRows, numCols];
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCols; j++)
            {
                float xPos = (float)(j * BigRadius() * 2);
                float yPos = (float)(i * SmallRadius() * 2);

                // Bit of a buffer so the edge colors don't overlap
                // (the hardcoded constants are just eyeballed to look reasonable)
                // but that said, they were used in deriving the math for GetAffectedHexagon(),
                // so don't change them without thinking
                xPos += EdgeThickness * j * 0.2F;
                yPos += EdgeThickness * i * 0.8F;

                Vector2 position = new Vector2(xPos, yPos);
                array[i, j] = RandomHexagon(position, baseColor);
                array[i, j].i = i;
                array[i, j].j = j;
                grid.AddChild(array[i, j], true);
            }
        }

        grid.Array = array;
        return grid;
    }

    private static List<Color> RandomEdgeColors()
    {
        List<Color> edgeColors = new List<Color>();
        for(int i = 0; i < 6; i++)
        {
            edgeColors.Add(RandomColor());
        }
        return edgeColors;
    }
}
