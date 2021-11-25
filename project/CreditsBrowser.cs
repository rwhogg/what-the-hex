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
 * Script for the Credits Browser
 */
public class CreditsBrowser: TextEdit
{
    /**
     * Displays the third party licenses file contents in the "editor".
     * Runs when this component enters the scene tree.
     */
    public override void _Ready()
    {
        var file = new File();
        _ = file.Open(ResourcePaths.THIRD_PARTY_LICENSES, File.ModeFlags.Read);
        Text = file.GetAsText();
        file.Dispose();
    }
}
