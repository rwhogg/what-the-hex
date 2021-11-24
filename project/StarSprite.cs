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
 * Sprite for the stars that appear on the title screen
 */
public class StarSprite : AnimatedSprite
{
    private static readonly SpriteFrames StarFrames = new SpriteFrames();

    private static readonly ImageTexture StarTexture = new ImageTexture();

    /**
     * Called when this node enters the scene tree
     */
    public override void _Ready()
    {
        base._Ready();

        if(StarTexture.GetData() == null)
        {
            Image starParticle = GD.Load<StreamTexture>("res://images/smallStar.png").GetData();
            StarTexture.CreateFromImage(starParticle);
            StarFrames.AddFrame("default", StarTexture);
        }

        Frames = StarFrames;
        Play("default");
    }
}
