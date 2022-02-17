using System.IO;
using System.Reflection;
using UnityEngine;
using OWML.ModHelper;
using OWML.Common;

namespace InputDemoRecorder
{
    class InputDemoRecorderModStart : ModBehaviour
    {
        private static InputDemoRecorderModStart instance;
        private static string gamePath;

        public static string DllExecutablePath
        {
            get
            {
                if (string.IsNullOrEmpty(gamePath))
                    gamePath = instance.ModHelper.Manifest.ModFolderPath;
                return gamePath;
            }
            private set { }
        }

        InputDemoRecorderUI ui;

        public void Start()
        {
            instance = this;
            InputChannelPatches.DoPatches(ModHelper.HarmonyHelper, ModHelper.Console);

            ui = new InputDemoRecorderUI();
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (ui.playOnlyOnSceneLoad)
                    ui.PlayDemo(ui.recordingToPlay);
                else if (ui.recordOnSceneLoad)
                    ui.RecordDemo();
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
                ui.RecordDemo();

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
