using Godot;
using System;
using System.Globalization;

public class TitleText : RichTextLabel
{
    private readonly CultureInfo Culture = ConfigFileUtils.GetCulture();

    public override void _Ready()
    {
        BbcodeText = String.Format(Culture, "[color=red][center]{0}[/center][/color]", Tr("GAME_TITLE_CLEAN"));
    }
}
