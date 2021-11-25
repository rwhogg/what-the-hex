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

public class MultiplayerButton: ThemeButton
{
    /**
     * Called when this button and its children enter the scene tree.
     */
    public override void _Ready()
    {
        CheckDisable();
        Input.Singleton.Connect("joy_connection_changed", this, nameof(CheckDisable));
    }

    /**
     * Called when the button is pressed.
     */
    protected override void ChangeScene()
    {
        RuntimeConfig.Is2Player = true;
        GetTree().ChangeScene(ResourcePaths.MULTIPLAYER_GAME_CONFIG_SCENE);
    }

    private void CheckDisable()
    {
        Disabled = ShouldDisable();
    }

#pragma warning disable CA1801
    private void CheckDisable(int device, bool connected)
#pragma warning restore CA1801
    {
        // this overload (2 arguments) is only needed so that the signal signatures match
        CheckDisable();
    }

    private static bool ShouldDisable()
    {
        return Utils.GetNumControllers() < 2;
    }
}
