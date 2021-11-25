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

public class GameConfig : Control
{
    public float GameTime = 100.0f;

    public int NumMatches = 20;

    [Export]
    public bool Is2Player = false;

    public void SetGameTime(float value)
    {
        GameTime = value;
    }

    public void SetNumMatches(int value)
    {
        NumMatches = value;
    }
}
