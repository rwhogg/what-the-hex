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
using Godot.Collections;

using static Godot.Mathf;

/**
 * Script for the main Game Component.
 */
public class GameComponent : Node2D
{
    /**
     * The current score
     */
    public int Score { get; set; }

    /**
     * The current high score (not yet saved necessarily)
     */
    public int HiScore { get; set; }

    /**
     * The color that it is currently most advantageous to target
     */
    public Color AdvantageColor { get; set; }

    /**
     * The number of matches made, counting by rhombuses (NOT by hexagons)
     */
    public int NumMatchesMade { get; set; }

    /**
     * The number of advantageous matches made in a row
     */
    public int NumAdvantageMatchesMade { get; set; }

    private IPowerUp ReservedPowerUp;

    private Grid HexagonGrid;

    private Vector2 HexagonStartPoint = new Vector2(70, 100);

    private int NumHexagonsToReplace = 3;

    private int NumRefreshes;

    private readonly System.Collections.Generic.List<IWinCondition> WinConditions = new System.Collections.Generic.List<IWinCondition>();

    /**
     * Initializes the Game Component, creates the hexagon grid, and sets up event listeners.
     * Called when the Game Component and all its children (except the hexagon grid)
     * have entered the scene tree.
     */
    public override void _Ready()
    {
        HiScore = ConfigFileUtils.LoadHiscore();
        Score = 0;
        NumMatchesMade = 0;

        HexagonGrid = Hexagon.RandomHexagonGrid(HexagonStartPoint, RuntimeConfig.HexesPerRow, Hexagon.DefaultHexColor);
        HexagonGrid.SetSelectedHexagon(0, 0, 0);
        if(RuntimeConfig.Is2Player)
        {
            HexagonGrid.SetSelectedHexagon(1, 0, 1);
        }
        AddChild(HexagonGrid);
        HexagonGrid.Connect(nameof(Grid.HexagonRotated), this, nameof(On_Hexagon_Rotated));

        TouchScreenButton rotateClockwiseButton = GetNodeOrNull<TouchScreenButton>("RotateClockwiseButton");
        if(rotateClockwiseButton != null && rotateClockwiseButton.ShapeVisible)
        {
            if(OS.HasTouchscreenUiHint())
            {
                TouchScreenButton rotateCounterClockwiseButton = GetNode<TouchScreenButton>("RotateCounterClockwiseButton");
                rotateClockwiseButton.Connect("pressed", this, nameof(On_TouchRotateClockwise));
                rotateCounterClockwiseButton.Connect("pressed", this, nameof(On_TouchRotateCounterClockwise));
            }
        }

        // Note: this one is always visible, even on desktop
        var powerUpButton = GetNode<TextureButton>("PowerUpContainer/PowerUpButton");
        powerUpButton.Connect("pressed", this, nameof(On_PowerUpActivated));

        var continueButton = GetNode<Button>("ContinueButton");
        continueButton.Connect("pressed", this, nameof(On_ContinueButtonPressed));

        AdvantageColor = Hexagon.RandomColor();
        var advantageTimer = GetNode<Timer>("AdvantageTimer");
        advantageTimer.Connect("timeout", this, nameof(On_AdvantageTimer_Timeout));

        var gameTimer = GetNode<Timer>("GameTimer");
        gameTimer.Connect("timeout", this, nameof(On_GameTimer_Timeout));
        gameTimer.Start(100.0f);

        Timer refreshTimer = GetNode<Timer>("RefreshTimer");
        refreshTimer.Connect("timeout", this, nameof(On_RefreshTimer_Timeout));

        GetNode<RichTextLabel>("GameOverLabel").Hide();

        WinConditions.Add(new NumMatchesWinCondition(RuntimeConfig.MatchesNeeded));
    }

    /**
     * Redraws once per frame.
     * Called every frame.
     * @param delta The time in seconds since the last frame
     */
    public override void _Process(float delta)
    {
        Update();

        var gameTimer = GetNode<Timer>("GameTimer");
        var music = GetNode<AudioStreamPlayer>("Music");
        _ = music.PitchScale;
        if((int)gameTimer.TimeLeft < 20)
        {
            music.PitchScale = 1.2f;
        }
        else
        {
            music.PitchScale = 1.0f;
        }
    }

    private void On_GameTimer_Timeout()
    {
        GameOver();
    }

    private void On_AdvantageTimer_Timeout()
    {
        AdvantageColor = Hexagon.RandomColor();
    }

    private void On_RefreshTimer_Timeout()
    {
        NumRefreshes = (NumRefreshes + 1) % 3;
        if(NumRefreshes == 0)
        {
            NumHexagonsToReplace++;
        }
        if(HexagonGrid != null)
        {
            HexagonGrid.SelectHexagonsForReplacement(NumHexagonsToReplace);
            AudioStreamPlayer refreshSoundPlayer = GetNode<AudioStreamPlayer>("RefreshSoundPlayer");
            refreshSoundPlayer.Play();
        }
    }

    private void On_TouchRotateClockwise()
    {
        if(HexagonGrid != null)
        {
            HexagonGrid.RotateSelected(Direction.CLOCKWISE, 0);
        }
    }

    private void On_TouchRotateCounterClockwise()
    {
        if(HexagonGrid != null)
        {
            HexagonGrid.RotateSelected(Direction.COUNTERCLOCKWISE, 0);
        }
    }

    private void On_PowerUpActivated()
    {
        IPowerUp currentPowerUp = ReservedPowerUp;
        if(currentPowerUp == null)
        {
            return;
        }
        ReservedPowerUp = null;
        TextureButton powerUpButton = GetNode<TextureButton>("PowerUpContainer/PowerUpButton");
        powerUpButton.TextureNormal = null;

        GetNode<AudioStreamPlayer>("PowerUpSoundPlayer").Play();

        currentPowerUp.ActivateEffect(this);
    }

    private void On_ContinueButtonPressed()
    {
        GetTree().ReloadCurrentScene();
    }

#pragma warning disable IDE0060
#pragma warning disable CA1801
    private void On_Hexagon_Rotated(Hexagon rotatedHexagon, Array matchedHexagons, Dictionary<Color, int> matchedColors)
#pragma warning restore CA1801
#pragma warning restore IDE0060
    {
        int additionalScore = 0;
        bool madeAnyAdvantageMatch = false;
        bool madeAnyMatch = false;
        foreach(var colorCount in matchedColors)
        {
            int numMatched = colorCount.Value;
            bool madeAdvantageMatch = colorCount.Key == AdvantageColor;
            madeAnyAdvantageMatch = madeAnyAdvantageMatch || madeAdvantageMatch;
            int multiplier = madeAdvantageMatch ? 300 : 100;
            additionalScore += (int)Pow(numMatched, 2) * multiplier;
            NumMatchesMade += numMatched;
            if(numMatched > 0)
            {
                madeAnyMatch = true;
            }
        }
        Score += additionalScore;
        if(madeAnyAdvantageMatch)
        {
            NumAdvantageMatchesMade++;
            AdvantageColor = Hexagon.RandomColor();
            Timer advantageTimer = GetNode<Timer>("AdvantageTimer");
            advantageTimer.Stop();
            advantageTimer.Start(15.0f);
            if(NumAdvantageMatchesMade == 3)
            {
                AssignPowerup();
                NumAdvantageMatchesMade = 0;
            }
        }
        else if(madeAnyMatch)
        {
            NumAdvantageMatchesMade = 0;
        }

        if(madeAnyMatch)
        {
            Timer gameTimer = GetNode<Timer>("GameTimer");
            gameTimer.Start(gameTimer.TimeLeft + 5);
        }

        if(Score > HiScore)
        {
            HiScore = Score;
        }
        if(additionalScore > 0)
        {
            BottomStatusLabel bottomStatusLabel = GetNode<BottomStatusLabel>("BottomPanelContainer/BottomStatusLabel");
            bottomStatusLabel.FlashScore(additionalScore);
        }

        var soundPlayer = GetNode<AudioStreamPlayer>(matchedColors.Count > 0 ? "MatchSoundPlayer" : "RotateSoundPlayer");
        soundPlayer.Play();

        if(madeAnyMatch)
        {
            CheckWin();
        }
    }

    private void CheckWin()
    {
        bool hasWon = WinConditions.Count > 0;
        foreach(IWinCondition wc in WinConditions)
        {
            if(!wc.HasWon(this))
            {
                hasWon = false;
            }
        }
        if(hasWon)
        {
            EndOfGame();
            GetNode<AudioStreamPlayer>("WinSoundPlayer").Play();
        }
    }

    private void AssignPowerup()
    {
        // FIXME need random
        IPowerUp powerUp = new StopRefreshPowerUp();
        ReservedPowerUp = powerUp;
        TextureButton powerUpButton = GetNode<TextureButton>("PowerUpContainer/PowerUpButton");
        powerUpButton.TextureNormal = powerUp.GetTexture();
    }

    private void EndOfGame()
    {
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<Timer>("AdvantageTimer").Stop();
        GetNode<Timer>("RefreshTimer").Stop();
        GetNode<Label>("PowerUpLabel").Hide();
        if(HexagonGrid != null)
        {
            RemoveChild(HexagonGrid);
            HexagonGrid.QueueFree();
            HexagonGrid = null;
        }
        ConfigFileUtils.SaveHiscore(HiScore);
    }

    private void GameOver()
    {
        EndOfGame();
        AudioStreamPlayer gameOverSound = GetNode<AudioStreamPlayer>("GameOverSoundPlayer");
        gameOverSound.Play();
        GetNode<RichTextLabel>("GameOverLabel").Show();

        Button continueButton = GetNode<Button>("ContinueButton");
        continueButton.Disabled = false;
        continueButton.Show();
    }
}
