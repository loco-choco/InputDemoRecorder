using System.Collections;
using UnityEngine;

namespace InputDemoRecorder
{
    public class InputDemoPlayer : MonoBehaviour
    {
        private int currentUpdateFrame = 0;

        private void Awake()
        {
            InputChannelPatches.AllowChangeInputs();
            foreach (string channelName in InputChannelPatches.InputChannelsEdited.Keys)
            {
                InputChannelPatches.InputChannelsEdited[channelName].GetAxis = () => ReturnAxisValue(channelName);
                InputChannelPatches.InputChannelsEdited[channelName].GetAxisRaw = () => ReturnRawAxisValue(channelName);

                InputChannelPatches.InputChannelsEdited[channelName].GetButton = () => ReturnButtonValue(channelName);
                InputChannelPatches.InputChannelsEdited[channelName].GetButtonDown = () => ReturnButtonDownValue(channelName);
                InputChannelPatches.InputChannelsEdited[channelName].GetButtonUp = () => ReturnButtonUpValue(channelName);
            }

            StartCoroutine("InputUpdate");
        }
        private float ReturnAxisValue(string channelName)
        {
            if (InputDemoRecorder.framesInputs.Count > 0)
                return InputDemoRecorder.framesInputs.Peek().GetAxisInput(channelName).Axis;
            return 0f;
        }
        private float ReturnRawAxisValue(string channelName)
        {
            if (InputDemoRecorder.framesInputs.Count > 0)
                return InputDemoRecorder.framesInputs.Peek().GetAxisInput(channelName).AxisRaw;
            return 0f;
        }

        private bool ReturnButtonValue(string channelName)
        {
            if (InputDemoRecorder.framesInputs.Count > 0)
                return InputDemoRecorder.framesInputs.Peek().GetButtonInput(channelName).Button;
            return false;
        }
        private bool ReturnButtonDownValue(string channelName)
        {
            if (InputDemoRecorder.framesInputs.Count > 0)
                return InputDemoRecorder.framesInputs.Peek().GetButtonInput(channelName).ButtonDown;
            return false;
        }
        private bool ReturnButtonUpValue(string channelName)
        {
            if (InputDemoRecorder.framesInputs.Count > 0)
                return InputDemoRecorder.framesInputs.Peek().GetButtonInput(channelName).ButtonUp;
            return false;
        }

        private IEnumerator InputUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                //Give the inputs in currentUpdateFrame
                if (Time.timeScale > 0f)
                {
                    if (InputDemoRecorder.framesInputs.Count > 0 && currentUpdateFrame > 0)
                        InputDemoRecorder.framesInputs.Dequeue();
                    currentUpdateFrame++;
                }
            }
        }
        private void OnGUI()
        {
            GUI.Box(new Rect(0, 0, 200, 20), "Update Frame: " + currentUpdateFrame);
            GUI.Box(new Rect(0, 20, 200, 20), "Playing back...");
        }
    }
}
