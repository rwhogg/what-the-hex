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

using static Godot.Mathf;

/**
 * Label saying how many more advantage matches we need until the next powerup
 */
public class PowerUpLabel: Label
{
    private readonly CultureInfo culture = ConfigFileUtils.GetCulture();

    /**
     * Called once per frame.
     * Takes the advantage count left from the GameComponent.
     * @param delta Time since last frame
     */
    public override void _Process(float delta)
    {
        GameComponent gameComponent = GetTree().Root.GetNode<GameComponent>("GameComponent");
        var advantageLeft = Max(3 - gameComponent.NumAdvantageMatchesMade, 0).ToString(culture);
        Text = String.Format(culture, "{0}\n{1}: {2}", Tr("POWERUP"), Tr("NEXT"), advantageLeft);
    }
}
