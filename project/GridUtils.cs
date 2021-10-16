using System;
using System.Collections.Generic;

using Godot;

class GridUtils
{
    enum Side
    {
        TL,
        TR,
        BL,
        BR
    }

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

        // FIXME: this logic might have to be adjusted to handle layouts with unequal columns per row

        // top-left
        if(row != 0 && column != 0)
        {
            Hexagon hexNW = Array[row - 1][column - 1];
            Hexagon hexN = Array[row - 1][column];
            Hexagon hexW = Array[row][column - 1];
            if(CheckSide(affectedHexagon, Side.TL, Array))
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
            if(CheckSide(affectedHexagon, Side.TR, Array))
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
            if(CheckSide(affectedHexagon, Side.BR, Array))
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
            if(CheckSide(affectedHexagon, Side.BL, Array))
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

    private static bool CheckSide(Hexagon affectedHexagon, Side side, Hexagon[][] Array)
    {
        int tre = 0;
        int bre = 1;
        int ble = 3;
        int tle = 4;
        int row = (int)affectedHexagon.I;
        int column = (int)affectedHexagon.J;

        int[] n1;
        int[] n2;
        int[] n3;
        int[] edgesToCheck;
        int originalEdge;

        switch(side)
        {
            case Side.TL:
                n1 = new int[]{row - 1, column - 1};
                n2 = new int[]{row - 1, column};
                n3 = new int[]{row, column - 1};
                edgesToCheck = new int[]{bre, ble, tre};
                originalEdge = tle;
                break;
            case Side.TR:
                n1 = new int[]{row - 1, column + 1};
                n2 = new int[]{row - 1, column};
                n3 = new int[]{row, column + 1};
                edgesToCheck = new int[]{ble, bre, tle};
                originalEdge = tre;
                break;
            case Side.BR:
                n1 = new int[]{row + 1, column + 1};
                n2 = new int[]{row + 1, column};
                n3 = new int[]{row, column + 1};
                edgesToCheck = new int[]{tle, tre, ble};
                originalEdge = bre;
                break;
            case Side.BL:
                n1 = new int[]{row + 1, column - 1};
                n2 = new int[]{row + 1, column};
                n3 = new int[]{row, column - 1};
                edgesToCheck = new int[]{tre, tle, bre};
                originalEdge = ble;
                break;
            default:
                throw new Exception("BAD SIDE CHECK");
        }

        Color colorToCheck = affectedHexagon.EdgeColors[originalEdge];
        bool n1eq = colorToCheck == Array[n1[0]][n1[1]].EdgeColors[edgesToCheck[0]];
        bool n2eq = colorToCheck == Array[n2[0]][n2[1]].EdgeColors[edgesToCheck[1]];
        bool n3eq = colorToCheck == Array[n3[0]][n3[1]].EdgeColors[edgesToCheck[2]];

        return n1eq && n2eq && n3eq;
    }
}
