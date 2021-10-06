using UnityEngine;

namespace InputDemoRecorder
{
    public static class InputDemoPlayer
    {
        private static float currentInputTime;
        private static float startPlaybackTime;
        static InputsCurveRecorder InputsCurve;

        public static float GetCurrentInputTime() => currentInputTime;

        static InputDemoPlayer()
        {
            InputChannelPatches.OnUpdateInputs += InputChannelPatches_OnUpdateInputs;
        }
        public static void StartPlayback(InputsCurveRecorder demoFile)
        {
            InputChannelPatches.ResetInputChannelEdited();
            InputChannelPatches.SetInputChanger(ReturnInputCommandValue);

            startPlaybackTime = Time.time;
        }
        public static void StopPlayback()
        {
            InputChannelPatches.AllowChangeInputs(false);
        }

        private static void InputChannelPatches_OnUpdateInputs()
        {
            currentInputTime = Time.time - startPlaybackTime;
        }

        private static void ReturnInputCommandValue(InputConsts.InputCommandType commandType, ref Vector2 axisValue)
        {
            if (InputsCurve.InputCurves.TryGetValue(commandType, out var curves))
                axisValue = new Vector2(curves[0].Evaluate(currentInputTime), curves[1].Evaluate(currentInputTime));
        }
        
        //private static void OnGUI()
        //{
        //    if (!play)
        //        return;
        //    GUI.Box(new Rect(0, 0, 200, 20), "Current Frame Time: " + currentInputTime);
        //    GUI.Box(new Rect(0, 20, 200, 20), "Playing back...");
        //}
    }
}
