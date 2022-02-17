using UnityEngine;
using System.Collections.Generic;

namespace InputDemoRecorder
{
    public static class InputDemoPlayer
    {
        private static int currentInputFrame = 0;
        private static bool isPaused = false;
        private static InputsCurveRecorder InputsCurve;

        public static int GetCurrentInputFrame() => currentInputFrame;
        public static float GetLastInputFrame() => InputsCurve.LastFrame();
        static InputDemoPlayer()
        {
            InputChannelPatches.OnUpdateInputs += InputChannelPatches_OnUpdateInputs;
        }
        public static void StartPlayback(InputsCurveRecorder demoFile)
        {
            InputChannelPatches.SetInputChanger(ReturnInputCommandValue);
            InputsCurve = demoFile;
            currentInputFrame = 0;
            isPaused = false;
        }
        public static void RestartPlayback(int startFrame = 0)
        {
            currentInputFrame = startFrame;
            isPaused = false;
        }
        public static void PausePlayback(bool resume = false)
        {
            isPaused = !resume;
        }
        public static void StopPlayback()
        {
            InputChannelPatches.ResetInputChannelEdited();
            isPaused = true;
        }

        private static void InputChannelPatches_OnUpdateInputs()
        {
            if (!isPaused && currentInputFrame < InputsCurve.LastFrame())
                currentInputFrame++;
        }
        
        private static void ReturnInputCommandValue(AbstractCommands command, ref Vector2 axisValue)
        {

            if (InputsCurve.InputCurves.TryGetValue(command.CommandType, out var curve) && !isPaused)
            {
                if (currentInputFrame < curve.Count)
                {
                    var input = curve[currentInputFrame];// new Vector2(curves[0].Evaluate(currentInputTime), curves[1].Evaluate(currentInputTime));

                    if (command.CommandType != InputConsts.InputCommandType.PAUSE)
                        axisValue = input;
                    else if (input.magnitude > float.Epsilon)
                        axisValue = input;
                }
            }
        }
    }
}
