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

public class ContinueButton: ThemeButton
{
    private GameComponent Game;

    public override void _Ready()
    {
        base._Ready();
        Game = GetParent<GameComponent>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var hasWon = Game.HasWon;
        Text = Tr(hasWon ? "PLAY_AGAIN_QUESTION" : "CONTINUE_QUESTION");
    }
}
