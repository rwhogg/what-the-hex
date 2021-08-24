
public class QuitButton : ThemeButton
{
    /**
     * Switches back to the menu scene.
     * Called when the button is pressed.
     */
    protected override void ChangeScene()
    {
        GetTree().Paused = false;
        var popup = GetTree().Root.GetNodeOrNull("PausePopupRoot");
        if(popup != null)
        {
            popup.QueueFree();
            popup = null;
        }
        GetTree().ChangeScene("res://Menu.tscn");
    }
}
