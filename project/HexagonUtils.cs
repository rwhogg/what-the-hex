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
using System.Collections.ObjectModel;

using Godot;

public static class HexagonUtils
{
    private static readonly Random Rand = new Random();

    /**
     * Create a new hexagon with randomly chosen edge colors
     * @param center Center position
     * @param baseColor Base color
     * @return A random hexagon
     */
    public static Hexagon RandomHexagon(Vector2 center, Color baseColor)
    {
        return RandomHexagon(center, baseColor, Hexagon.EdgeColorOptions);
    }

    /**
     * Create a new hexagon with randomly chosen edge colors
     * @param center Center position
     * @param baseColor Base color
     * @param edgeColorOptions Edge color options
     * @return A random hexagon
     */
    public static Hexagon RandomHexagon(Vector2 center, Color baseColor, Color[] edgeColorOptions)
    {
        Collection<Color> edgeColors = RandomEdgeColors(edgeColorOptions);
        return new Hexagon(center, baseColor, edgeColors);
    }

    /**
     * Get a list of edge colors
     * @param edgeColorOptions Edge color options
     * @return A list of randomly chosen edge colors
     */
    public static Collection<Color> RandomEdgeColors(Color[] edgeColorOptions)
    {
        var edgeColors = new Collection<Color>();
        for(var i = 0; i < 6; i++)
        {
            edgeColors.Add(RandomColor(edgeColorOptions));
        }
        return edgeColors;
    }

    /**
     * Get a random edge color from the default color set
     * @return Color A randomly chosen edge color
     */
    public static Color RandomColor()
    {
        return RandomColor(Hexagon.EdgeColorOptions);
    }

    /**
     * Get a random edge color
     * @param edgeColorOptions Edge color options
     * @return Color A randomly chosen edge color
     */
    public static Color RandomColor(Color[] edgeColorOptions)
    {
        if(edgeColorOptions == null)
        {
            throw new ArgumentNullException(nameof(edgeColorOptions));
        }

#pragma warning disable CA5394
        return edgeColorOptions[Rand.Next(0, edgeColorOptions.Length)];
#pragma warning restore CA5394
    }
}
