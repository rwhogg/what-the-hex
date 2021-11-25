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



/**
 * Script for the Credits button on the menu scene.
 */
public class CreditsButton : ThemeButton
{
    /**
     * Changes the scene to the credits scene.
     * Called when the button is pressed.
     */
    protected override void ChangeScene()
    {
        GetTree().ChangeScene(ResourcePaths.CREDITS_SCENE);
    }
}
