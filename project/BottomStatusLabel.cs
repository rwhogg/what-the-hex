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

public class BottomStatusLabel : RichTextLabel
{
    private int RecentScore;

    public override void _Process(float delta)
    {
        base._Process(delta);
        GameComponent gameComponent = GetTree().Root.GetNode<GameComponent>("GameComponent");
        string colorName = Utils.ColorMap(gameComponent.AdvantageColor);
        int advantageTimeLeft = (int)gameComponent.GetNode<Timer>("AdvantageTimer").TimeLeft;
        int refreshTimeLeft = (int)gameComponent.GetNode<Timer>("RefreshTimer").TimeLeft;
        BbcodeText = "Advantage: [color=" + (colorName == "pink" ? "#ff69b4" : colorName) + "]" + colorName + "[/color] " + advantageTimeLeft +
            "   Refresh: " + refreshTimeLeft +
            "   " + (RecentScore > 0 ? "+" + RecentScore.ToString() : "");

    }

    public void FlashScore(int score)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(1.0f);
        timer.Connect("timeout", this, nameof(ResetRecentScore));
        RecentScore = score;
    }

    private void ResetRecentScore()
    {
        RecentScore = 0;
    }
}
