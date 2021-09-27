using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InputDemoRecorder
{
    class InputDemoRecorder : MonoBehaviour
    {
        private int currentUpdateFrame = 0;
        public static Queue<FrameInputRecorder> framesInputs = new Queue<FrameInputRecorder > ();

        private void Awake()
        {
            InputChannelPatches.AllowChangeInputs(false);
            framesInputs.Clear();

            StartCoroutine("InputUpdate");
        }
        private IEnumerator InputUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                //Give the inputs in currentUpdateFrame
                if (Time.timeScale > 0f)
                {
                    FrameInputRecorder inputRecorder = new FrameInputRecorder(currentUpdateFrame);
                    for (int i = 0; i < Channels.Length; i++)
                    {
                        if (Channels[i].inputType == InputType.Axis)
                            inputRecorder.AddAxisInput(Channels[i].channelName, new AxisInputRecorder(Channels[i].channel));
                        else
                            inputRecorder.AddButtonInput(Channels[i].channelName, new ButtonInputRecorder(Channels[i].channel));
                    }
                    framesInputs.Enqueue(inputRecorder);

                    currentUpdateFrame++;
                }
            }
        }
        private void OnGUI()
        {
            GUI.Box(new Rect(0, 0, 200, 20), "Update Frame: " + currentUpdateFrame);
            GUI.Box(new Rect(0, 20, 200, 20), "Recording...");
        }
        private readonly InputToRecord[] Channels =
        {
            new InputToRecord("Move X_Key",InputChannels.moveX ,InputType.Axis),
            new InputToRecord("Move Z_Key",InputChannels.moveZ, InputType.Axis),
            new InputToRecord("Move Up_Key",InputChannels.moveUp, InputType.Axis),
            new InputToRecord("Move Down_Key",InputChannels.moveDown, InputType.Axis),

            new InputToRecord("Pitch_Key",InputChannels.pitch, InputType.Axis),
            new InputToRecord("Yaw_Key",InputChannels.yaw, InputType.Axis),

            new InputToRecord("Zoom In_Key",InputChannels.zoomIn, InputType.Axis),
            new InputToRecord("Zoom Out_Key",InputChannels.zoomOut, InputType.Axis),

            new InputToRecord("Interact_Key",InputChannels.interact, InputType.Button),
            new InputToRecord("Cancel_Key",InputChannels.cancel, InputType.Button),

            new InputToRecord("Jump_Key",InputChannels.jump, InputType.Button),

            new InputToRecord("Flashlight_Key",InputChannels.flashlight, InputType.Axis),
            new InputToRecord("Telescope_Key",InputChannels.telescope, InputType.Button),

            new InputToRecord("Lock On_Key",InputChannels.lockOn, InputType.Button),
            new InputToRecord("Probe_Key",InputChannels.probe, InputType.Button),
            new InputToRecord("Alt Probe_Key",InputChannels.altProbe, InputType.Button),

            new InputToRecord("Match Velocity_Key",InputChannels.matchVelocity, InputType.Button),
            new InputToRecord("Autopilot_Key",InputChannels.autopilot, InputType.Button),
            new InputToRecord("Landing Camera_Key",InputChannels.landingCam, InputType.Button),
            new InputToRecord("Swap Roll/Yaw_Key",InputChannels.swapRollAndYaw, InputType.Button),

            new InputToRecord("Map_Key",InputChannels.map, InputType.Button),
            new InputToRecord("Pause_Key",InputChannels.pause, InputType.Button)
        };
        enum InputType
        {
            Axis,
            Button
        }
        struct InputToRecord
        {
            public string channelName;
            public InputChannel channel;
            public InputType inputType;

            public InputToRecord(string channelName, InputChannel channel, InputType inputType)
            {
                this.channelName = channelName;
                this.channel = channel;
                this.inputType = inputType;
            }
        }
    }
}
