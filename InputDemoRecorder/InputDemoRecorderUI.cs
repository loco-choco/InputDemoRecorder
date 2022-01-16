using System.IO;
using UnityEngine;

namespace InputDemoRecorder
{
    public class InputDemoRecorderUI
    {

        public string demoFile { get; private set; } = "OuterWilds_Demo" + DemoFileLoader.DEMO_FILE_EXTENSION;
        public bool playOnlyOnSceneLoad { get; private set; } = false;

        public bool isRecording { get; private set; } = false;
        public bool isPlayingback { get; private set; } = false;
        public bool isPaused { get; private set; } = false;

        public InputsCurveRecorder recordingToPlay { get; private set; }
        public InputsCurveRecorder latestRecording { get; private set; }

        public void PlayDemo(InputsCurveRecorder recording)
        {
            if (!recording.IsEmpty())
            {
                InputDemoPlayer.StartPlayback(recording);
                InputChannelPatches.AllowChangeInputs(true);
                isPlayingback = true;
            }
        }

        public void PlayerUI(int id)
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
                if (DemoFileLoader.SaveDemoFile(Path.Combine(InputDemoRecorderModStart.DllExecutablePath, demoFile), latestRecording))
                    Debug.Log(demoFile + " was saved");
            }

            demoFile = GUI.TextField(new Rect(0, 80, 240, 20), demoFile);

            if (GUI.Button(new Rect(0, 100, 240, 20), "Play Saved Demo/TAS"))
            {
                string extension = Path.GetExtension(demoFile);

                InputsCurveRecorder loadedDemo = InputsCurveRecorder.empty;
                bool canBePlayed = false;

                if (extension == DemoFileLoader.DEMO_FILE_EXTENSION)
                    canBePlayed = DemoFileLoader.LoadDemoFile(Path.Combine(InputDemoRecorderModStart.DllExecutablePath, demoFile), out loadedDemo);
                else if (extension == TASFileLoader.TAS_FILE_EXTENSION)
                    canBePlayed = TASFileLoader.LoadTASFile(Path.Combine(InputDemoRecorderModStart.DllExecutablePath, demoFile), out loadedDemo);

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
