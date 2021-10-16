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

    /**
     * Create a new grid of hexagons, each of which has random edge colors
     * @param basePosition Position of the top-left hexagon in the grid
     * @param hexagonsPerRow Number of hexagons that appear in each row
     * @param baseColor Base color of every hexagon
     */
    public static Grid RandomHexagonGrid(Vector2 basePosition, int[] hexagonsPerRow, Color baseColor)
    {
        if(hexagonsPerRow == null)
        {
            throw new ArgumentNullException(nameof(hexagonsPerRow));
        }
        Grid grid = new Grid(basePosition, hexagonsPerRow);
        int numRows = hexagonsPerRow.Length;
        Hexagon[][] array = new Hexagon[numRows][];
        for(int i = 0; i < numRows; i++)
        {
            array[i] = new Hexagon[hexagonsPerRow[i]];
            for(int j = 0; j < hexagonsPerRow[i]; j++)
            {
                float xPos = (float)(j * Hexagon.BigRadius() * 2);
                float yPos = (float)(i * Hexagon.SmallRadius() * 2);

                // Bit of a buffer so the edge colors don't overlap
                // (the hardcoded constants are just eyeballed to look reasonable)
                // but that said, they were used in deriving the math for GetAffectedHexagon(),
                // so don't change them without thinking
                xPos += Hexagon.EdgeThickness * j * 0.2F;
                yPos += Hexagon.EdgeThickness * i * 0.8F;

                Vector2 position = new Vector2(xPos, yPos);
                array[i][j] = HexagonUtils.RandomHexagon(position, baseColor);
                array[i][j].I = i;
                array[i][j].J = j;
                grid.AddChild(array[i][j], true);
            }
        }

        grid.Array = array;
        return grid;
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
                throw new Exception("BAD SIDE CHECK");
        }

        return CheckEdges(affectedHexagon, Array, n1, n2, n3, edgesToCheck, originalEdge);
    }

    private static bool CheckEdges(Hexagon affectedHexagon, Hexagon[][] Array, int[] n1, int[] n2, int[] n3, int[] edgesToCheck, int originalEdge)
    {
        Color colorToCheck = affectedHexagon.EdgeColors[originalEdge];
        return
            (colorToCheck == Array[n1[0]][n1[1]].EdgeColors[edgesToCheck[0]]) &&
            (colorToCheck == Array[n2[0]][n2[1]].EdgeColors[edgesToCheck[1]]) &&
            (colorToCheck == Array[n3[0]][n3[1]].EdgeColors[edgesToCheck[2]]);
    }
}
