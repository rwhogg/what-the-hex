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

using static Godot.Mathf;

/**
 * Wrapper class for a grid of hexagons
 */
public class BaseGrid : Node2D
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

    // FIXME: this signal should definitely NOT be emitted by the grid, but we're doing most of the non-trivial button handling here for now, so it's gotta go here for now
    [Signal]
    public delegate void PowerUpActivated();

    /**
     * The underlying jagged array of hexagons
     */
    public Hexagon[][] Array;
    // FIXME: Consider changing the field 'Array' to a private or internal field and add a 'SetArray' method.
    // Would have to look into the specifics of how I would want to do that

    protected readonly int[] HexagonsPerRow;

    protected readonly Hexagon[] SelectedHexagons = new Hexagon[1 + Convert.ToInt32(RuntimeConfig.Is2Player)];

    private static readonly Random Rand = new Random();

    private static readonly CultureInfo culture = ConfigFileUtils.GetCulture();

    /**
     * Do not use. Only for constructing Mono temp objects
     */
    public BaseGrid() { }

    /**
     * Starts off a grid at the specified start point.
     * @param startPoint Top-left point of the grid
     * @param hexagonsPerRow Number of hexagons per row
     */
    public BaseGrid(Vector2 startPoint, int[] hexagonsPerRow)
    {
        Position = startPoint;
        HexagonsPerRow = hexagonsPerRow;
    }

    /**
     * Select the hexagon at the specified position
     * @param i Row to select
     * @param j Column to select
     * @param playerIndex Currently acting player
     */
    public void SetSelectedHexagon(int i, int j, int playerIndex)
    {
        Array[i][j].Selected[playerIndex] = true;
        SelectedHexagons[playerIndex] = Array[i][j];
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
     * @param playerIndex Currently active player
     */
    public void RotateSelected(Direction direction, int playerIndex)
    {
        HandleRotation(SelectedHexagons[playerIndex], direction, playerIndex);
    }

    protected void HandleRotation(Hexagon affectedHexagon, Direction direction, int playerIndex)
    {
        affectedHexagon.Rot(direction);
        Color colorToFlash = Hexagon.DefaultHexColor;
        HashSet<Hexagon> matchedHexagons = GridUtils.CheckMatches(Array, affectedHexagon, out Godot.Collections.Dictionary<Color, int> matchedColors, HexagonsPerRow);
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
        EmitSignal(nameof(HexagonRotated), affectedHexagon, hexagons, matchedColors, playerIndex);
    }

    protected void HandlePowerUp()
    {
        // FIXME: the grid really should not be handling button presses
        // not to mention that this signal is definitely not something it should be handling either
        EmitSignal(nameof(PowerUpActivated));
    }

    protected Hexagon GetAffectedHexagon(Vector2 clickPos)
    {
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
