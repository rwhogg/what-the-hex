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

/**
 * Win condition that is based on making a certain number of matches
 */
public class NumMatchesWinCondition: IWinCondition
{
    private readonly int MatchesToWin;

    /**
     * Constructor
     * @param matches The number of matches it takes to win
     */
    public NumMatchesWinCondition(int matches)
    {
        MatchesToWin = matches;
    }

    /**
     * Check if we have met this win condition
     * @param game The game component
     */
    public bool HasWon(GameComponent game)
    {
        if(game == null)
        {
            throw new ArgumentNullException(nameof(game));
        }
        return game.NumMatchesMade >= MatchesToWin;
    }
}
