using System.IO;
using System.Reflection;
using OWML.ModHelper;
using OWML.Common;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace InputDemoRecorder
{
    class InputDemoRecorderModStart : ModBehaviour
    {
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

        public void Start()
        {
            InputChannelPatches.DoPatches(ModHelper.HarmonyHelper);

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (ui.playOnlyOnSceneLoad)
                ui.PlayDemo(ui.recordingToPlay);
            };

            ui = new InputDemoRecorderUI();
        }
    }
}
