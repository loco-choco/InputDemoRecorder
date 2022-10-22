using System;
using System.IO;
using UnityEngine;

namespace InputDemoRecorder
{
    public class InputDemoRecorderUI
    {

        public string demoFile { get; private set; } = "OuterWilds_Demo" + DemoFileLoader.DEMO_FILE_EXTENSION;
        public bool playOnlyOnSceneLoad { get; private set; } = false;
        public bool recordOnSceneLoad { get; private set; } = false;

        public bool isRecording { get; private set; } = false;
        public bool isPlayingback { get; private set; } = false;
        public bool isPaused { get; private set; } = false;

        public int seed { get; private set; } = (int)DateTime.Now.Ticks;

        public InputsCurveRecorder recordingToPlay { get; private set; } = InputsCurveRecorder.empty;
        public InputsCurveRecorder latestRecording { get; private set; } = InputsCurveRecorder.empty;

        public void PlayDemo(InputsCurveRecorder recording)
        {
            if (!recording.IsEmpty())
            {
                InputDemoPlayer.StartPlayback(recording);
                InputChannelPatches.AllowChangeInputs(true);
                isPlayingback = true;
            }
        }

        public void TogglePlayOnSceneLoad() => playOnlyOnSceneLoad = !playOnlyOnSceneLoad;
        public void ToggleRecordOnSceneLoad() => recordOnSceneLoad = !recordOnSceneLoad;
        public void RecordDemo(int seed) 
        {
            InputDemoRecorder.StartRecording(seed);
            InputChannelPatches.AllowChangeInputs(true);
            isRecording = true;
        }
        public void StopRecordingDemo()
        {
            latestRecording = InputDemoRecorder.StopRecording();
            InputChannelPatches.ResetInputChannelEdited();
            isRecording = false;
        }
        public void PlayRecordedDemo() 
        {
            recordingToPlay = latestRecording;
            if (!playOnlyOnSceneLoad)
                PlayDemo(latestRecording);
        }
        public void StopDemoPlayback()
        {
            InputDemoPlayer.StopPlayback();
            InputChannelPatches.ResetInputChannelEdited();
            isPlayingback = false;
        }
        public void RestartDemoPlayback()
        {
            InputDemoPlayer.RestartPlayback();
            isPaused = false;
        }
        public void TogglePauseDemoPlayback()
        {
            InputDemoPlayer.PausePlayback(isPaused);
            isPaused = !isPaused;
        }

        public void PlayTASOrDemoFile()
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

        public bool SaveRecordedDemo() 
        {
            return DemoFileLoader.SaveDemoFile(Path.Combine(InputDemoRecorderModStart.DllExecutablePath, demoFile), latestRecording);
        }

        public Rect windowRect = new Rect(0, 0, 240, 180);
        public void OnGUI() 
        {
            GUI.Window(0, windowRect, PlayerUI, "Input Demo Recorder");
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

            string seedString = GUI.TextField(new Rect(0, 20, 240, 20), seed.ToString());

            int.TryParse(seedString, out int newSeed);
            seed = newSeed;

            if (GUI.Button(new Rect(0, 40, 240, 20), "Record (F3)"))
                RecordDemo(seed);


            if (GUI.Button(new Rect(0, 60, 240, 20), "Play Recorded Demo (F4)"))
                PlayRecordedDemo();

            if (GUI.Button(new Rect(0, 80, 240, 20), "Save Demo (F5)"))
                SaveRecordedDemo();


            demoFile = GUI.TextField(new Rect(0, 100, 240, 20), demoFile);

            if (GUI.Button(new Rect(0, 120, 240, 20), "Play Saved Demo/TAS (F6)"))
                PlayTASOrDemoFile();

            playOnlyOnSceneLoad = GUI.Toggle(new Rect(0, 140, 240, 20), playOnlyOnSceneLoad, "Play Only On Scene Load (F7)");

            recordOnSceneLoad = GUI.Toggle(new Rect(0, 160, 240, 20), recordOnSceneLoad, "Start Recording On Scene Load (F8)");

            GUI.DragWindow();
        }

        
        private void RecordingGUI()
        {
            if (GUI.Button(new Rect(0, 20, 240, 20), "Stop Recording (F3)"))
                StopRecordingDemo();

            GUI.Label(new Rect(0, 80, 240, 20), $"Seed: {InputDemoPlayer.GetSeed()}");
            GUI.Label(new Rect(0, 40, 240, 20), $"Frame: {InputDemoRecorder.GetCurrentInputFrame()}");
        }

        private void PlayingbackGUI()
        {
            if (GUI.Button(new Rect(0, 20, 240, 20), "Stop Playingback (F3)"))
                StopDemoPlayback();

            if (GUI.Button(new Rect(0, 40, 240, 20), "Restart Playback (F4)"))
                RestartDemoPlayback();

            if (GUI.Button(new Rect(0, 60, 240, 20), (isPaused ? "Resume" : "Pause") + " Playback (F5)"))
                TogglePauseDemoPlayback();

            GUI.Label(new Rect(0, 80, 240, 20), $"Seed: {InputDemoPlayer.GetSeed()}");
            GUI.Label(new Rect(0, 100, 240, 20), $"Frame: {InputDemoPlayer.GetCurrentInputFrame()} / {InputDemoPlayer.GetLastInputFrame()}");
        }
    }
}
