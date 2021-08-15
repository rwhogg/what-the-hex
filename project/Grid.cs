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

using static Godot.JoystickList;
using static Godot.Mathf;

/**
 * Wrapper class for a grid of hexagons
 */
public class Grid : Node2D
{
    // FIXME: the hexagon and the grid classes are so tightly intertwined, and also they
    // probably have game logic embedded in them. It would be better if I could
    // sort out what should be where, and maybe also move some of the logic into signals
    // emitted by the hexagons and grid as appropriate, with the game logic moved to the game
    // component in signal handlers

    /**
     * Signal issued when any hexagon in the grid is rotated
     */
    [Signal]
    public delegate void HexagonRotated(Hexagon rotatedHexagon, Godot.Collections.Array matchedHexagons, Godot.Collections.Dictionary<Color, int> matchedColors);

    /**
     * The underlying jagged array of hexagons
     */
    public Hexagon[][] Array;

    private readonly int[] HexagonsPerRow;

    private Hexagon SelectedHexagon;

    private static readonly Random Rand = new Random();

    private static readonly CultureInfo culture = ConfigFileUtils.GetCulture();

    /**
     * Do not use. Only for constructing Mono temp objects
     */
    public Grid() { }

    /**
     * Starts off a grid at the specified start point.
     * @param startPoint Top-left point of the grid
     * @param hexagonsPerRow Number of hexagons per row
     */
    public Grid(Vector2 startPoint, int[] hexagonsPerRow)
    {
        Position = startPoint;
        HexagonsPerRow = hexagonsPerRow;
    }

    /**
     * Handle input for mouse clicks and joystick buttons
     * @param event Input event
     */
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventJoypadButton eventControllerButton)
        {
            GetTree().SetInputAsHandled();
            HandleButtonPress(eventControllerButton);
        }
        else if(@event is InputEventMouseButton eventMouseButton)
        {
            HandleMouseClick(eventMouseButton);
        }
        else if(@event is InputEventKey eventKey)
        {
            HandleKeyPress(eventKey);
        }
    }

    private void HandleKeyPress(InputEventKey eventKey)
    {
        if(eventKey.Scancode == (int)KeyList.Escape)
        {
            Pause();
        }
    }

    private void HandleButtonPress(InputEventJoypadButton eventControllerButton)
    {
        // ensure we don't double up the actions
        if(eventControllerButton.IsPressed())
        {
            return;
        }
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
                SelectedHexagon.Selected = false;
                int newI = SelectedHexagon.I + dir[0];
                int numRows = HexagonsPerRow.Length;
                if(newI < 0)
                {
                    newI = 0;
                }
                else if(newI >= numRows)
                {
                    newI = numRows - 1;
                }
                int newJ = SelectedHexagon.J + dir[1];
                if(newJ < 0)
                {
                    newJ = 0;
                }
                else if(newJ >= HexagonsPerRow[newI])
                {
                    newJ = HexagonsPerRow[newI] - 1;
                }
                Hexagon newlySelectedHexagon = Array[newI][newJ];
                newlySelectedHexagon.Selected = true;
                SelectedHexagon = newlySelectedHexagon;
                break;
            case L:
            case L2:
                // Note: deliberately not supporting control stick / C stick triggers (L3 and R3 in Godot).
                // They are way too easy to use intentionally.
                HandleRotation(SelectedHexagon, Direction.LEFT);
                break;
            case R:
            case R2:
                HandleRotation(SelectedHexagon, Direction.RIGHT);
                break;
            case Start:
                Pause();
                break;
        }
    }

    private void Pause()
    {
        GetTree().Paused = true;
        OS.Alert("PAUSED", "Pause");
        GetTree().Paused = false;
    }

    private void HandleMouseClick(InputEventMouseButton eventMouseButton)
    {
        // ensure we don't double-rotate from a single click and that we don't accidentally trigger from the mouse wheel
        if(eventMouseButton.IsPressed() || eventMouseButton.ButtonIndex >= (int) ButtonList.Middle)
        {
            return;
        }

        Vector2 clickPos = eventMouseButton.Position - Position; // offset because we were one diagonal hexagon down
        Hexagon affectedHexagon = GetAffectedHexagon(clickPos);
        if(affectedHexagon == null)
        {
            return;
        }

        GetTree().SetInputAsHandled();

        Direction direction = Direction.LEFT;
        if(OS.HasTouchscreenUiHint())
        {
            // on touch screen devices, a tap should be equivalent to a select, not a rotation
            SelectedHexagon.Selected = false;
            SetSelectedHexagon(affectedHexagon.I, affectedHexagon.J);
            return;
        }
        else if((int)ButtonList.Right == eventMouseButton.ButtonIndex)
        {
            direction = Direction.RIGHT;
        }
        HandleRotation(affectedHexagon, direction);
    }

    /**
     * Select the hexagon at the specified position
     * @param i Row to select
     * @param j Column to select
     */
    public void SetSelectedHexagon(int i, int j)
    {
        Array[i][j].Selected = true;
        SelectedHexagon = Array[i][j];
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
#pragma warning disable CA5394
            int randomRow = Rand.Next(0, HexagonsPerRow.Length);
            int randomCol = Rand.Next(0, HexagonsPerRow[randomRow]);
#pragma warning restore CA5394
            if(Array[randomRow][randomCol].MarkedForReplacement)
            {
                continue;
            }
            Array[randomRow][randomCol].StartRefreshTimer();
            n++;
        }
    }

    /**
     * Force the currently selected hexagon to rotate
     * @param direction Which direction to rotate
     */
    public void RotateSelected(Direction direction)
    {
        HandleRotation(SelectedHexagon, direction);
    }

    private void HandleRotation(Hexagon affectedHexagon, Direction direction)
    {
        affectedHexagon.Rot(direction);
        Color colorToFlash = Hexagon.DefaultHexColor;
        HashSet<Hexagon> matchedHexagons = CheckMatches(affectedHexagon, out Godot.Collections.Dictionary<Color, int> matchedColors);
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
                GD.Print("Matched hex " + hexagon.I.ToString(culture) + ", " + hexagon.J.ToString(culture));
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
        int row = (int)affectedHexagon.I;
        int column = (int)affectedHexagon.J;

        // FIXME: 2 issues here
        // 1. This logic might have to be adjusted to handle layouts with unequal columns per row
        // 2. More generally, look into refactoring this (in trunk). Maybe the checking logic can be abstracted?

        // top-left
        if(row != 0 && column != 0)
        {
            Hexagon hexNW = Array[row - 1][column - 1];
            Hexagon hexN = Array[row - 1][column];
            Hexagon hexW = Array[row][column - 1];
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
        if(row != 0 && column != HexagonsPerRow[row] - 1)
        {
            Hexagon hexNE = Array[row - 1][column + 1];
            Hexagon hexN = Array[row - 1][column];
            Hexagon hexE = Array[row][column + 1];
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
        if(row != HexagonsPerRow.Length - 1 && column != HexagonsPerRow[row] - 1)
        {
            Hexagon hexSE = Array[row + 1][column + 1];
            Hexagon hexS = Array[row + 1][column];
            Hexagon hexE = Array[row][column + 1];
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
        if(row != HexagonsPerRow.Length - 1 && column != 0)
        {
            Hexagon hexSW = Array[row + 1][column - 1];
            Hexagon hexS = Array[row + 1][column];
            Hexagon hexW = Array[row][column - 1];
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
        // FIXME: does this need to be adjusted for a different edge thickness???
        int j = RoundToInt(clickPos.x / (2 * Hexagon.BigRadius() + 0.2F * Hexagon.EdgeThickness));
        int i = RoundToInt(clickPos.y / (2 * Hexagon.SmallRadius() + 0.8F * Hexagon.EdgeThickness));
        if(i >= HexagonsPerRow.Length || j >= HexagonsPerRow[i])
        {
            if(OS.IsDebugBuild())
            {
                GD.Print("Bad Click! i, j: " + i.ToString(culture) + ", " + j.ToString(culture));
                GD.Print("Click Position is " + clickPos);
            }
            return null;
        }

        return Array[i][j];
    }
}
