using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace InputDemoRecorder
{
    //TODO descobir pq está dando erro de falta de memoria
    class TASFileLoader
    {
        public const string TAS_FILE_EXTENSION = ".txt";

        public static bool LoadTASFile(string fileName, out InputsCurveRecorder loadedDemoFile)
        {
            string[] linesOfTheFile = File.ReadAllLines(fileName);
            loadedDemoFile = InputsCurveRecorder.empty;
            for (int i = 0; i < linesOfTheFile.Length; i++)
            {
                string trimmedString = linesOfTheFile[i].Trim();
                if(trimmedString.Length <= 0)
                {
                }
                else if(trimmedString[0] != '#')
                {
                    //InputDemoRecorderModStart.Instance.ModHelper.Console.WriteLine(trimmedString);
                    string[] dataInTheLine = trimmedString.Split(new string[] { " ", "- " }, StringSplitOptions.RemoveEmptyEntries);

                    if (dataInTheLine[0].ToLower() == "seed")
                    {
                        int seedValue = int.Parse(dataInTheLine[1], System.Globalization.NumberStyles.Integer);
                        loadedDemoFile.Seed = seedValue;
                    }
                    else
                    {
                        float begginingTime = float.Parse(dataInTheLine[0], System.Globalization.NumberStyles.Float);
                        //InputDemoRecorderModStart.Instance.ModHelper.Console.WriteLine(begginingTime.ToString());

                        float endTime = float.Parse(dataInTheLine[1], System.Globalization.NumberStyles.Float);
                        //InputDemoRecorderModStart.Instance.ModHelper.Console.WriteLine(endTime.ToString());

                        InputConsts.InputCommandType inputCommandToChange = StringToInputCommandType[dataInTheLine[2]];
                        //InputDemoRecorderModStart.Instance.ModHelper.Console.WriteLine(inputCommandToChange.ToString());

                        Vector2 AxisValueToSet;

                        //InputDemoRecorderModStart.Instance.ModHelper.Console.WriteLine(dataInTheLine[3]);
                        if (float.TryParse(dataInTheLine[3], out float firstValue))
                        {
                            AxisValueToSet = new Vector2(firstValue, float.Parse(dataInTheLine[4], System.Globalization.NumberStyles.Float));
                        }
                        else if (!StringToAxisValue.TryGetValue(dataInTheLine[3], out AxisValueToSet))
                        {
                            InputDemoRecorderModStart.Instance.ModHelper.Console.WriteLine(string.Format("Invalid value keywork in line {0}", i));
                            return false;
                        }
                        //InputDemoRecorderModStart.Instance.ModHelper.Console.WriteLine(AxisValueToSet.ToString());
                        loadedDemoFile.AddLineValue(inputCommandToChange, begginingTime, endTime, AxisValueToSet);
                    }
                }
            }
            return true;
        }
        public static readonly Dictionary<string, Vector2> StringToAxisValue = new Dictionary<string, Vector2>
        {
            { "foward" , Vector2.up},
            { "up" , Vector2.up},
            { "backwards" , Vector2.down},
            { "down" , Vector2.down},
            { "left" , Vector2.left},
            { "right" , Vector2.right},
        };

        public static readonly Dictionary<string, InputConsts.InputCommandType> StringToInputCommandType = new Dictionary<string, InputConsts.InputCommandType>
        {
            { "move" , InputConsts.InputCommandType.MOVE_XZ },
            { "right" , InputConsts.InputCommandType.RIGHT },
            { "left" , InputConsts.InputCommandType.LEFT },
            { "up" , InputConsts.InputCommandType.UP },
            { "down" , InputConsts.InputCommandType.DOWN },
            { "jump" , InputConsts.InputCommandType.JUMP },

            //{ "look" , InputConsts.InputCommandType.LOOK },
            //{ "lookX" , InputConsts.InputCommandType.LOOK_X },
            //{ "lookY" , InputConsts.InputCommandType.LOOK_Y },

            { "rollMode" , InputConsts.InputCommandType.ROLL_MODE },
            { "pitch" , InputConsts.InputCommandType.LOOK_X },
            { "yaw" , InputConsts.InputCommandType.LOOK_Y },

            { "autopilot" , InputConsts.InputCommandType.AUTOPILOT },
            { "freelook" , InputConsts.InputCommandType.FREELOOK },
            { "landingCamera" , InputConsts.InputCommandType.LANDING_CAMERA },

            { "matchVeloc" , InputConsts.InputCommandType.MATCH_VELOCITY },
            { "lockon" , InputConsts.InputCommandType.LOCKON },

            { "boost" , InputConsts.InputCommandType.BOOST },
            { "cancel" , InputConsts.InputCommandType.CANCEL },
            { "confirm" , InputConsts.InputCommandType.CONFIRM },
            
            { "enter" , InputConsts.InputCommandType.ENTER },
            { "esc" , InputConsts.InputCommandType.ESCAPE },
            { "stick" , InputConsts.InputCommandType.THRUST_UP },
            { "flashlight" , InputConsts.InputCommandType.FLASHLIGHT },
            
            { "interact" , InputConsts.InputCommandType.INTERACT },
            { "interactSecond" , InputConsts.InputCommandType.INTERACT_SECONDARY },

            { "map" , InputConsts.InputCommandType.MAP },
            { "mapMouse0" , InputConsts.InputCommandType.MAP_MOUSE0 },
            { "mapMouse1" , InputConsts.InputCommandType.MAP_MOUSE1 },
            { "mapZoom" , InputConsts.InputCommandType.MAP_ZOOM },
            { "mapZoomIn" , InputConsts.InputCommandType.MAP_ZOOM_IN },
            { "mapZoomOut" , InputConsts.InputCommandType.MAP_ZOOM_OUT },

            { "markEntry" , InputConsts.InputCommandType.MARK_ENTRY_ON_HUD },

            { "pause" , InputConsts.InputCommandType.PAUSE },
            { "menuConfirm" , InputConsts.InputCommandType.MENU_CONFIRM },
            { "menuLeft" , InputConsts.InputCommandType.MENU_LEFT },
            { "menuRight" , InputConsts.InputCommandType.MENU_RIGHT },
                        
            { "scrollText" , InputConsts.InputCommandType.SCROLLING_TEXT },

            { "select" , InputConsts.InputCommandType.SELECT },
            
            { "sleep" , InputConsts.InputCommandType.SLEEP },

            { "thrustX" , InputConsts.InputCommandType.MOVE_X },
            { "thrustZ" , InputConsts.InputCommandType.MOVE_Z },
            { "thrustDown" , InputConsts.InputCommandType.THRUST_DOWN },
            { "thrustUp" , InputConsts.InputCommandType.THRUST_UP },

            { "probeLaunch" , InputConsts.InputCommandType.TOOL_PRIMARY },
            { "probeRetrieve" , InputConsts.InputCommandType.PROBERETRIEVE },
            { "takeScreenshot" , InputConsts.InputCommandType.TAKE_SCREENSHOT },
            { "take2xScreenshot" , InputConsts.InputCommandType.TAKE_2XSCREENSHOT },

            { "signalscope" , InputConsts.InputCommandType.SIGNALSCOPE },
            //{ "scopeView" , InputConsts.InputCommandType.SCOPEVIEW },

            { "toolDown" , InputConsts.InputCommandType.TOOL_DOWN },
            { "toolLeft" , InputConsts.InputCommandType.TOOL_LEFT },
            { "toolPrim" , InputConsts.InputCommandType.TOOL_PRIMARY },
            { "toolSecond" , InputConsts.InputCommandType.TOOL_SECONDARY },
            { "toolRight" , InputConsts.InputCommandType.TOOL_RIGHT },
            { "toolUp" , InputConsts.InputCommandType.TOOL_UP },
            { "toolX" , InputConsts.InputCommandType.TOOL_X },
            { "toolY" , InputConsts.InputCommandType.TOOL_Y },
        };
    }
}
