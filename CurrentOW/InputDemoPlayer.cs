using UnityEngine;
using System.Collections.Generic;

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
            InputChannelPatches.SetInputChanger(ReturnInputCommandValue, 
                ReturnIsPressedCommandValue, ReturnIsNewlyPressedCommandValue, ReturnIsNewlyReleasedCommandValue);

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

        private static Dictionary<InputConsts.InputCommandType, InputActionData> inputsActionData = new Dictionary<InputConsts.InputCommandType, InputActionData>();
        private static void ReturnInputCommandValue(InputConsts.InputCommandType commandType, ref Vector2 axisValue)
        {
            if (InputsCurve.InputCurves.TryGetValue(commandType, out var curves))
            {
                axisValue = new Vector2(curves[0].Evaluate(currentInputTime), curves[1].Evaluate(currentInputTime));

                if (!inputsActionData.ContainsKey(commandType))
                    inputsActionData.Add(commandType, new InputActionData());

                inputsActionData[commandType].WasActiveLastFrame = inputsActionData[commandType].IsActiveThisFrame;
                inputsActionData[commandType].IsActiveThisFrame = axisValue != Vector2.zero;

                if (axisValue != Vector2.zero && inputsActionData[commandType].InputStartedTime == float.MaxValue)
                    inputsActionData[commandType].InputStartedTime = currentInputTime;
                else if (axisValue == Vector2.zero && inputsActionData[commandType].InputStartedTime != float.MaxValue)
                    inputsActionData[commandType].InputStartedTime = float.MaxValue;
            }
        }

        private static void ReturnIsPressedCommandValue(InputConsts.InputCommandType commandType, ref bool value, float minPressDuration)
        {
            if (inputsActionData.TryGetValue(commandType, out var actionData))
            {
                if (actionData.IsActiveThisFrame)
                    value = startPlaybackTime - actionData.InputStartedTime >= minPressDuration;
                else
                    value = false;
            }
        }
        private static void ReturnIsNewlyPressedCommandValue(InputConsts.InputCommandType commandType, ref bool value, bool consumed)
        {
            if (inputsActionData.TryGetValue(commandType, out var actionData))
            {
                if (consumed)
                    value = false;
                else if (!actionData.WasActiveLastFrame)
                    value = actionData.IsActiveThisFrame;
                else
                    value = false;
            }
        }
        private static void ReturnIsNewlyReleasedCommandValue(InputConsts.InputCommandType commandType, ref bool value, bool consumed)
        {
            if (inputsActionData.TryGetValue(commandType, out var actionData))
            {
                if (actionData.WasActiveLastFrame)
                    value = !actionData.IsActiveThisFrame;
                else
                    value = false;
            }
        }
    }

    public class InputActionData
    {
        public bool IsActiveThisFrame;
        public bool WasActiveLastFrame;
        public float InputStartedTime;
    }
}
