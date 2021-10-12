using System.IO;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using UnityEngine;

namespace InputDemoRecorder
{
    [BepInPlugin("locochoco.plugins.InputDemoRecorder", "OW Input Demo Recorder", "1.0.0.0")]
    [BepInProcess("OuterWilds.exe")]
    class InputDemoRecorderStart : BaseUnityPlugin
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

        public void Awake()
        {
            var harmonyInstance = new Harmony("locochoco.InputDemoRecorder");
            InputChannelPatches.DoPatches(harmonyInstance);
        }

        string demoName = "OuterWilds_Demo";
        private void OnGUI()
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

            if (GUI.Button(new Rect(200, 0, 100, 20), "Record"))
            {
                InputDemoRecorder.StartRecording();
                InputChannelPatches.AllowChangeInputs(true);
                isRecording = true;
            }

            if (GUI.Button(new Rect(200, 20, 200, 20), "Play Recorded Demo"))
            {
                InputDemoPlayer.StartPlayback(latestRecording);
                InputChannelPatches.AllowChangeInputs(true);
                isPlayingback = true;
            }

            if (GUI.Button(new Rect(200, 40, 100, 20), "Save Demo"))
            {
                if (DemoFileLoader.SaveDemoFile(DllExecutablePath, demoName, latestRecording))
                    Debug.Log(demoName + DemoFileLoader.DEMO_FILE_EXTENSION + " was saved");
            }

            demoName = GUI.TextField(new Rect(200, 60, 200, 20), demoName);

            if (GUI.Button(new Rect(200, 80, 200, 20), "Play Saved Demo"))
            {
                DemoFileLoader.LoadDemoFile(Path.Combine(DllExecutablePath, demoName + DemoFileLoader.DEMO_FILE_EXTENSION), out var loadedDemo);

                InputDemoPlayer.StartPlayback(loadedDemo);
                InputChannelPatches.AllowChangeInputs(true);
                isPlayingback = true;
            }
        }

        InputsCurveRecorder latestRecording;
        bool isRecording = false;
        private void RecordingGUI()
        {
            if (GUI.Button(new Rect(200, 0, 100, 20), "Stop Recording"))
            {
                latestRecording = InputDemoRecorder.StopRecording();
                InputChannelPatches.ResetInputChannelEdited();

                isRecording = false;
            }
            GUI.Label(new Rect(200, 20, 100, 20), "Time: " + InputDemoRecorder.GetCurrentInputTime());
        }

        bool isPlayingback = false;
        private void PlayingbackGUI()
        {
            if (GUI.Button(new Rect(200, 0, 100, 20), "Stop Playingback"))
            {
                InputDemoPlayer.StopPlayback();
                InputChannelPatches.ResetInputChannelEdited();

                isPlayingback = false;
            }
            GUI.Label(new Rect(200, 20, 100, 20), "Time: " + InputDemoPlayer.GetCurrentInputTime());
        }
    }   
}
