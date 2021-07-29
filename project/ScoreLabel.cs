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
using System.Globalization;

using Godot;

public class ScoreLabel : RichTextLabel
{
    private CultureInfo Culture = ConfigFileUtils.GetCulture();

    public override void _Process(float delta)
    {
        Timer timer = GetTree().Root.GetNode<Timer>("GameComponent/GameTimer");
        GameComponent game = GetTree().Root.GetNode<GameComponent>("GameComponent");
        int timeLeft = (int)timer.TimeLeft;
        int score = game.Score;
        int matches = game.NumMatchesMade;
        int hiscore = game.HiScore;
        BbcodeText = String.Format(Culture, "[color=red]TIME {0} MATCH {2} SCORE {1} HISCORE {3}[/color]", timeLeft, score, matches, hiscore);
    }
}
