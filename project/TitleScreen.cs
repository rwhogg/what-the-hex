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



using System.IO;

using Godot;

/**
 * TitleScreen is the extension script for the title screen.
 */
public class TitleScreen : Control
{
    private static readonly RandomNumberGenerator Random = new RandomNumberGenerator();

    private const string StarGroupName = "stars";

    private const string TIMEOUT_SIGNAL = "timeout";

    private bool CanStart;

    /**
     * Called when this node and all its children enter the scene tree.
     */
    public override void _Ready()
    {
        try
        {
            ControllerMappings.SetUpMappings();
            OS.SetWindowTitle(Tr("GAME_TITLE_CLEAN"));
            Timer starCreateTimer = GetNode<Timer>("StarCreateTimer");
            Timer starDeleteTimer = GetNode<Timer>("StarDeleteTimer");
            starCreateTimer.Connect(TIMEOUT_SIGNAL, this, nameof(GenerateStars));
            starDeleteTimer.Connect(TIMEOUT_SIGNAL, this, nameof(RemoveStars));
            GetNode<AudioStreamPlayer>("TitleTextAudio").Connect("finished", this, nameof(StartStarTimers));

            // When you go back to the title screen from the menu, it's sucking up input events from the menu
            // which immediately dumps you back in the menu...
            SceneTreeTimer startTimer = GetTree().CreateTimer(1.0f);
            startTimer.Connect(TIMEOUT_SIGNAL, this, nameof(AllowStart));
        }
        catch(IOException)
        {
            // Don't want to crash just because of this
        }

        Random.Randomize();
        GenerateStars();
    }

    /**
     * Called once per frame.
     * Handles the input.
     * @param delta Time since last frame
     */
    public override void _Process(float delta)
    {
        if(!CanStart)
        {
            return;
        }
        if(Input.IsActionJustPressed("ui_start") || Input.IsActionJustPressed("ui_accept"))
        {
            GoToMenu();
        }
    }

    /**
     * Handles otherwise unhandled touch events, to enter the menu screen
     * @param event Input event
     */
    public override void _UnhandledInput(InputEvent @event)
    {
        // This is for Android touch events, because not handled by the input map
        GetTree().SetInputAsHandled();
        if(@event is InputEventScreenTouch)
        {
            GoToMenu();
        }
    }

    private void GoToMenu()
    {
        GetTree().ChangeScene("res://Menu.tscn");
    }

    private void GenerateStars()
    {
        for(int i = 1; i <= 5; i++)
        {
#pragma warning disable CA2000
            StarSprite star = new StarSprite
            {
                Position = new Vector2(Random.RandiRange(100, 300), Random.RandiRange(100, 400))
            }; // these are later disposed by Godot
            AddChild(star);
            star.AddToGroup(StarGroupName);
        }
        for(int i = 1; i <= 5; i++)
        {
            StarSprite star = new StarSprite
            {
#pragma warning restore CA2000
                Position = new Vector2(Random.RandiRange(600, 800), Random.RandiRange(100, 400))
            }; // these are later disposed by Godot
            AddChild(star);
            star.AddToGroup(StarGroupName);
        }
    }

    private void RemoveStars()
    {
        SceneTree tree = GetTree();
        foreach(Node2D item in tree.GetNodesInGroup(StarGroupName))
        {
            RemoveChild(item);
            item.QueueFree();
        }
    }

    private void AllowStart()
    {
        CanStart = true;
    }

    private void StartStarTimers()
    {
        Timer starCreateTimer = GetNode<Timer>("StarCreateTimer");
        Timer starDeleteTimer = GetNode<Timer>("StarDeleteTimer");
        starCreateTimer.Start(0.5f);
        starDeleteTimer.Start(1.0f);
    }
}
