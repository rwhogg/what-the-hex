using System;
using System.Collections.Generic;

using Godot;

class GridUtils
{
    public static HashSet<Hexagon> CheckMatches(Hexagon[][] Array, Hexagon affectedHexagon, out Godot.Collections.Dictionary<Color, int> matchedColors, int[] HexagonsPerRow)
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
                matchedHexagons.UnionWith(new Hexagon[] { affectedHexagon, hexNW, hexN, hexW });
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
                matchedHexagons.UnionWith(new Hexagon[] { affectedHexagon, hexNE, hexN, hexE });
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
                matchedHexagons.UnionWith(new Hexagon[] { affectedHexagon, hexSE, hexS, hexE });
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
                matchedHexagons.UnionWith(new Hexagon[] { affectedHexagon, hexSW, hexS, hexW });
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
}
