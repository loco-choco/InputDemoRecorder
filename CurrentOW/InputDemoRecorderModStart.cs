using System.IO;
using System.Reflection;
using HarmonyLib;
using BepInEx;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace InputDemoRecorder
{
    [BepInPlugin("locochoco.plugins.InputDemoRecorder", MOD_NAME, MOD_VERSION)]
    [BepInProcess("OuterWilds.exe")]
    class InputDemoRecorderModStart : BaseUnityPlugin
    {
        const string MOD_VERSION = "1.0.1.0";
        const string MOD_NAME = "OW Input Demo Recorder";
        private static string gamePath;

        public static string DllExecutablePath
        {
            get
            {
                if (string.IsNullOrEmpty(gamePath))
                    gamePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return gamePath;
            }
            private set { }
        }

        InputDemoRecorderUI ui;

        public void Awake()
        {
            var harmonyInstance = new Harmony("locochoco.InputDemoRecorder");
            InputChannelPatches.DoPatches(harmonyInstance);
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

            ui = new InputDemoRecorderUI();
        }

        Rect windowPositionAndScale = new Rect(200, 0, 240, 140);
        private void OnGUI()
        {
            windowPositionAndScale = GUI.Window(0, windowPositionAndScale, ui.PlayerUI, string.Format("{0} - v{1}", MOD_NAME, MOD_VERSION));
        }
        
        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            if (ui.playOnlyOnSceneLoad)
                ui.PlayDemo(ui.recordingToPlay);
        }
    }
}
