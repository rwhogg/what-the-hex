using Godot;
using System;

public class ContinueButton : ThemeButton
{
    private GameComponent Game;

    public override void _Ready()
    {
        base._Ready();
        Game = GetParent<GameComponent>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        bool hasWon = Game.HasWon;
        Text = Tr(hasWon ? "PLAY_AGAIN_QUESTION" : "CONTINUE_QUESTION");
    }
}
