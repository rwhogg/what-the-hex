

using Godot;

public class QuitButton : ThemeButton
{
    /**
     * Switches back to the menu scene.
     * Called when the button is pressed.
     */
    protected override void ChangeScene()
    {
        GetTree().Paused = false;
        RuntimeConfig.NukePopup();
        GetTree().ChangeScene("res://Menu.tscn");
    }
}
