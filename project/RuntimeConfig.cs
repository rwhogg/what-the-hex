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

/**
 * Globals that we can use to configure the game scene
 */
public class RuntimeConfig : Node
{
    /**
     * True if we are in 2-player mode
     */
    public static bool Is2Player { get; set; }

    /**
     * Lists the number of hexagons in each row
     * (the number of columns is determined by the length)
     */
    public static int[] HexesPerRow { get; set; }

    public static int MatchesNeeded { get; set; }

    public static void DefaultLayout()
    {
        RuntimeConfig.HexesPerRow = new int[5];
        for(int i = 0; i < 5; i++)
        {
            RuntimeConfig.HexesPerRow[i] = 8;
        }
    }
}
