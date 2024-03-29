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
* This script is shared by the settings, about, and credits back buttons,
* because they all go back to the menu scene.
*/
public class BackToMenuButton: ThemeButton
{
    /**
     * Receives ui_cancel events to go back to the main menu
     * @param event Input event
     */
    public override void _Input(InputEvent @event)
    {
        if(@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        base._Input(@event);
        if(@event.IsActionPressed("ui_cancel"))
        {
            this._Pressed();
        }
    }

    /**
     * Switches back to the menu scene.
     * Called when the button is pressed.
     */
    protected override void ChangeScene()
    {
        GetTree().ChangeScene(ResourcePaths.MENU_SCENE);
    }
}
