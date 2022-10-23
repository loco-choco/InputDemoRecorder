using HarmonyLib;
using UnityEngine;
using OWML.ModHelper;
using OWML.Common;

namespace InputDemoRecorder
{
    class InputDemoRecorderModStart : ModBehaviour
    {
        public static InputDemoRecorderModStart Instance;
        private static string gamePath;

        public static string DllExecutablePath
        {
            get
            {
                if (string.IsNullOrEmpty(gamePath))
                    gamePath = Instance.ModHelper.Manifest.ModFolderPath;
                return gamePath;
            }
            private set { }
        }

        InputDemoRecorderUI ui;

        public void Start()
        {
            Instance = this;
            new Harmony("locochoco.inputDemoRecorder.com").PatchAll();

            ui = new InputDemoRecorderUI();
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (ui.applySeedOnSceneLoad)
                    Random.InitState(ui.seed);

                if (ui.playOnlyOnSceneLoad)
                    ui.PlayDemo(ui.recordingToPlay);
                else if (ui.recordOnSceneLoad)
                    ui.RecordDemo(ui.seed);
            };
        }

        public void OnGUI()
        {
            ui.OnGUI();
        }
        public void Update()
        {
            if (ui.isRecording)
            {
                RecordingInputUpdate();
                return;
            }
            else if (ui.isPlayingback)
            {
                PlaybackInputUpdate();
                return;
            }
            MainMenuInputUpdate();
        }
        public void MainMenuInputUpdate() 
        {
            if (DebugKeyCode.GetKeyUp(KeyCode.F3))
                ui.RecordDemo(ui.seed);

            if (DebugKeyCode.GetKeyDown(KeyCode.F4))
                ui.PlayRecordedDemo();

            if (DebugKeyCode.GetKeyDown(KeyCode.F5))
                ui.SaveRecordedDemo();

            if (DebugKeyCode.GetKeyDown(KeyCode.F6))
                ui.PlayTASOrDemoFile();

            if (DebugKeyCode.GetKeyDown(KeyCode.F7))
                ui.TogglePlayOnSceneLoad();

            if (DebugKeyCode.GetKeyDown(KeyCode.F8))
                ui.ToggleRecordOnSceneLoad();

            if (DebugKeyCode.GetKeyDown(KeyCode.F9))
                ui.TogglApplySeedOnSceneLoad();
        }
        public void RecordingInputUpdate()
        {
            if (DebugKeyCode.GetKeyUp(KeyCode.F3))
                ui.StopRecordingDemo();
        }
        public void PlaybackInputUpdate()
        {
            if (DebugKeyCode.GetKeyUp(KeyCode.F3))
                ui.StopDemoPlayback();

            if (DebugKeyCode.GetKeyDown(KeyCode.F4))
                ui.RestartDemoPlayback();

            if (DebugKeyCode.GetKeyDown(KeyCode.F5)) 
                ui.TogglePauseDemoPlayback();
        }

        public override void Configure(IModConfig config)
        {
        }
    }
}
