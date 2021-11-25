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

/**
 * Script for the bottom status bar
 */
public class BottomStatusLabel: RichTextLabel
{
    private int RecentScore;

    private readonly CultureInfo culture = ConfigFileUtils.GetCulture();

    /**
     * Called once per frame.
     * Reads statistics from the GameComponent and displays them at the bottom
     */
    public override void _Process(float delta)
    {
        base._Process(delta);
        var gameComponent = GetTree().Root.GetNode<GameComponent>("GameComponent");
        var colorName = Utils.ColorMap(gameComponent.AdvantageColor);
        var advantageTimeLeft = ((int)gameComponent.GetNode<Timer>("AdvantageTimer").TimeLeft).ToString(culture);
        var refreshTimeLeft = ((int)gameComponent.GetNode<Timer>("RefreshTimer").TimeLeft).ToString(culture);
        var advantageLabel = Tr("ADVANTAGE");
        var refreshLabel = Tr("REFRESH");
        var advantageColor = (colorName == "pink" ? "#ff69b4" : colorName);
        var recentScoreToShow = RecentScore > 0 ? "+" + RecentScore.ToString(culture) : String.Empty;
        BbcodeText = String.Format(
            culture,
            "{0}: [color={1}]{2} {3}[/color] {4}: {5}\t{6}",
            advantageLabel,
            advantageColor,
            colorName,
            advantageTimeLeft,
            refreshLabel,
            refreshTimeLeft,
            recentScoreToShow
        );
    }

    /**
     * Show the score increment
     * @param score Additional score to show
     */
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
