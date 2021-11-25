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
    public Collection<Color> EdgeColors { get; }

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

    /**
     * Row index of this hexagon
     */
    public int I { get; set; }

    /**
     * Column index of this hexagon
     */
    public int J { get; set; }

    private DynamicFont RocketFont;

    private Timer HexRefreshTimer;

    private Timer ColorFlashTimer;

    /**
     * How thick this hexagon's edges are, in pixels
     */
    public static readonly int EdgeThickness = ConfigFileUtils.GetEdgeThickness();

    /**
     * Default background hexagon color
     */
    public static readonly Color DefaultHexColor = Black;

    /**
     * List of all possible options for edge colors
     */
    public static readonly Color[] EdgeColorOptions = { Green, Purple, HotPink, Yellow };

    public const float SideLength = 50.0F;

    private static readonly float Cos30 = (float)Cos(Deg2Rad(30));

    private const int DefaultRefreshTimeSeconds = 5;

    private static readonly bool IsDebug = OS.IsDebugBuild();

    private static readonly CultureInfo culture = ConfigFileUtils.GetCulture();

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
    public Hexagon(Vector2 center, Color baseColor, Collection<Color> edgeColors)
    {
        Position = center;
        BaseColor = baseColor;
        EdgeColors = new Collection<Color>(edgeColors);
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
        if(Selected[0])
        {
            DrawCircle(new Vector2(0, 0), 5, Colors.GetPlayerColors()[0]);
        }
        else if(Selected[1])
        {
            DrawCircle(new Vector2(0, 0), 5, Colors.GetPlayerColors()[1]);
        }
        if(IsDebug)
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
        if(direction == Direction.COUNTERCLOCKWISE)
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
        if(IsDebug)
        {
            GD.Print("Marked " + I.ToString(culture) + ", " + J.ToString(culture) + " for replacement");
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
        if(IsDebug)
        {
            GD.Print("Hex " + I.ToString(culture) + "," + J.ToString(culture) + " refreshed");
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

    /**
     * Generate a new set of edge colors for this hexagon
     */
    public void Refresh()
    {
        Collection<Color> newEdgeColors = HexagonUtils.RandomEdgeColors(EdgeColorOptions);
        for(int i = 0; i < EdgeColors.Count; i++)
        {
            EdgeColors[i] = newEdgeColors[i];
        }
    }

    private Color GetHexColor()
    {
        if(MarkedForReplacement)
        {
            return DeepSkyBlue;
        }
        return BaseColor;
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
}
