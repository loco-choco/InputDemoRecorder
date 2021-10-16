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
    class InputDemoRecorderStart : BaseUnityPlugin
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

        public void Awake()
        {
            var harmonyInstance = new Harmony("locochoco.InputDemoRecorder");
            InputChannelPatches.DoPatches(harmonyInstance);
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }
        string demoFile = "OuterWilds_Demo" + DemoFileLoader.DEMO_FILE_EXTENSION;
        InputsCurveRecorder latestRecording;

        Rect windowPositionAndScale = new Rect(200, 0, 240, 140);
        private void OnGUI()
        {
            windowPositionAndScale = GUI.Window(0, windowPositionAndScale, PlayerUI, string.Format("{0} - v{1}", MOD_NAME, MOD_VERSION));
        }
        private void PlayerUI(int id)
        {
            if (isRecording)
            {
                RecordingGUI();
                return;
            }
            else if (isPlayingback)
            {
                PlayingbackGUI();
                return;
            }

            if (GUI.Button(new Rect(0, 20, 240, 20), "Record"))
            {
                InputDemoRecorder.StartRecording();
                InputChannelPatches.AllowChangeInputs(true);
                isRecording = true;
            }

            if (GUI.Button(new Rect(0, 40, 240, 20), "Play Recorded Demo"))
            {
                recordingToPlay = latestRecording;
                if (!playOnlyOnSceneLoad)
                    PlayDemo(latestRecording);
            }

            if (GUI.Button(new Rect(0, 60, 240, 20), "Save Demo"))
            {
                if (DemoFileLoader.SaveDemoFile(Path.Combine(DllExecutablePath, demoFile), latestRecording))
                    Debug.Log(demoFile + " was saved");
            }

            demoFile = GUI.TextField(new Rect(0, 80, 240, 20), demoFile);

            if (GUI.Button(new Rect(0, 100, 240, 20), "Play Saved Demo/TAS"))
            {
                string extension = Path.GetExtension(demoFile);

                InputsCurveRecorder loadedDemo = InputsCurveRecorder.empty;
                bool canBePlayed = false;

                if (extension == DemoFileLoader.DEMO_FILE_EXTENSION)
                    canBePlayed = DemoFileLoader.LoadDemoFile(Path.Combine(DllExecutablePath, demoFile), out loadedDemo);
                else if (extension == TASFileLoader.TAS_FILE_EXTENSION)
                    canBePlayed = TASFileLoader.LoadTASFile(Path.Combine(DllExecutablePath, demoFile), out loadedDemo);

                if (canBePlayed)
                {
                    recordingToPlay = loadedDemo;
                    if (!playOnlyOnSceneLoad)
                        PlayDemo(loadedDemo);
                }

            }

            playOnlyOnSceneLoad = GUI.Toggle(new Rect(0, 120, 240, 20), playOnlyOnSceneLoad, "Play Only On Scene Load");

            GUI.DragWindow();
        }

        bool playOnlyOnSceneLoad = false;
        InputsCurveRecorder recordingToPlay;
        public void PlayDemo(InputsCurveRecorder recording)
        {
            if (!recording.IsEmpty())
            {
                InputDemoPlayer.StartPlayback(recording);
                InputChannelPatches.AllowChangeInputs(true);
                isPlayingback = true;
            }
        }
        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            if (playOnlyOnSceneLoad)
                PlayDemo(recordingToPlay);
        }

        bool isRecording = false;
        private void RecordingGUI()
        {
            if (GUI.Button(new Rect(0, 20, 240, 20), "Stop Recording"))
            {
                latestRecording = InputDemoRecorder.StopRecording();
                InputChannelPatches.ResetInputChannelEdited();
                isRecording = false;
            }
            GUI.Label(new Rect(0, 40, 240, 20), "Time: " + InputDemoRecorder.GetCurrentInputTime());
        }

        bool isPlayingback = false;
        bool isPaused = false;
        private void PlayingbackGUI()
        {
            if (GUI.Button(new Rect(0, 20, 240, 20), "Stop Playingback"))
            {
                InputDemoPlayer.StopPlayback();
                InputChannelPatches.ResetInputChannelEdited();
                isPlayingback = false;
            }
            if (GUI.Button(new Rect(0, 40, 240, 20), "Restart Playback"))
            {
                InputDemoPlayer.RestartPlayback();
                isPaused = false;
            }

            if (GUI.Button(new Rect(0, 60, 240, 20), (isPaused ? "Resume" : "Pause") + " Playback"))
            {
                InputDemoPlayer.PausePlayback(isPaused);
                isPaused = !isPaused;
            }
            GUI.Label(new Rect(0, 80, 240, 20), "Time: " + InputDemoPlayer.GetCurrentInputTime());
        }
    }
}
