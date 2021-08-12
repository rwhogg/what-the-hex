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
 * Script for playing music from the title screen
 */
public class TitleMusic : AudioStreamPlayer
{
    /**
     * Called when this node and its children enter the scene tree
     */
    public override void _Ready()
    {
        var titleTextAudio = GetTree().Root.GetNode("MainControl/TitleTextAudio");
        titleTextAudio.Connect("finished", this, nameof(StartMusic));
    }

    private void StartMusic()
    {
        SceneTreeTimer timer = GetTree().CreateTimer(0.5f);
        timer.Connect("timeout", this, nameof(ActuallyPlay));
    }

    private void ActuallyPlay()
    {
        Play();
    }
}
