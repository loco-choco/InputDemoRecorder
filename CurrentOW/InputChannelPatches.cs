using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
namespace InputDemoRecorder
{
    public class InputChannelPatches
    {
        static readonly AccessTools.FieldRef<AbstractCommands, Vector2> axisValue = AccessTools.FieldRefAccess<AbstractCommands, Vector2>("AxisValue");

        private static bool ChangeInputs = false;

        public delegate void InputData(InputConsts.InputCommandType commandType, ref Vector2 value);
        private static InputData InputChanger;

        public delegate void UpdateInputs();
        public static event UpdateInputs OnUpdateInputs;

        static public void DoPatches(Harmony harmonyInstance)
        {
            HarmonyMethod abstractCommandsUpdatePostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.SetInputValuePostfix));
            HarmonyMethod inputManagerUpdatePrefix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.UpdateInputsPrefix));

            harmonyInstance.Patch(typeof(AbstractCommands).GetMethod("Update"), postfix: abstractCommandsUpdatePostfix);
            harmonyInstance.Patch(typeof(InputManager).GetMethod(nameof(InputManager.Update)), prefix: inputManagerUpdatePrefix);
            
        }

        static void UpdateInputsPrefix()
        {
            OnUpdateInputs?.Invoke();
        }
        static void SetInputValuePostfix(AbstractCommands __instance)
        {
            if (ChangeInputs)
                InputChanger?.Invoke(__instance.CommandType, ref axisValue(__instance));
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

