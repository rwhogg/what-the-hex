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
 *  Label that displays score details
 */
public class TwoPlayerScoreLabel : ScoreLabel
{
    /**
     * Called once per frame.
     * @param delta Time since last frame
     */
    public override void _Process(float delta)
    {
        base._Process(delta);
        Timer timer = GetTree().Root.GetNode<Timer>("GameComponent/GameTimer");
        GameComponent game = GetTree().Root.GetNode<GameComponent>("GameComponent");

        if(!Visible)
        {
            return;
        }

        int timeLeft = (int)timer.TimeLeft;
        int score1 = game.Scores[0];
        int score2 = game.Scores[1];
        BbcodeText = String.Format(Culture,
            "[color=red]{0}: {1}[/color]\t\t\t\t\t\t\t[color=blue]{2}: {3}[/color]\t\t\t\t\t\t\t\t[color=green]{4}: {5}[/color]",
            Tr("PLAYER_ONE"),
            score1,
            Tr("TIME"),
            timeLeft,
            Tr("PLAYER_TWO"),
            score2
        );
    }
}
