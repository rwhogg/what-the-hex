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
using System.Diagnostics.Contracts;

using Godot;
using Godot.Collections;

using static Godot.Mathf;

/**
 * Script for the main Game Component.
 */
public class GameComponent : Node2D
{
    /**
     * The current score(s)
     */
    public int[] Scores { get; set; }

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

    public bool GameEnded = false;

    public bool HasWon = false;

    public BaseGrid HexagonGrid;

    private IPowerUp ReservedPowerUp;

    private Vector2 HexagonStartPoint = new Vector2(70, 100);

    private int NumHexagonsToReplace = 3;

    private const float AdvantageTime = 15.0f;

    private const string TimeoutSignal = "timeout";

    private int NumRefreshes;

    private readonly Random MyRandom = new Random();

    private readonly System.Collections.Generic.List<IWinCondition> WinConditions = new System.Collections.Generic.List<IWinCondition>();

    /**
     * Initializes the Game Component, creates the hexagon grid, and sets up event listeners.
     * Called when the Game Component and all its children (except the hexagon grid)
     * have entered the scene tree.
     */
    public override void _Ready()
    {
        HiScore = ConfigFileUtils.LoadHiscore();
        Scores = new int[RuntimeConfig.Is2Player ? 2 : 1];

        Scores[0] = 0;
        if(RuntimeConfig.Is2Player)
        {
            Scores[1] = 0;
            // FIXME: maybe it makes sense to split some of these details into subclasses... and perhaps even scene inheritance?
            GetNode<RichTextLabel>("TopPanelContainer/OnePlayerScoreLabel").Hide();
            GetNode<RichTextLabel>("TopPanelContainer/TwoPlayerScoreLabel").Show();
            GetNode<Label>("PowerUpLabel").Hide(); // FIXME support this for multiplayer too
        }
        else
        {
            GetNode<RichTextLabel>("TopPanelContainer/TwoPlayerScoreLabel").Hide();
        }

        NumMatchesMade = 0;

        InitGrid();

        SetUpButtonHandlers();

        AdvantageColor = HexagonUtils.RandomColor();

        StartTimers();

        GetNode<RichTextLabel>("GameOverLabel").Hide();
        GetNode<RichTextLabel>("YouWinLabel").Hide();
        DisableContinueButton();

        int matchesNeeded = RuntimeConfig.MatchesNeeded > 0 ? RuntimeConfig.MatchesNeeded : 20;
        WinConditions.Add(new NumMatchesWinCondition(matchesNeeded));

        HasWon = false;
        GameEnded = false;
    }

    private void InitGrid()
    {
        if(RuntimeConfig.HexesPerRow == null)
        {
            RuntimeConfig.DefaultLayout();
        }
        HexagonGrid = GridUtils.RandomHexagonGrid(HexagonStartPoint, RuntimeConfig.HexesPerRow, Hexagon.DefaultHexColor, RuntimeConfig.IsInert);
        HexagonGrid.SetSelectedHexagon(0, 0, 0);
        if(RuntimeConfig.Is2Player)
        {
            HexagonGrid.SetSelectedHexagon(1, 0, 1);
        }
        AddChild(HexagonGrid);
        HexagonGrid.Connect(nameof(Grid.HexagonRotated), this, nameof(On_Hexagon_Rotated));

        if(!RuntimeConfig.Is2Player)
        {
            // FIXME: support for multiplayer as well
            HexagonGrid.Connect(nameof(Grid.PowerUpActivated), this, nameof(On_PowerUpActivated));
        }
    }

    private void SetUpButtonHandlers()
    {
        if(RuntimeConfig.IsInert)
        {
            GD.Print("Running in inert mode");
            return;
        }

        var rotateClockwiseButton = GetNodeOrNull<TouchScreenButton>("RotateClockwiseButton");
        if(rotateClockwiseButton != null && rotateClockwiseButton.ShapeVisible)
        {
            if(OS.HasTouchscreenUiHint())
            {
                var rotateCounterClockwiseButton = GetNode<TouchScreenButton>("RotateCounterClockwiseButton");
                rotateClockwiseButton.Connect("pressed", this, nameof(On_TouchRotateClockwise));
                rotateCounterClockwiseButton.Connect("pressed", this, nameof(On_TouchRotateCounterClockwise));
            }
        }

        // Note: this one is always visible (in single player), even on desktop
        if(!RuntimeConfig.Is2Player)
        {
            // FIXME: support this for multiplayer as well
            var powerUpButton = GetNode<TextureButton>("PowerUpContainer/PowerUpButton");
            powerUpButton.Connect("pressed", this, nameof(On_PowerUpActivated));
        }

        var continueButton = GetNode<Button>("ContinueButton");
        continueButton.Connect("pressed", this, nameof(On_ContinueButtonPressed));
    }

    private void StartTimers()
    {
        if(RuntimeConfig.IsInert)
        {
            GD.Print("Running in inert mode, not starting timers");
            return;
        }

        var advantageTimer = GetNode<Timer>("AdvantageTimer");
        advantageTimer.Connect(TimeoutSignal, this, nameof(On_AdvantageTimer_Timeout));

        var gameTimer = GetNode<Timer>("GameTimer");
        gameTimer.Connect(TimeoutSignal, this, nameof(On_GameTimer_Timeout));
        float defaultTime = OS.IsDebugBuild() ? 20.0f : 100.0f;
        gameTimer.Start(RuntimeConfig.GameStartTime > 0 ? RuntimeConfig.GameStartTime : defaultTime);

        var refreshTimer = GetNode<Timer>("RefreshTimer");
        refreshTimer.Connect(TimeoutSignal, this, nameof(On_RefreshTimer_Timeout));
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

        if(RuntimeConfig.IsInert)
        {
            return;
        }
        var music = GetNode<AudioStreamPlayer>("Music");
        _ = music.PitchScale;
        if((int)gameTimer.TimeLeft < 20)
        {
            music.PitchScale = 1.15f;
        }
        else
        {
            music.PitchScale = 1.0f;
        }
    }

    /**
     * Handles input.
     * @param @event Input event.
     */
    public override void _Input(InputEvent @event)
    {
        if(RuntimeConfig.IsInert)
        {
            if(@event is InputEventJoypadButton || @event is InputEventMouseButton || @event is InputEventKey)
            {
                GetTree().ChangeScene("root.tscn");
            }
            return;
        }
        else if(GameEnded)
        {
            // allow continue via gamepad
            if(@event is InputEventJoypadButton eventJoypadButton)
            {
                if((JoystickList)eventJoypadButton.ButtonIndex == JoystickList.Start)
                {
                    On_ContinueButtonPressed();
                    return;
                }
            }
        }
        base._Input(@event);
    }

    private void On_GameTimer_Timeout()
    {
        if(RuntimeConfig.Is2Player)
        {
            DetermineWinner();
        }
        else
        {
            GameOver();
        }
    }

    private void On_AdvantageTimer_Timeout()
    {
        AdvantageColor = HexagonUtils.RandomColor();
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
            GetNode<AudioStreamPlayer>("RefreshSoundPlayer").Play();
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

        // FIXME: see about moving this into a method of the PowerUpButton. Not a huge fan of futzing with the internal textures here
        var powerUpButton = GetNode<TextureButton>("PowerUpContainer/PowerUpButton");
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
    private void On_Hexagon_Rotated(Hexagon rotatedHexagon, Godot.Collections.Array matchedHexagons, Dictionary<Color, int> matchedColors, int playerIndex)
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
        Scores[playerIndex] += additionalScore;
        if(madeAnyAdvantageMatch)
        {
            NumAdvantageMatchesMade++;
            AdvantageColor = HexagonUtils.RandomColor();
            var advantageTimer = GetNode<Timer>("AdvantageTimer");
            advantageTimer.Stop();
            advantageTimer.Start(AdvantageTime);
            if(NumAdvantageMatchesMade == 3)
            {
                if(!RuntimeConfig.Is2Player)
                {
                    // FIXME: support for 2 player as well
                    AssignPowerUp();
                }
                NumAdvantageMatchesMade = 0;
            }
        }
        else if(madeAnyMatch)
        {
            NumAdvantageMatchesMade = 0;
        }

        if(madeAnyMatch && !RuntimeConfig.Is2Player)
        {
            var gameTimer = GetNode<Timer>("GameTimer");
            gameTimer.Start(gameTimer.TimeLeft + 5);
        }

        if(!RuntimeConfig.Is2Player && Scores[0] > HiScore)
        {
            HiScore = Scores[0];
        }
        if(additionalScore > 0)
        {
            var bottomStatusLabel = GetNode<BottomStatusLabel>("BottomPanelContainer/BottomStatusLabel");
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
            HasWon = true;
            EndOfGame();
            GetNode<AudioStreamPlayer>("WinSoundPlayer").Play();
            if(!RuntimeConfig.Is2Player)
            {
                ConfigFileUtils.SaveHiscore(Math.Max(Scores[0], HiScore));
            }
            GetNode<RichTextLabel>("YouWinLabel").Show();
            EnableContinueButton();
        }
    }

    private void AssignPowerUp()
    {
        IPowerUp powerUp = RandomPowerUp();
        ReservedPowerUp = powerUp;
        var powerUpButton = GetNode<TextureButton>("PowerUpContainer/PowerUpButton");
        powerUpButton.TextureNormal = powerUp.GetTexture();
    }

    private IPowerUp RandomPowerUp()
    {
        int random = MyRandom.Next(0, 2);
        if(random == 0)
        {
            return new StopRefreshPowerUp();
        }
        return new EliminateColorPowerUp();
    }

    private void EndOfGame()
    {
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<Timer>("AdvantageTimer").Stop();
        GetNode<Timer>("RefreshTimer").Stop();
        GetNode<Timer>("GameTimer").Stop();
        GetNode<Label>("PowerUpLabel").Hide();
        if(HexagonGrid != null)
        {
            RemoveChild(HexagonGrid);
            HexagonGrid.QueueFree();
            HexagonGrid = null;
        }
        GameEnded = true;
    }

    private void DetermineWinner()
    {
        Contract.Assert(RuntimeConfig.Is2Player);
        EndOfGame();
        if(Scores[0] == Scores[1])
        {
            HandleTie();
        }
        else if(Scores[0] > Scores[1])
        {
            HandleVictory(0);
        }
        else
        {
            HandleVictory(1);
        }
    }

    private void HandleTie()
    {
        var tieSound = GetNode<AudioStreamPlayer>("TieSoundPlayer");
        tieSound.Play();
        EnableContinueButton();
    }

    private async void HandleVictory(int winnerIndex)
    {
        var winnerSound = GetNode<AudioStreamPlayer>("WinSynthVoicePlayer");
        var playerSound = GetNode<AudioStreamPlayer>("Player" + (winnerIndex == 0 ? "One" : "Two") + "SynthVoicePlayer");
        playerSound.Play();
        await ToSignal(playerSound, "finished");
        winnerSound.Play();

        EnableContinueButton();
    }

    private void GameOver()
    {
        EndOfGame();
        ConfigFileUtils.SaveHiscore(HiScore);
        var gameOverSound = GetNode<AudioStreamPlayer>("GameOverSoundPlayer");
        gameOverSound.Play();
        GetNode<RichTextLabel>("GameOverLabel").Show();

        EnableContinueButton();
    }

    private void EnableContinueButton()
    {
        var continueButton = GetNode<Button>("ContinueButton");
        continueButton.Disabled = false;
        continueButton.Show();
    }

    private void DisableContinueButton()
    {
        var continueButton = GetNode<Button>("ContinueButton");
        continueButton.Disabled = true;
        continueButton.Hide();
    }
}
