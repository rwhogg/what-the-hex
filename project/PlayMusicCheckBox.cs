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
 * Script for the PlayMusicCheckBox in the settings scene
 */
public class PlayMusicCheckBox : CheckBox
{
    /**
     * Check the config file to see if we are configured to play music or not.
     * Called when this control enters the scene tree.
     */
    public override void _Ready()
    {
        Pressed = ConfigFileUtils.ShouldPlayMusic();
    }
}
