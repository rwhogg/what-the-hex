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



using Godot;

/**
 * Label that displays "Press Start To Play" or equivalent
 * Label depends on whether or not a controller is detected,
 * and on whether or not we have a touchscreen.
 */
public class PressStart: Label
{
    /**
     * Called when this node and its children enter the scene tree
     */
    public override void _Ready()
    {
        if(Utils.IsControllerMode())
        {
            Text = Tr("PRESS_START_TO_PLAY");
        }
        else if(!OS.HasTouchscreenUiHint())
        {
            Text = Tr("PRESS_ENTER_TO_PLAY");
        }
        else
        {
            Text = Tr("TAP_SCREEN_TO_PLAY");
        }
    }
}
