using System.Collections;
using UnityEngine;

namespace InputDemoRecorder
{
    public class InputDemoPlayer : MonoBehaviour
    {
        private bool stopPlaying = false;
        private int currentUpdateFrame = 0;
        FrameInputRecorder[] frameInputs;

        private void Awake()
        {
            if (DemoFileLoader.LoadedDemoFile == null)
            {
                Destroy(this);
                return;
            }

            InputChannelPatches.AllowChangeInputs();
            foreach (string channelName in InputChannelPatches.InputChannelsEdited.Keys)
            {
                InputChannelPatches.InputChannelsEdited[channelName].GetAxis = () => ReturnAxisValue(channelName);
                InputChannelPatches.InputChannelsEdited[channelName].GetAxisRaw = () => ReturnRawAxisValue(channelName);

                InputChannelPatches.InputChannelsEdited[channelName].GetButton = () => ReturnButtonValue(channelName);
                InputChannelPatches.InputChannelsEdited[channelName].GetButtonDown = () => ReturnButtonDownValue(channelName);
                InputChannelPatches.InputChannelsEdited[channelName].GetButtonUp = () => ReturnButtonUpValue(channelName);
            }
            frameInputs = DemoFileLoader.LoadedDemoFile;
            StartCoroutine("InputUpdate");
        }
        private float ReturnAxisValue(string channelName)
        {
            if (currentUpdateFrame < frameInputs.Length)
                return frameInputs[currentUpdateFrame].GetAxisInput(channelName).Axis;
            return 0f;
        }
        private float ReturnRawAxisValue(string channelName)
        {
            if (currentUpdateFrame < frameInputs.Length)
                return frameInputs[currentUpdateFrame].GetAxisInput(channelName).AxisRaw;
            return 0f;
        }

        private bool ReturnButtonValue(string channelName)
        {
            if (currentUpdateFrame < frameInputs.Length)
                return frameInputs[currentUpdateFrame].GetButtonInput(channelName).Button;
            return false;
        }
        private bool ReturnButtonDownValue(string channelName)
        {
            if (currentUpdateFrame < frameInputs.Length)
                return frameInputs[currentUpdateFrame].GetButtonInput(channelName).ButtonDown;
            return false;
        }
        private bool ReturnButtonUpValue(string channelName)
        {
            if (currentUpdateFrame < frameInputs.Length)
                return frameInputs[currentUpdateFrame].GetButtonInput(channelName).ButtonUp;
            return false;
        }

        private IEnumerator InputUpdate()
        {
            while (!stopPlaying)
            {
                yield return new WaitForFixedUpdate();
                //Give the inputs in currentUpdateFrame
                if (Time.timeScale > 0f)
                    currentUpdateFrame++;

                if (currentUpdateFrame >= frameInputs.Length)
                {
                    stopPlaying = true;
                    InputChannelPatches.AllowChangeInputs(false);
                }
            }
        }
        private void OnGUI()
        {
            if (stopPlaying)
                return;
            GUI.Box(new Rect(0, 0, 200, 20), "Update Frame: " + currentUpdateFrame);
            GUI.Box(new Rect(0, 20, 200, 20), "Playing back...");
        }
    }
}
