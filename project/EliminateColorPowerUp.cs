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
            GD.Print("Color Eliminated: " + colorToEliminate);
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
