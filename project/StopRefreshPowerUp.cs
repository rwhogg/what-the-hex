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
 * PowerUp that stops all refreshes for 15 seconds.
 */
public class StopRefreshPowerUp : Godot.Object, IPowerUp
{
    public static readonly ImageTexture Texture = new ImageTexture();

    /**
     * How long the refreshes stop for
     */
    public int StopRefreshTime { get; }

    private float RefreshWaitTime;

    private GameComponent Game;

    public StopRefreshPowerUp()
    {
        StopRefreshTime = 15;

        if(Texture.GetData() == null)
        {
            Image clock = GD.Load<StreamTexture>("res://clock.png").GetData();
            Texture.CreateFromImage(clock);
        }
    }

    public Texture GetTexture()
    {
        return Texture;
    }

    public void ActivateEffect(GameComponent game)
    {
        if(game == null)
        {
            throw new ArgumentNullException(nameof(game));
        }

        var refreshTimer = game.GetNode<Timer>("RefreshTimer");
        RefreshWaitTime = refreshTimer.WaitTime;
        refreshTimer.Stop();
        SceneTreeTimer waitTimer = game.GetTree().CreateTimer(StopRefreshTime);
        waitTimer.Connect("timeout", this, nameof(RestartRefreshTimer));
        // FIXME: also have to nullify all currently selected-for-refresh hexagons and their timers
        Game = game;
    }

    private void RestartRefreshTimer()
    {
        var refreshTimer = Game.GetNode<Timer>("RefreshTimer");
        refreshTimer.Start(RefreshWaitTime);
    }
}
