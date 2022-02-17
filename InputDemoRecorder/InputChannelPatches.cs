using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using OWML.Common;
using UnityEngine;
using HarmonyLib;

namespace InputDemoRecorder
{
    public class InputChannelPatches
    {
        private static bool ChangeInputs = false;

        public delegate void InputData(AbstractCommands __instance, ref Vector2 value);
        private static InputData InputChanger;

        public delegate void UpdateInputs();
        public static event UpdateInputs OnUpdateInputs;

        static public void DoPatches(IHarmonyHelper harmonyInstance, IModConsole console)
        {
            harmonyInstance.AddPrefix(typeof(InputManager).GetMethod(nameof(InputManager.Update)), typeof(InputChannelPatches), nameof(InputChannelPatches.UpdateInputsPrefix));
            harmonyInstance.Transpile(typeof(AbstractCommands).GetMethod("Update"), typeof(InputChannelPatches), nameof(InputChannelPatches.AbstractCommandsUpdateTranspiler));
        }

        static void UpdateInputsPrefix()
        {
            OnUpdateInputs?.Invoke();
        }
        public static void SetInputValue(AbstractCommands __instance)
        {
            if (ChangeInputs)
            {
                Vector2 axisValue = __instance.AxisValue;
                InputChanger?.Invoke(__instance, ref axisValue);
                __instance.AxisValue = axisValue;

                float comparer = (__instance.ValueType == InputConsts.InputValueType.DOUBLE_AXIS) ? float.Epsilon : __instance.PressedThreshold;
                __instance.IsActiveThisFrame = axisValue.magnitude > comparer;
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

        static IEnumerable<CodeInstruction> AbstractCommandsUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int index = -1;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_0 && codes[i + 1].opcode == OpCodes.Call && codes[i + 2].opcode == OpCodes.Ldarg_0 && codes[i + 3].opcode == OpCodes.Callvirt)
                {
                    index = i + 4;
                    break;
                }
            }
            if (index > -1)
            {
                codes.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
                var method = AccessTools.Method(typeof(InputChannelPatches), nameof(InputChannelPatches.SetInputValue), new Type[] { typeof(AbstractCommands) });
                codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, method));
            }
            return codes;
        }
    }
}

