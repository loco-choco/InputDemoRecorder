using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
namespace InputDemoRecorder
{
    public class InputChannelPatches
    {
        static readonly AccessTools.FieldRef<AbstractCommands, Vector2> axisValue = AccessTools.FieldRefAccess<AbstractCommands, Vector2>("AxisValue");

        private static bool ChangeInputs = false;

        public delegate Vector2 InputData();
        private static InputData InputChanger;

        static public void DoPatches(Harmony harmonyInstance)
        {
            HarmonyMethod abstractCommandsUpdatePostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.SetInputValuePostfix));
            harmonyInstance.Patch(typeof(AbstractCommands).GetMethod("Update"), postfix: abstractCommandsUpdatePostfix);
        }

        static void SetInputValuePostfix(AbstractCommands __instance)
        {
            if (InputChanger != null && ChangeInputs)
                axisValue(__instance) = InputChanger.Invoke();
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

