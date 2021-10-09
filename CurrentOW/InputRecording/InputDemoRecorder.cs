using System.Collections.Generic;
using UnityEngine;

namespace InputDemoRecorder
{
    public static class InputDemoRecorder
    {
        private static float currentInputTime;
        private static float startRecordingTime;
        private static InputsCurveRecorder recordedInputsCurve;

        public static float GetCurrentInputTime() => currentInputTime;

        static InputDemoRecorder()
        {
            InputChannelPatches.OnUpdateInputs += InputChannelPatches_OnUpdateInputs;
        }

        private static void InputChannelPatches_OnUpdateInputs()
        {
            currentInputTime = Time.time - startRecordingTime;
        }
        public static void StartRecording()
        {
            recordedInputsCurve = new InputsCurveRecorder() { InputCurves = new Dictionary<InputConsts.InputCommandType, AnimationCurve[]>() };
            
            InputChannelPatches.SetInputChanger(RecordInputCommandValue);

            startRecordingTime = Time.time;
        }

        public static InputsCurveRecorder StopRecording()
        {
            InputChannelPatches.ResetInputChannelEdited();
            return recordedInputsCurve;
        }

        private static void RecordInputCommandValue(InputConsts.InputCommandType commandType, ref Vector2 axisValue)
        {
            //if(ChannelsToIgnore.Contains(commandType))
                recordedInputsCurve.AddValue(commandType, currentInputTime, axisValue.x, axisValue.y);
        }
        private static readonly HashSet<InputConsts.InputCommandType> ChannelsToIgnore = new HashSet<InputConsts.InputCommandType>()
        {
            InputConsts.InputCommandType.MOVE_X,
            InputConsts.InputCommandType.MOVE_Z,
            InputConsts.InputCommandType.UP,
            InputConsts.InputCommandType.DOWN,
             InputConsts.InputCommandType.UP2,
            InputConsts.InputCommandType.DOWN2,
        };
    }
}
