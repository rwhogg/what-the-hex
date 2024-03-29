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
public class Menu: Control
{
    private Button[] Buttons;
    private int selected = -1;

    private const int QuitButtonIndex = 6;

    /**
     * Set up the button indices.
     * Called when this node enters the scene tree.
     */
    public override void _Ready()
    {
        selected = 0;
        Buttons = new Button[QuitButtonIndex + 1];
        Buttons[0] = GetNode<Button>("ButtonContainer/PlayButton");
        Buttons[1] = GetNode<Button>("ButtonContainer/MultiplayerButton");
        Buttons[2] = GetNode<Button>("ButtonContainer/SettingsButton");
        Buttons[3] = GetNode<Button>("ButtonContainer/CreditsButton");
        Buttons[4] = GetNode<Button>("ButtonContainer/AboutButton");
        Buttons[5] = GetNode<Button>("ButtonContainer/BackButton");
        Buttons[QuitButtonIndex] = GetNode<Button>("ButtonContainer/QuitButton");

        Buttons[selected].GrabFocus();
    }

    /**
     * Processes the input.
     * @param event Input event
     */
    public override void _Input(InputEvent @event)
    {
        if(@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if(@event.IsActionPressed("ui_up"))
        {
            selected = Math.Max(selected - 1, 0);
            Buttons[selected].GrabFocus();
        }
        else if(@event.IsActionPressed("ui_down"))
        {
            selected = Math.Min(selected + 1, QuitButtonIndex);
            Buttons[selected].GrabFocus();
        }
        else if(@event.IsActionPressed("ui_select"))
        {
            selected = (selected + 1) % (QuitButtonIndex + 1);
            Buttons[selected].GrabFocus();
        }
        else if(@event.IsActionPressed("ui_cancel"))
        {
            Buttons[QuitButtonIndex]._Pressed();
        }
        // Note: no need to handle ui_accept here
        // it seems like Godot has already bound that for me
    }
}
