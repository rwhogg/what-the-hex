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

public static class ControllerMappings
{
    private const string N64_REPLICA_MAPPING_WINDOWS = "03000000632500007505000000000000,N64 Replica,a:b1,b:b2,start:b12,leftshoulder:b4,rightshoulder:b5,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a1,righttrigger:b6,platform:Windows";
    private const string SN30_PRO_MAPPING_WINDOWS = "03000000c82d00000121000000000000,8BitDo SN30 Pro for Android,a:b0,b:b1,y:b4,x:b3,start:b11,back:b10,leftstick:b13,rightstick:b14,leftshoulder:b6,rightshoulder:b7,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a2,rightx:a5,righty:a5,lefttrigger:b8,righttrigger:b9,platform:Windows";
    private const string WIRED_FIGHT_PAD_MAPPING_WINDOWS = "030000006f0e00008501000000000000,Wired Fight Pad Pro for Nintendo Switch,a:b2,b:b1,y:b0,x:b3,start:b9,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,platform:Windows";
    private const string RETROBIT_NES_MAPPING_WINDOWS = "03000000790000001100000000000000,Retro Controller,a:b2,b:b1,start:b9,back:b8,dpup:-a4,dpleft:-a3,dpdown:+a4,dpright:+a3,platform:Windows";

    private const string SN30_PRO_MAPPING_ANDROID = "38426974446f20534e33302050726f20,8BitDo SN30 Pro for Android,a:b0,b:b1,y:b3,x:b2,start:b6,back:b4,leftstick:b7,rightstick:b8,leftshoulder:b9,rightshoulder:b10,dpright:b11,dpup:b12,dpleft:b14,dpdown:b13,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:a5,righttrigger:a4,platform:Android";


    public static void SetUpMappings()
    {
        string osName = OS.GetName();
        if(osName == "Windows")
        {
            Input.AddJoyMapping(SN30_PRO_MAPPING_WINDOWS, true);
            Input.AddJoyMapping(WIRED_FIGHT_PAD_MAPPING_WINDOWS, true);
            Input.AddJoyMapping(N64_REPLICA_MAPPING_WINDOWS, true);
            Input.AddJoyMapping(RETROBIT_NES_MAPPING_WINDOWS, true);
        }
        else if(osName == "Android")
        {
            Input.AddJoyMapping(SN30_PRO_MAPPING_ANDROID, true);
        }
    }
}
