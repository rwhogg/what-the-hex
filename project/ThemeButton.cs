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

public abstract class ThemeButton : Button
{
    private AudioStreamPlayer ClickButtonSound;
    private AudioStreamPlayer SelectButtonSound;

    public override void _EnterTree()
    {
        base._EnterTree();

        ClickButtonSound = new ClickButtonSound();
        AddChild(ClickButtonSound);
        SelectButtonSound = new SelectButtonSound();
        AddChild(SelectButtonSound);
        Connect("focus_entered", this, nameof(PlaySelectSound));
        Connect("mouse_entered", this, nameof(PlaySelectSound));
        ClickButtonSound.Connect("finished", this, nameof(ChangeScene));
    }

    /**
     * Called when the button is pressed.
     */
    public override void _Pressed()
    {
        base._Pressed();
        ClickButtonSound.Play();
    }

    protected virtual void ChangeScene() { }

    private void PlaySelectSound()
    {
        SelectButtonSound.Play();
    }
}
