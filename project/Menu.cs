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
 * Script for the Menu screen.
 */
public class Menu : Control
{
    private Button[] Buttons;
    private int selected = -1;

    private static int BackButtonIndex = 4;

    /**
     * Set up the button indices.
     * Called when this node enters the scene tree.
     */
    public override void _Ready()
    {
        selected = 0;
        Buttons = new Button[5];
        Buttons[0] = GetNode<Button>("ButtonContainer/PlayButton");
        Buttons[1] = GetNode<Button>("ButtonContainer/SettingsButton");
        Buttons[2] = GetNode<Button>("ButtonContainer/CreditsButton");
        Buttons[3] = GetNode<Button>("ButtonContainer/AboutButton");
        Buttons[BackButtonIndex] = GetNode<Button>("ButtonContainer/BackButton");

        Buttons[selected].GrabFocus();
    }

    /**
     * Processes the input. Support for up and down.
     */
    public override void _Input(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("ui_up"))
        {
            selected = Math.Max(selected - 1, 0);
            Buttons[selected].GrabFocus();
        }
        else if(inputEvent.IsActionPressed("ui_down"))
        {
            selected = Math.Min(selected + 1, BackButtonIndex);
            Buttons[selected].GrabFocus();
        }
        else if(inputEvent.IsActionPressed("ui_cancel"))
        {
            Buttons[BackButtonIndex]._Pressed();
        }
        // Note: no need to handle ui_accept here
        // it seems like Godot has already bound that for me
    }
}
