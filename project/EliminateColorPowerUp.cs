using System;

using Godot;

public class EliminateColorPowerUp : Godot.Object, IPowerUp
{
    public static readonly ImageTexture Texture = new ImageTexture();

    private GameComponent Game;

    public EliminateColorPowerUp()
    {
        if(Texture.GetData() == null)
        {
            Image clock = GD.Load<StreamTexture>("res://paint_bucket.png").GetData();
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

        
    }
}
