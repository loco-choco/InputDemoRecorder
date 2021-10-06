using System.Collections.Generic;
using HarmonyLib;

namespace InputDemoRecorder
{
    public class InputChannelEdited
    {
        public delegate float AxisData();
        public delegate bool ButtonData();

        public AxisData GetAxis = ()=> 0f;
        public AxisData GetAxisRaw = () => 0f;
        public ButtonData GetButton = () => false;
        public ButtonData GetButtonDown = () => false;
        public ButtonData GetButtonUp = () => false;
    }
    public class InputChannelPatches
    {
        static readonly AccessTools.FieldRef<InputChannel, string> keyBinding = AccessTools.FieldRefAccess<InputChannel, string>("_keyBinding");

        private static bool ChangeInputs = false;

        static public void DoPatches(Harmony harmonyInstance)
        {
            HarmonyMethod axisPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.GetAxisPostfix));
            HarmonyMethod axisRawPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.GetAxisRawPostfix));
            HarmonyMethod buttonPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.GetButtonPostfix));
            HarmonyMethod buttonDownPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.GetButtonDownPostfix));
            HarmonyMethod buttonUpPostfix = new HarmonyMethod(typeof(InputChannelPatches), nameof(InputChannelPatches.GetButtonUpPostfix));

            harmonyInstance.Patch(typeof(InputChannel).GetMethod(nameof(InputChannel.GetAxis)), postfix: axisPostfix);
            harmonyInstance.Patch(typeof(InputChannel).GetMethod(nameof(InputChannel.GetAxisRaw)), postfix: axisRawPostfix);
            harmonyInstance.Patch(typeof(InputChannel).GetMethod(nameof(InputChannel.GetButton)), postfix: buttonPostfix);
            harmonyInstance.Patch(typeof(InputChannel).GetMethod(nameof(InputChannel.GetButtonDown)), postfix: buttonDownPostfix);
            harmonyInstance.Patch(typeof(InputChannel).GetMethod(nameof(InputChannel.GetButtonUp)), postfix: buttonUpPostfix);
        }

        static void GetAxisPostfix(InputChannel __instance, ref float __result)
        {
            if (InputChannelsEdited.TryGetValue(keyBinding(__instance), out InputChannelEdited input) && ChangeInputs)
                __result = input.GetAxis();
        }
        static void GetAxisRawPostfix(InputChannel __instance, ref float __result)
        {
            if (InputChannelsEdited.TryGetValue(keyBinding(__instance), out InputChannelEdited input) && ChangeInputs)
                __result = input.GetAxisRaw();
        }
        static void GetButtonPostfix(InputChannel __instance, ref bool __result)
        {
            if (InputChannelsEdited.TryGetValue(keyBinding(__instance), out InputChannelEdited input) && ChangeInputs)
                __result = input.GetButton();
        }
        static void GetButtonDownPostfix(InputChannel __instance, ref bool __result)
        {
            if (InputChannelsEdited.TryGetValue(keyBinding(__instance), out InputChannelEdited input) && ChangeInputs)
                __result = input.GetButtonDown();
        }
        static void GetButtonUpPostfix(InputChannel __instance, ref bool __result)
        {
            if (InputChannelsEdited.TryGetValue(keyBinding(__instance), out InputChannelEdited input) && ChangeInputs)
                __result = input.GetButtonUp();
        }
        
        public static void AllowChangeInputs(bool changeInputs = true)
        {
            ChangeInputs = changeInputs;
        }
        public static void ResetInputChannelEdited()
        {
            ChangeInputs = false;
            foreach (var pair in InputChannelsEdited.Keys)
                InputChannelsEdited[pair] = new InputChannelEdited();
        }

        public static Dictionary<string, InputChannelEdited> InputChannelsEdited = new Dictionary<string, InputChannelEdited>
        {
            { "Move X_Key",new InputChannelEdited() },
            { "Move Z_Key",new InputChannelEdited() },
            { "Move Up_Key",new InputChannelEdited() },
            { "Move Down_Key",new InputChannelEdited() },
            { "Pitch_Key",new InputChannelEdited() },
            { "Yaw_Key",new InputChannelEdited() },
            { "Zoom In_Key",new InputChannelEdited() },
            { "Zoom Out_Key",new InputChannelEdited() },
            { "Interact_Key",new InputChannelEdited() },
            { "Cancel_Key",new InputChannelEdited() },
            { "Jump_Key",new InputChannelEdited() },
            { "Flashlight_Key",new InputChannelEdited() },
            { "Telescope_Key",new InputChannelEdited() },
            { "Lock On_Key",new InputChannelEdited() },
            { "Probe_Key",new InputChannelEdited() },
            { "Alt Probe_Key",new InputChannelEdited() },
            { "Match Velocity_Key",new InputChannelEdited() },
            { "Autopilot_Key",new InputChannelEdited() },
            { "Landing Camera_Key",new InputChannelEdited() },
            { "Swap Roll/Yaw_Key",new InputChannelEdited() },
            { "Map_Key",new InputChannelEdited() },
            //{ "Pause_Key",new InputChannelEdited() },
        };
    }
}
