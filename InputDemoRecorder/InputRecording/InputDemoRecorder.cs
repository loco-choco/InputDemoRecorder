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
            currentInputTime = Time.unscaledTime - startRecordingTime;
        }
        public static void StartRecording()
        {
            recordedInputsCurve = InputsCurveRecorder.empty;

            InputChannelPatches.SetInputChanger(RecordInputCommandValue);

            startRecordingTime = Time.unscaledTime;
        }

        public static InputsCurveRecorder StopRecording()
        {
            InputChannelPatches.ResetInputChannelEdited();
            return recordedInputsCurve;
        }

        private static void RecordInputCommandValue(InputConsts.InputCommandType commandType, ref Vector2 axisValue)
        {
            if (ChannelsToRecord.Contains(commandType))
                recordedInputsCurve.AddValue(commandType, currentInputTime, axisValue.x, axisValue.y);
        }
        private static readonly HashSet<InputConsts.InputCommandType> ChannelsToRecord = new HashSet<InputConsts.InputCommandType>()
        {
    InputConsts.InputCommandType.SELECT,

    InputConsts.InputCommandType.MENU_CONFIRM,

    InputConsts.InputCommandType.ENTER,

    InputConsts.InputCommandType.ENTER2,

    InputConsts.InputCommandType.CANCEL,

    InputConsts.InputCommandType.CANCEL_REBINDING1,

    InputConsts.InputCommandType.CANCEL_REBINDING2,

    InputConsts.InputCommandType.ESCAPE,

    InputConsts.InputCommandType.SET_DEFAULTS,

    InputConsts.InputCommandType.CONFIRM,

    InputConsts.InputCommandType.CONFIRM2,

    InputConsts.InputCommandType.UP,

    InputConsts.InputCommandType.DOWN,

    InputConsts.InputCommandType.RIGHT,

    InputConsts.InputCommandType.LEFT,

    InputConsts.InputCommandType.MENU_RIGHT,

    InputConsts.InputCommandType.MENU_LEFT,

    InputConsts.InputCommandType.SUBMENU_RIGHT,

    InputConsts.InputCommandType.SUBMENU_LEFT,

    InputConsts.InputCommandType.UP2,

    InputConsts.InputCommandType.DOWN2,

    InputConsts.InputCommandType.RIGHT2,

    InputConsts.InputCommandType.LEFT2,

    InputConsts.InputCommandType.TAB,

    InputConsts.InputCommandType.TABL,

    InputConsts.InputCommandType.TABR,

    InputConsts.InputCommandType.TABL2,

    InputConsts.InputCommandType.TABR2,

    InputConsts.InputCommandType.SHIFTL,

    InputConsts.InputCommandType.SHIFTR,

    InputConsts.InputCommandType.PAUSE,

    InputConsts.InputCommandType.INTERACT_SECONDARY,

    InputConsts.InputCommandType.JUMP,

    InputConsts.InputCommandType.LOOK_Y,

    InputConsts.InputCommandType.MAP,

    InputConsts.InputCommandType.THRUST_UP,

    InputConsts.InputCommandType.THRUST_DOWN,

    InputConsts.InputCommandType.MOVE_XZ,

    InputConsts.InputCommandType.LOOK,

    InputConsts.InputCommandType.JUMP,

    InputConsts.InputCommandType.INTERACT,

    InputConsts.InputCommandType.INTERACT_SECONDARY,

    InputConsts.InputCommandType.FLASHLIGHT,

    InputConsts.InputCommandType.THRUST_UP,

    InputConsts.InputCommandType.TOOL_PRIMARY,

    InputConsts.InputCommandType.TOOL_SECONDARY,

    InputConsts.InputCommandType.TOOL_X,

    InputConsts.InputCommandType.TOOL_Y,

    InputConsts.InputCommandType.TOOL_RIGHT,

    InputConsts.InputCommandType.TOOL_LEFT,

    InputConsts.InputCommandType.TOOL_UP,

    InputConsts.InputCommandType.TOOL_DOWN,

    InputConsts.InputCommandType.SIGNALSCOPE,

    InputConsts.InputCommandType.TOOL_PRIMARY,

    InputConsts.InputCommandType.PROBERETRIEVE,

    InputConsts.InputCommandType.ROLL_MODE,

    InputConsts.InputCommandType.BOOST,

    InputConsts.InputCommandType.MOVE_X,

    InputConsts.InputCommandType.MOVE_Z,

    InputConsts.InputCommandType.THRUST_UP,

    InputConsts.InputCommandType.THRUST_DOWN,

    InputConsts.InputCommandType.LOOK_X,

    InputConsts.InputCommandType.LOOK_Y,

    InputConsts.InputCommandType.LOCKON,

    InputConsts.InputCommandType.MATCH_VELOCITY,

    InputConsts.InputCommandType.LANDING_CAMERA,

    InputConsts.InputCommandType.AUTOPILOT,

    InputConsts.InputCommandType.FREELOOK,

    InputConsts.InputCommandType.TAKE_SCREENSHOT,

    InputConsts.InputCommandType.TAKE_2XSCREENSHOT

    };
    }
}
