using System;
using System.Collections.Generic;

using Godot;

public class HexagonUtils
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
        List<Color> edgeColors = RandomEdgeColors(edgeColorOptions);
        return new Hexagon(center, baseColor, edgeColors);
    }

    /**
     * Get a list of edge colors
     * @param edgeColorOptions Edge color options
     * @return A list of randomly chosen edge colors
     */
    public static List<Color> RandomEdgeColors(Color[] edgeColorOptions)
    {
        List<Color> edgeColors = new List<Color>();
        for(int i = 0; i < 6; i++)
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
    #pragma warning disable CA5394
        return edgeColorOptions[Rand.Next(0, edgeColorOptions.Length)];
    #pragma warning restore CA5394
    }
}
