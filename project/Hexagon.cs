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
using System.Globalization;

using Godot;

using static Godot.Colors;
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

    public int i { get; set; }

    public int j { get; set; }

    private DynamicFont RocketFont;

    private Timer HexRefreshTimer;

    private Timer ColorFlashTimer;

    /**
     * List of all possible options for edge colors
     */
    private static Color[] EdgeColorOptions = { Green, Purple, HotPink, Yellow };

    private static float SideLength = 50.0F;

    private static float Cos30 = (float)Cos(Deg2Rad(30));

    public static int EdgeThickness = ConfigFileUtils.GetEdgeThickness();

    private static Random Rand = new Random();

    public static Color DefaultHexColor = Black;

    private static int DefaultRefreshTimeSeconds = 5;

    private static CultureInfo culture = ConfigFileUtils.GetCulture();
    private static Color[] CursorColors = { Red, Green };

    /**

                int controllerIndex = eventControllerButton.Device;

         * @param playerIndex Currently acting player
         * @param playerIndex Currently active player
            // FIXME this is not handled right!
     * Get a random edge color
     * @return Color A randomly chosen edge color
     */
    public static Color RandomColor()
    {
#pragma warning disable CA5394
        return EdgeColorOptions[Rand.Next(0, EdgeColorOptions.Length)];
#pragma warning restore CA5394
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
            DrawString(Utils.AnyFont, new Vector2(-25, 30), String.Format(culture, "({0},{1})", (int)Position.x, (int)Position.y));
        }
        if(HexRefreshTimer != null && HexRefreshTimer.TimeLeft > 0)
        {
            DrawString(RocketFont, new Vector2(-10, 0), String.Format(culture, "{0}", (int)HexRefreshTimer.TimeLeft + 1), Red);
        }
    }

    private static Vector2[] GetPoints()
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
            GD.Print("Marked " + i.ToString(culture) + ", " + j.ToString(culture) + " for replacement");
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
            GD.Print("Hex " + i.ToString(culture) + "," + j.ToString(culture) + " refreshed");
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

    public void Refresh()
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

    private static bool ShouldShowSelections()
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
