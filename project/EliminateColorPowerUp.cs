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

public class EliminateColorPowerUp: Godot.Object, IPowerUp
{
    public static readonly ImageTexture Texture = new ImageTexture();

    public EliminateColorPowerUp()
    {
        if(Texture.GetData() == null)
        {
            Image clock = GD.Load<StreamTexture>(ResourcePaths.PAINT_BUCKET_IMAGE).GetData();
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

        Color colorToEliminate = HexagonUtils.RandomColor();
        if(OS.IsDebugBuild())
        {
            GD.Print("Color Eliminated: " + Utils.ColorMap(colorToEliminate));
        }
        Color[] allOtherColors = new Color[Hexagon.EdgeColorOptions.Length - 1];
        int j = 0;
        for(int i = 0; i < Hexagon.EdgeColorOptions.Length; i++)
        {
            if(Hexagon.EdgeColorOptions[i] != colorToEliminate)
            {
                allOtherColors[j] = Hexagon.EdgeColorOptions[i];
                j++;
            }
        }


        foreach(Hexagon[] hexRow in game.HexagonGrid.HexArray)
        {
            foreach(Hexagon hex in hexRow)
            {
                for(int i = 0; i < hex.EdgeColors.Count; i++)
                {
                    Color edgeColor = hex.EdgeColors[i];
                    if(edgeColor == colorToEliminate)
                    {
                        hex.EdgeColors[i] = HexagonUtils.RandomColor(allOtherColors);
                    }
                }
            }
        }
    }
}
