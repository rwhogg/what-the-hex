using Godot;

public class ControllerMappings
{
    private readonly static string N64_REPLICA_MAPPING_WINDOWS = "03000000632500007505000000000000,N64 Replica,a:b1,b:b2,start:b12,leftshoulder:b4,rightshoulder:b5,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a1,righttrigger:b6,platform:Windows";
    private readonly static string SN30_PRO_MAPPING_WINDOWS = "03000000c82d00000121000000000000,8BitDo SN30 Pro for Android,a:b0,b:b1,y:b4,x:b3,start:b11,back:b10,leftstick:b13,rightstick:b14,leftshoulder:b6,rightshoulder:b7,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a2,rightx:a5,righty:a5,lefttrigger:b8,righttrigger:b9,platform:Windows";
    private readonly static string WIRED_FIGHT_PAD_MAPPING_WINDOWS = "030000006f0e00008501000000000000,Wired Fight Pad Pro for Nintendo Switch,a:b2,b:b1,y:b0,x:b3,start:b9,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:b12,dpleft:b14,dpdown:b13,dpright:b15,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,platform:Windows";

    private readonly static string SN30_PRO_MAPPING_ANDROID = "38426974446f20534e33302050726f20,8BitDo SN30 Pro for Android,a:b0,b:b1,y:b3,x:b2,start:b6,back:b4,leftstick:b7,rightstick:b8,leftshoulder:b9,rightshoulder:b10,dpright:b11,dpup:b12,dpleft:b14,dpdown:b13,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:a5,righttrigger:a4,platform:Android";


    public static void SetUpMappings()
    {
        string osName = OS.GetName();
        if(osName == "Windows")
        {
            Input.AddJoyMapping(SN30_PRO_MAPPING_WINDOWS, true);
            Input.AddJoyMapping(WIRED_FIGHT_PAD_MAPPING_WINDOWS, true);
            Input.AddJoyMapping(N64_REPLICA_MAPPING_WINDOWS, true);
        }
        else if(osName == "Android")
        {
            Input.AddJoyMapping(SN30_PRO_MAPPING_ANDROID, true);
        }
    }
}
