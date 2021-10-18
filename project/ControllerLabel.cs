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
using System.Globalization;

using Godot;

public class ControllerLabel : RichTextLabel
{
#pragma warning disable CS0649
    [Export]
    private readonly int ControllerIndex;
#pragma warning restore CS0649

    private readonly CultureInfo culture = ConfigFileUtils.GetCulture();

    public override void _Ready()
    {
        BbcodeText = String.Format(
            culture,
            "[color={0}]P{1}: {2}[/color]",
            Utils.ColorMap(Colors.GetPlayerColors()[ControllerIndex]),
            ControllerIndex + 1,
            GetControllerName()
        );
        GD.Print(BbcodeText);
    }

    private String GetControllerName()
    {
        return Input.GetJoyName(ControllerIndex);
    }
}
