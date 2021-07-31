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



using System.Globalization;

using Godot;

/**
 * Utilities for working with the configuration file
 */
public static class ConfigFileUtils
{
    // on Windows, this corresponds to ~/AppData/Roaming/Godot/app_userdata/<Game Name>/WhatTheHex.cfg
    private static string ConfigFileLocation = "user://WhatTheHex.cfg";

    private static ConfigFile LoadedConfigFile;

    /**
     * Get the currently defined culture (i.e. locale setting).
     * Defaults to en-US.
     */
    public static CultureInfo GetCulture()
    {
        var configFile = GetConfigFile();
        string cultureName = (string)configFile.GetValue("settings", "culture", "en-US");
        return new CultureInfo(cultureName);

        // Yes, I prefer en-US over en-CA. Sorry to all the other Canucks reading this.
    }

    /**
     * Loads and returns the current high score.
     * Note, the score should always be rendered as "Hiscore" or "HISCORE"
     * (in English, anyhow) for aesthetic consistency.
     * @return The value of the current high score, or 0 if there isn't one yet.
     */
    public static int LoadHiscore()
    {
        var configFile = GetConfigFile();
        return (int)configFile.GetValue("progress", "hiscore", 0);
    }

    /**
     * Saves the specified score as the new high score.
     * @param newHiscore The score to set as the new high score.
     */
    public static void SaveHiscore(int newHiscore)
    {
        var configFile = GetConfigFile();
        configFile.SetValue("progress", "hiscore", newHiscore);
        SaveConfigFile(configFile);
    }

    /**
     * Checks if we should play music (during the main game) or not.
     * Defaults to true in release mode and false in debug mode.
     * @return true if we should play music; false otherwise.
     */
    public static bool ShouldPlayMusic()
    {
        var configFile = GetConfigFile();
        var defaultValue = OS.IsDebugBuild() ? false : true;
        return (bool)configFile.GetValue("music", "enabled", defaultValue);
    }

    /**
     * Set if we should or should not play music
     * @param should True if we should play music and false if we should not
     */
    public static void SetShouldPlayMusic(bool should)
    {
        var configFile = GetConfigFile();
        configFile.SetValue("music", "enabled", should);
        SaveConfigFile(configFile);
    }

    public static void SetEdgeThickness(int thickness)
    {
        var configFile = GetConfigFile();
        configFile.SetValue("hexagons", "edge_thickness", thickness);
        SaveConfigFile(configFile);
    }

    public static int GetEdgeThickness()
    {
        var configFile = GetConfigFile();
        return (int)configFile.GetValue("hexagons", "edge_thickness", 6);
    }

    private static ConfigFile GetConfigFile()
    {
        if(LoadedConfigFile != null)
        {
            return LoadedConfigFile;
        }
        ConfigFile configFile = new ConfigFile();
        configFile.Load(ConfigFileLocation);
        LoadedConfigFile = configFile;
        return configFile;
    }

    private static void SaveConfigFile(ConfigFile configFile)
    {
        Error result = configFile.Save(ConfigFileLocation);
        if(result != Error.Ok)
        {
            GD.Print("Saving config file failed! Error is " + result);
        }
    }
}
