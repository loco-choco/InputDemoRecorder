using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputDemoRecorder
{
    public class InputChannelPatches
    {
        static readonly MethodInfo axisValueGetter = AccessTools.PropertyGetter(typeof(AbstractCommands), "AxisValue");
        static readonly MethodInfo axisValueSetter = AccessTools.PropertySetter(typeof(AbstractCommands), "AxisValue");

        static readonly MethodInfo isActiveThisFrameSetter = AccessTools.PropertySetter(typeof(AbstractCommands), "IsActiveThisFrame");

        static readonly MethodInfo wasActiveLastFrameGetter = AccessTools.PropertyGetter(typeof(AbstractCommands), "WasActiveLastFrame");

        static readonly MethodInfo inputStartedTimeSetter = AccessTools.PropertySetter(typeof(AbstractCommands), "InputStartedTime");

        private static bool ChangeInputs = false;

        public delegate void InputData(InputConsts.InputCommandType commandType, ref Vector2 value);
        private static InputData InputChanger;

        public delegate void UpdateInputs();
        public static event UpdateInputs OnUpdateInputs;

        static public void DoPatches(Harmony harmonyInstance)
        {
            HarmonyMethod inputManagerUpdatePrefix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.UpdateInputsPrefix));
            HarmonyMethod abstractCommandsUpdatePostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.SetInputValuePostfix));

            harmonyInstance.Patch(typeof(InputManager).GetMethod(nameof(InputManager.Update)), prefix: inputManagerUpdatePrefix);
            harmonyInstance.Patch(typeof(AbstractCommands).GetMethod("Update"), postfix: abstractCommandsUpdatePostfix);
        }

        static void UpdateInputsPrefix()
        {
            OnUpdateInputs?.Invoke();
        }
        public static void SetInputValuePostfix(AbstractCommands __instance)
        {
            if (ChangeInputs)
            {
                Vector2 axisValue = (Vector2)axisValueGetter.Invoke(__instance, null);
                InputChanger?.Invoke(__instance.CommandType, ref axisValue);
                axisValueSetter.Invoke(__instance, new object[] { axisValue });

                float comparer = (__instance.ValueType == InputConsts.InputValueType.DOUBLE_AXIS) ? float.Epsilon : __instance.PressedThreshold;
                bool isActiveThisFrame = axisValue.magnitude > comparer;
                isActiveThisFrameSetter.Invoke(__instance, new object[] { isActiveThisFrame });

                bool wasActiveLastFrame = (bool)wasActiveLastFrameGetter.Invoke(__instance, null);
                if (!isActiveThisFrame)
                {
                    if (wasActiveLastFrame)
                    {
                        inputStartedTimeSetter.Invoke(__instance, new object[] { float.MaxValue });
                    }
                }
                else if (!wasActiveLastFrame)
                {
                    inputStartedTimeSetter.Invoke(__instance, new object[] { Time.realtimeSinceStartup });
                }
            }
        }
        public static void SetInputChanger(InputData inputChanger)
        {
            InputChanger = inputChanger;
        }
        public static void AllowChangeInputs(bool changeInputs = true)
        {
            ChangeInputs = changeInputs;
        }
        public static void ResetInputChannelEdited()
        {
            ChangeInputs = false;
            InputChanger = null;
        }
    }
}

