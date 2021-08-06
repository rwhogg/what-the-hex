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

    private bool CanStart;

    // FIXME: test this on Android and update KNOWN_ISSUES.md if necessary
    private readonly string N64_REPLICA_MAPPING_WINDOWS = "03000000632500007505000000000000,N64 Replica,a:b1,b:b2,start:b12,leftshoulder:b4,rightshoulder:b5,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a1,righttrigger:b6,platform:Windows";
    private readonly string SN30_PRO_MAPPING_WINDOWS = "03000000c82d00000121000000000000,8BitDo SN30 Pro for Android,a:b0,b:b1,y:b4,x:b3,start:b11,back:b10,leftstick:b13,rightstick:b14,leftshoulder:b6,rightshoulder:b7,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a2,rightx:a5,righty:a5,lefttrigger:b8,righttrigger:b9,platform:Windows";
    private readonly string WIRED_FIGHT_PAD_MAPPING_WINDOWS = "030000006f0e00008501000000000000,Wired Fight Pad Pro for Nintendo Switch,a:b2,b:b1,y:b0,x:b3,start:b9,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,platform:Windows";

    // FIXME: for some reason, the remapping tool didn't really work as I expected here? dpright is excluded from here because it was acting on the left trigger...
    // so instead, just accept for now that the left trigger doesn't work on this one controller
    private readonly string SN30_PRO_MAPPING_ANDROID = "38426974446f20534e33302050726f20,8BitDo SN30 Pro for Android,a:b0,b:b1,y:b3,x:b2,start:b6,back:b4,leftstick:b7,rightstick:b8,leftshoulder:b9,rightshoulder:b10,dpup:b112,dpleft:b14,dpdown:b13,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b9,righttrigger:a4,platform:Android";

    /**
     * Called when this node and all its children enter the scene tree.
     */
    public override void _Ready()
    {
        try
        {
            string osName = OS.GetName();
            if(osName == "Windows")
            {
                Input.AddJoyMapping(SN30_PRO_MAPPING_WINDOWS, true);
                Input.AddJoyMapping(WIRED_FIGHT_PAD_MAPPING_WINDOWS, true);
                Input.AddJoyMapping(N64_REPLICA_MAPPING_WINDOWS, true);
            }
            else if(osName == "Android")
            {
                Input.AddJoyMapping(SN30_PRO_MAPPING_ANDROID, true);
            }

            OS.SetWindowTitle("What The Hex?");
            Timer starCreateTimer = GetNode<Timer>("StarCreateTimer");
            Timer starDeleteTimer = GetNode<Timer>("StarDeleteTimer");
            starCreateTimer.Connect("timeout", this, nameof(GenerateStars));
            starDeleteTimer.Connect("timeout", this, nameof(RemoveStars));
            GetNode<AudioStreamPlayer>("TitleTextAudio").Connect("finished", this, nameof(StartStarTimers));

            // When you go back to the title screen from the menu, it's sucking up input events from the menu
            // which immediately dumps you back in the menu...
            SceneTreeTimer startTimer = GetTree().CreateTimer(1.0f);
            startTimer.Connect("timeout", this, nameof(AllowStart));
        }
        catch(IOException)
        {
            // Don't want to crash just because of this
        }

        Random.Randomize();
        GenerateStars();
    }

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

    public override void _UnhandledInput(InputEvent @event)
    {
        // This is for Android touch events...
        // Apparently the input map didn't really work
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
