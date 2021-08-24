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
 * Globals that we can use to configure the game scene
 */
public class RuntimeConfig : Node
{
    /**
     * True if we are in 2-player mode
     */
    public static bool Is2Player { get; set; }

    /**
     * Lists the number of hexagons in each row
     * (the number of columns is determined by the length)
     */
    public static int[] HexesPerRow { get; set; }

    public static int MatchesNeeded { get; set; }

    private static PopupDialog popup;

    public override void _Ready()
    {
        base._Ready();
        popup = null;
    }

    public static void DefaultLayout()
    {
        RuntimeConfig.HexesPerRow = new int[5];
        for(int i = 0; i < 5; i++)
        {
            RuntimeConfig.HexesPerRow[i] = 8;
        }
    }

    public override async void _Process(float delta)
    {
        base._Process(delta);

        var sceneTree = GetTree();
        var root = sceneTree.Root;
        var gameComponent = root.GetNodeOrNull<GameComponent>("GameComponent");
        if(gameComponent == null)
        {
            return;
        }

        if(Input.IsActionJustPressed("ui_pause"))
        {
            var pauseSoundPlayer = gameComponent.GetNode<AudioStreamPlayer>("PauseSoundPlayer");
            pauseSoundPlayer.Play();
            await ToSignal(pauseSoundPlayer, "finished");

            if(popup == null)
            {
                popup = ResourceLoader.Load<PackedScene>("res://PausePopup.tscn").Instance<PopupDialog>();
                gameComponent.AddChild(popup);
                popup.GetNode("ButtonContainer/ResumeButton").Connect("pressed", this, nameof(Resume));
                popup.PopupCentered();
                sceneTree.Paused = true;
            }
        }
    }

    public static void NukePopup()
    {
        if(popup != null)
        {
            popup.QueueFree();
            popup = null;
        }
    }

    private async void Resume()
    {
        var sceneTree = GetTree();
        sceneTree.Paused = false;
        NukePopup();

        var pauseSoundPlayer = sceneTree.Root.GetNodeOrNull<AudioStreamPlayer>("GameComponent/PauseSoundPlayer");
        pauseSoundPlayer.Play();
        await ToSignal(pauseSoundPlayer, "finished");
    }
}
