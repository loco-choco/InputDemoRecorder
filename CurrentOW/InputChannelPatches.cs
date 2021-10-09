using System.Reflection;
using HarmonyLib;
using UnityEngine;
namespace InputDemoRecorder
{
    public class InputChannelPatches
    {
        static readonly MethodInfo axisValueGetter = AccessTools.PropertyGetter(typeof(AbstractCommands), "AxisValue");
        static readonly MethodInfo axisValueSetter = AccessTools.PropertySetter(typeof(AbstractCommands), "AxisValue");

        static readonly MethodInfo consumedGetter = AccessTools.PropertyGetter(typeof(AbstractCommands), "Consumed");

        private static bool ChangeInputs = false;

        public delegate void InputData(InputConsts.InputCommandType commandType, ref Vector2 value);
        private static InputData InputChanger;

        public delegate void NewlyButtonData(InputConsts.InputCommandType commandType, ref bool value, bool consumed);
        public delegate void PressedButtonData(InputConsts.InputCommandType commandType, ref bool value, float minPressDuration);
        private static PressedButtonData IsPressedChanger;
        private static NewlyButtonData IsNewlyPressedChanger;
        private static NewlyButtonData IsNewlyReleasedChanger;

        public delegate void UpdateInputs();
        public static event UpdateInputs OnUpdateInputs;

        static public void DoPatches(Harmony harmonyInstance)
        {
            HarmonyMethod abstractCommandsUpdatePostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.SetInputValuePostfix));
            HarmonyMethod inputManagerUpdatePrefix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.UpdateInputsPrefix));

            HarmonyMethod isPressedPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.ReturnIsPressed));
            HarmonyMethod isNewlyPressedPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.ReturnIsNewlyPressed));
            HarmonyMethod isNewlyReleasedPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.ReturnIsNewlyReleased));

            harmonyInstance.Patch(typeof(AbstractCommands).GetMethod("Update"), postfix: abstractCommandsUpdatePostfix);
            harmonyInstance.Patch(typeof(InputManager).GetMethod(nameof(InputManager.Update)), prefix: inputManagerUpdatePrefix);

            harmonyInstance.Patch(typeof(InputManager).GetMethod(nameof(InputManager.IsPressed)), postfix: isPressedPostfix);
            harmonyInstance.Patch(typeof(InputManager).GetMethod(nameof(InputManager.IsNewlyPressed)), postfix: isNewlyPressedPostfix);
            harmonyInstance.Patch(typeof(InputManager).GetMethod(nameof(InputManager.IsNewlyReleased)), postfix: isNewlyReleasedPostfix);
        }

        static void UpdateInputsPrefix()
        {
            OnUpdateInputs?.Invoke();
        }
        static void SetInputValuePostfix(AbstractCommands __instance)
        {
            Vector2 axisValue = (Vector2)axisValueGetter.Invoke(__instance, null);
            if (ChangeInputs)
                InputChanger?.Invoke(__instance.CommandType, ref axisValue);

            axisValueSetter.Invoke(__instance, new object[] { axisValue });
        }

        static void ReturnIsPressed(AbstractCommands __instance, ref bool __result, float minPressDuration)
        {
            if (ChangeInputs)
                IsPressedChanger?.Invoke(__instance.CommandType, ref __result, minPressDuration);
        }
        static void ReturnIsNewlyPressed(AbstractCommands __instance, ref bool __result)
        {
            if (ChangeInputs)
                IsNewlyPressedChanger?.Invoke(__instance.CommandType, ref __result, (bool)consumedGetter.Invoke(__instance, null));
        }
        static void ReturnIsNewlyReleased(AbstractCommands __instance, ref bool __result)
        {
            if (ChangeInputs)
                IsNewlyReleasedChanger?.Invoke(__instance.CommandType, ref __result, (bool)consumedGetter.Invoke(__instance, null));
        }

        public static void SetInputChanger(InputData inputChanger, PressedButtonData isPressedChanger = null, NewlyButtonData isNewlyPressedChanger = null, NewlyButtonData isNewlyReleasedChanger = null)
        {
            InputChanger = inputChanger;
            IsPressedChanger = isPressedChanger;
            IsNewlyPressedChanger = isNewlyPressedChanger;
            IsNewlyReleasedChanger = isNewlyReleasedChanger;
        }
        public static void AllowChangeInputs(bool changeInputs = true)
        {
            ChangeInputs = changeInputs;
        }
        public static void ResetInputChannelEdited()
        {
            ChangeInputs = false;
            InputChanger = null;
            IsPressedChanger = null;
            IsNewlyPressedChanger = null;
            IsNewlyReleasedChanger = null;
        }
    }
}

