using HarmonyLib;
using BepInEx;

namespace InputDemoRecorder
{
    [BepInDependency("locochoco.plugins.CAMOWA",BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("locochoco.plugins.InputDemoRecorder", "OWA Input Demo Recorder", "1.0.0.0")]
    [BepInProcess("OuterWilds_Alpha_1_2.exe")]
    class InputDemoRecorderStart : BaseUnityPlugin
    {
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

                record = !record;
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
