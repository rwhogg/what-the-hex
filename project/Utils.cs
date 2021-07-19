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

using static Godot.Colors;

/**
 * Assorted utility functions it doesn't make sense to put anywhere else
 */
public static class Utils
{
    public static Font AnyFont { get; } = new Label().GetFont(string.Empty);

    private static DynamicFont RocketFont;

    public static DynamicFont GetRocketFont()
    {
        if(RocketFont == null)
        {
            RocketFont = new DynamicFont();
            RocketFont.FontData = ResourceLoader.Load<DynamicFontData>("KenneyRocket.ttf");
        }
        RocketFont.OutlineColor = Red;
        RocketFont.Size = 30;
        return RocketFont;
    }

    public static bool IsControllerMode()
    {
        return Input.GetConnectedJoypads().Count > 0;
    }

    public static string ColorMap(Color color)
    {
        if(color == Green)
        {
            return "green";
        }
        if(color == Purple)
        {
            return "purple";
        }
        if(color == HotPink)
        {
            return "pink";
        }
        if(color == Yellow)
        {
            return "yellow";
        }

        throw new System.Exception("Unknown color specified");
    }
}