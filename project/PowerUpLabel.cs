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

using static Godot.Mathf;

public class PowerUpLabel : Label
{
    public override void _Process(float delta)
    {
        GameComponent gameComponent = GetTree().Root.GetNode<GameComponent>("GameComponent");
        string advantageLeft = Max(3 - gameComponent.NumAdvantageMatchesMade, 0).ToString();
        Text = "Powerup\nNext: " + advantageLeft;
    }
}
