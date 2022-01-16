using System.IO;
using System.Reflection;
using OWML.ModHelper;
using OWML.Common;

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
            ModHelper.Console.WriteLine("Starting...");

            ModHelper.Console.WriteLine("BBBB");
            InputChannelPatches.DoPatches(ModHelper.HarmonyHelper, ModHelper.Console);

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (ui.playOnlyOnSceneLoad)
                    ui.PlayDemo(ui.recordingToPlay);
            };

            ui = new InputDemoRecorderUI();
        }
        public override void Configure(IModConfig config)
        {
        }
    }
}
