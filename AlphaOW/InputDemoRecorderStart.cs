using System.IO;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using UnityEngine;

namespace InputDemoRecorder
{
    [BepInDependency("locochoco.plugins.CAMOWA",BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("locochoco.plugins.AlphaInputDemoRecorder", "OWA Input Demo Recorder", "1.0.0.0")]
    [BepInProcess("OuterWilds_Alpha_1_2.exe")]
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

            CAMOWA.SceneLoading.OnSceneLoad += SceneLoading_OnSceneLoad;
        }
        bool record = true;
        private void SceneLoading_OnSceneLoad(int sceneId)
        {
            if (sceneId == 1)
            {
                if (record)
                    Locator.GetPlayerTransform().gameObject.AddComponent<InputDemoRecorder>();
                else
                    Locator.GetPlayerTransform().gameObject.AddComponent<InputDemoPlayer>();
            }
                
        }
        string demoName = "OuterWildsAlpha_Demo";
        private void OnGUI()
        {
            if (GUI.Button(new Rect(200, 0, 100, 20), "Record"))
                record = true;
            if (GUI.Button(new Rect(200, 20, 200, 20), "Play Recorded Demo"))
            {
                record = false;
                AlphaInputDemoRecorder.LoadedDemoFile = InputDemoRecorder.framesInputs.ToArray();
            }
            if (GUI.Button(new Rect(200, 40, 100, 20), "Save Demo"))
            {
                if (AlphaInputDemoRecorder.SaveDemoFile(DllExecutablePath, demoName, InputDemoRecorder.framesInputs.ToArray()))
                    Debug.Log(demoName + AlphaInputDemoRecorder.DEMO_FILE_EXTENSION + " was saved");
            }

            demoName = GUI.TextField(new Rect(200, 60, 200, 20), demoName);

            if (GUI.Button(new Rect(200, 80, 200, 20), "Play Saved Demo"))
            {
                record = false;
                AlphaInputDemoRecorder.LoadDemoFile(Path.Combine(DllExecutablePath, demoName + AlphaInputDemoRecorder.DEMO_FILE_EXTENSION));
            }
        }

        //TODO Create a way to record inputs (from QSA)
        //- Start recording when the Scene Starts
        //- Capture it in Update() (Store the update frame)
        //- Stop when it reloads
        //- Have files that contain multiple demos (?)
        //TODO Create a way to store the recorded inputs (demos)
        //TODO Create a way to read the recorded inputs into acctual inputs

        //Later:
        //TODO Create a way to edit the demo files
        //TODO 
    }
}
