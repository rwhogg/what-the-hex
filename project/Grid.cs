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

using static Godot.JoystickList;

/**
 * Wrapper class for a grid of hexagons
 */
public class Grid : BaseGrid
{
    // FIXME: the hexagon and the grid classes are so tightly intertwined, and also they
    // probably have game logic embedded in them. It would be better if I could
    // sort out what should be where, and maybe also move some of the logic into signals
    // emitted by the hexagons and grid as appropriate, with the game logic moved to the game
    // component in signal handlers

    /**
     * Do not use. Only for constructing Mono temp objects
     */
    public Grid() : base() { }

    /**
     * Starts off a grid at the specified start point.
     * @param startPoint Top-left point of the grid
     * @param hexagonsPerRow Number of hexagons per row
     */
    public Grid(Vector2 startPoint, int[] hexagonsPerRow) : base(startPoint, hexagonsPerRow)
    {
    }

    /**
     * Handle input for mouse clicks and joystick buttons
     * @param event Input event
     */
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventJoypadButton eventControllerButton)
        {
            GetTree().SetInputAsHandled();
            HandleButtonPress(eventControllerButton);
        }
        else if(@event is InputEventMouseButton eventMouseButton)
        {
            HandleMouseClick(eventMouseButton);
        }
    }

    // FIXME: I wonder if the button press handling can be moved to a different class
    private void HandleButtonPress(InputEventJoypadButton eventControllerButton)
    {
        // ensure we don't double up the actions
        if(eventControllerButton.IsPressed())
        {
            return;
        }

        int controllerIndex = eventControllerButton.Device;
        if(controllerIndex != 0 && !RuntimeConfig.Is2Player)
        {
            // ensure we don't get index-out-of-bounds exceptions because the controller index maps to the player
            return;
        }

        switch((JoystickList)eventControllerButton.ButtonIndex)
        {
            case DpadUp:
            case DpadDown:
            case DpadLeft:
            case DpadRight:
                int[][] dirsToGo ={
                    new int[] { -1, 0 },
                    new int[] { 1, 0 },
                    new int[] { 0, -1 },
                    new int[] { 0, 1 },
                };
                int[] dir = dirsToGo[eventControllerButton.ButtonIndex - (int)DpadUp];
                SelectedHexagons[controllerIndex].Selected[controllerIndex] = false;
                int newI = SelectedHexagons[controllerIndex].I + dir[0];
                int numRows = HexagonsPerRow.Length;
                if(newI < 0)
                {
                    newI = 0;
                }
                else if(newI >= numRows)
                {
                    newI = numRows - 1;
                }
                int newJ = SelectedHexagons[controllerIndex].J + dir[1];
                if(newJ < 0)
                {
                    newJ = 0;
                }
                else if(newJ >= HexagonsPerRow[newI])
                {
                    newJ = HexagonsPerRow[newI] - 1;
                }
                Hexagon newlySelectedHexagon = HexArray[newI][newJ];
                newlySelectedHexagon.Selected[controllerIndex] = true;
                SelectedHexagons[controllerIndex] = newlySelectedHexagon;
                break;
            case L:
            case L2:
            case DsB: // Also XboxA, SonyX
                // Note: deliberately not supporting control stick / C stick triggers (L3 and R3 in Godot).
                // They are way too easy to use intentionally.
                HandleRotation(SelectedHexagons[controllerIndex], Direction.LEFT, controllerIndex);
                break;
            case R:
            case R2:
            case DsA: // Also XboxB, SonyCircle
                HandleRotation(SelectedHexagons[controllerIndex], Direction.RIGHT, controllerIndex);
                break;
            case DsY: // Also XboxX, SonySquare
                HandlePowerUp();
                break;
        }
    }

    private void HandleMouseClick(InputEventMouseButton eventMouseButton)
    {
        // note, we assume mouse clicks are only used by a single player for now

        // ensure we don't double-rotate from a single click and that we don't accidentally trigger from the mouse wheel
        if(eventMouseButton.IsPressed() || eventMouseButton.ButtonIndex >= (int)ButtonList.Middle)
        {
            return;
        }

        Vector2 clickPos = eventMouseButton.Position - Position; // offset because we were one diagonal hexagon down
        Hexagon affectedHexagon = GetAffectedHexagon(clickPos);
        if(affectedHexagon == null)
        {
            return;
        }

        GetTree().SetInputAsHandled();

        Direction direction = Direction.LEFT;
        if(OS.HasTouchscreenUiHint())
        {
            // on touch screen devices, a tap should be equivalent to a select, not a rotation
            SelectedHexagons[0].Selected[0] = false;
            SetSelectedHexagon(affectedHexagon.I, affectedHexagon.J, 0);
            return;
        }
        else if((int)ButtonList.Right == eventMouseButton.ButtonIndex)
        {
            direction = Direction.RIGHT;
        }
        HandleRotation(affectedHexagon, direction, 0);
    }
}
