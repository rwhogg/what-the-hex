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
 * Script for the About box
 */
public class AboutInfoBox : TextEdit
{
    /**
     * Called when this object and its children enter the scene tree.
     * Replaces the version number in the about box automatically.
     */
    public override void _Ready()
    {
        // Note: if this file complains about GitVersionInformation not existing,
        // run "dotnet restore" followed by "dotnet build"
        Text = Text.Replace("_vers_", GitVersionInformation.FullSemVer)
            .Replace("_godotversion_", GodotVersionInfo.GetGodotInfo());
    }
}
