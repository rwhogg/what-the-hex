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

public static class GridUtils
{
    enum Side
    {
        TL,
        TR,
        BL,
        BR
    }

    public static HashSet<Hexagon> CheckMatches(Hexagon[][] HexArray, Hexagon affectedHexagon, out Godot.Collections.Dictionary<Color, int> matchedColors, int[] HexagonsPerRow)
    {
        if(HexArray == null)
        {
            throw new ArgumentNullException(nameof(HexArray));
        }
        if(affectedHexagon == null)
        {
            throw new ArgumentNullException(nameof(affectedHexagon));
        }
        if(HexagonsPerRow == null)
        {
            throw new ArgumentNullException(nameof(HexagonsPerRow));
        }

        var matchedHexagons = new HashSet<Hexagon>();
        matchedColors = new Godot.Collections.Dictionary<Color, int>();
        var tre = 0;
        var bre = 1;
        var ble = 3;
        var tle = 4;
        var row = (int)affectedHexagon.I;
        var column = (int)affectedHexagon.J;

        // FIXME: this logic might have to be adjusted to handle layouts with unequal columns per row

        // top-left
        if(row != 0 && column != 0)
        {
            Hexagon hexNW = HexArray[row - 1][column - 1];
            Hexagon hexN = HexArray[row - 1][column];
            Hexagon hexW = HexArray[row][column - 1];
            if(CheckSide(affectedHexagon, Side.TL, HexArray))
            {
                matchedHexagons.UnionWith(new Hexagon[] { affectedHexagon, hexNW, hexN, hexW });
                // No need to += 1 here because this is definitely the first instance
                matchedColors[affectedHexagon.EdgeColors[tle]] = 1;
            }
        }
        // top-right
        if(row != 0 && column != HexagonsPerRow[row] - 1)
        {
            Hexagon hexNE = HexArray[row - 1][column + 1];
            Hexagon hexN = HexArray[row - 1][column];
            Hexagon hexE = HexArray[row][column + 1];
            if(CheckSide(affectedHexagon, Side.TR, HexArray))
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
            Hexagon hexSE = HexArray[row + 1][column + 1];
            Hexagon hexS = HexArray[row + 1][column];
            Hexagon hexE = HexArray[row][column + 1];
            if(CheckSide(affectedHexagon, Side.BR, HexArray))
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
            Hexagon hexSW = HexArray[row + 1][column - 1];
            Hexagon hexS = HexArray[row + 1][column];
            Hexagon hexW = HexArray[row][column - 1];
            if(CheckSide(affectedHexagon, Side.BL, HexArray))
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

    /**
     * Create a new grid of hexagons, each of which has random edge colors
     * @param basePosition Position of the top-left hexagon in the grid
     * @param hexagonsPerRow Number of hexagons that appear in each row
     * @param baseColor Base color of every hexagon
     * @param isInert If true, return an inert grid
     */
    public static BaseGrid RandomHexagonGrid(Vector2 basePosition, int[] hexagonsPerRow, Color baseColor, bool isInert)
    {
        if(hexagonsPerRow == null)
        {
            throw new ArgumentNullException(nameof(hexagonsPerRow));
        }
        BaseGrid grid = new Grid(basePosition, hexagonsPerRow);
        if(isInert)
        {
            grid = new BaseGrid(basePosition, hexagonsPerRow);
        }
        var numRows = hexagonsPerRow.Length;
        var hexArray = new Hexagon[numRows][];
        for(var i = 0; i < numRows; i++)
        {
            hexArray[i] = new Hexagon[hexagonsPerRow[i]];
            for(var j = 0; j < hexagonsPerRow[i]; j++)
            {
                var xPos = (float)(j * Hexagon.BigRadius() * 2);
                var yPos = (float)(i * Hexagon.SmallRadius() * 2);

                // Bit of a buffer so the edge colors don't overlap
                // (the hardcoded constants are just eyeballed to look reasonable)
                // but that said, they were used in deriving the math for GetAffectedHexagon(),
                // so don't change them without thinking
                xPos += Hexagon.EdgeThickness * j * 0.2F;
                yPos += Hexagon.EdgeThickness * i * 0.8F;

                var position = new Vector2(xPos, yPos);
                hexArray[i][j] = HexagonUtils.RandomHexagon(position, baseColor);
                hexArray[i][j].I = i;
                hexArray[i][j].J = j;
                grid.AddChild(hexArray[i][j], true);
            }
        }

        grid.HexArray = hexArray;
        return grid;
    }

    private static bool CheckSide(Hexagon affectedHexagon, Side side, Hexagon[][] HexArray)
    {
        var tre = 0;
        var bre = 1;
        var ble = 3;
        var tle = 4;
        var row = (int)affectedHexagon.I;
        var column = (int)affectedHexagon.J;

        int[] n1;
        int[] n2;
        int[] n3;
        int[] edgesToCheck;
        int originalEdge;

        switch(side)
        {
            case Side.TL:
                n1 = new int[] { row - 1, column - 1 };
                n2 = new int[] { row - 1, column };
                n3 = new int[] { row, column - 1 };
                edgesToCheck = new int[] { bre, ble, tre };
                originalEdge = tle;
                break;
            case Side.TR:
                n1 = new int[] { row - 1, column + 1 };
                n2 = new int[] { row - 1, column };
                n3 = new int[] { row, column + 1 };
                edgesToCheck = new int[] { ble, bre, tle };
                originalEdge = tre;
                break;
            case Side.BR:
                n1 = new int[] { row + 1, column + 1 };
                n2 = new int[] { row + 1, column };
                n3 = new int[] { row, column + 1 };
                edgesToCheck = new int[] { tle, tre, ble };
                originalEdge = bre;
                break;
            case Side.BL:
                n1 = new int[] { row + 1, column - 1 };
                n2 = new int[] { row + 1, column };
                n3 = new int[] { row, column - 1 };
                edgesToCheck = new int[] { tre, tle, bre };
                originalEdge = ble;
                break;
            default:
                throw new ArgumentException("BAD SIDE CHECK");
        }

        return CheckEdges(affectedHexagon, HexArray, n1, n2, n3, edgesToCheck, originalEdge);
    }

    private static bool CheckEdges(Hexagon affectedHexagon, Hexagon[][] HexArray, int[] n1, int[] n2, int[] n3, int[] edgesToCheck, int originalEdge)
    {
        Color colorToCheck = affectedHexagon.EdgeColors[originalEdge];
        return
            (colorToCheck == HexArray[n1[0]][n1[1]].EdgeColors[edgesToCheck[0]]) &&
            (colorToCheck == HexArray[n2[0]][n2[1]].EdgeColors[edgesToCheck[1]]) &&
            (colorToCheck == HexArray[n3[0]][n3[1]].EdgeColors[edgesToCheck[2]]);
    }
}
