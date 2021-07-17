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

using Godot;

/**
 * Script for the refresh timer label in the game scene
 */
public class RefreshLabel : RichTextLabel
{
    /**
     * Updates the refresh timer display.
     * Called once per frame.
     * @param delta Time in seconds since last frame.
     */
    public override void _Process(float delta)
    {
        Timer timer = GetTree().Root.GetNode<Timer>("GameComponent/RefreshTimer");
        int timeLeft = (int)timer.TimeLeft + 1;
        this.BbcodeText = String.Format("[color=red][right]{0}[/right][/color]", timeLeft);
    }
}
