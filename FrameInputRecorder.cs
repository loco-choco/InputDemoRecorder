using System.Collections.Generic;
namespace InputDemoRecorder
{
    public struct FrameInputRecorder
    {
        public int FrameOfInput;
        private Dictionary<string, AxisInputRecorder> AxisInputChannels;
        private Dictionary<string, ButtonInputRecorder> ButtonInputChannels;

        public FrameInputRecorder(int frameOfInput)
        {
            FrameOfInput = frameOfInput;
            AxisInputChannels = new Dictionary<string, AxisInputRecorder>();
            ButtonInputChannels = new Dictionary<string, ButtonInputRecorder>();
        }
        public void AddAxisInput(string inputChannelName, AxisInputRecorder axisInput )
        {
            AxisInputChannels.Add(inputChannelName, axisInput);
        }
        public void AddButtonInput(string inputChannelName, ButtonInputRecorder buttonInput)
        {
            ButtonInputChannels.Add(inputChannelName, buttonInput);
        }
        public AxisInputRecorder GetAxisInput(string inputChannelName)
        {
            if (AxisInputChannels.TryGetValue(inputChannelName, out var axis))
                return axis;
            return new AxisInputRecorder();
        }
        public ButtonInputRecorder GetButtonInput(string inputChannelName)
        {
            if (ButtonInputChannels.TryGetValue(inputChannelName, out var button))
                return button;
            return new ButtonInputRecorder();
        }
    }
}
