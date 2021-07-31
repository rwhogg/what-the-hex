using Godot;
using System;

public class EdgeThicknessSlider : HSlider
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Value = ConfigFileUtils.GetEdgeThickness();
    }
}
