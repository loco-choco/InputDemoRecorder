using System.Collections.Generic;
using System.IO;

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


        public byte[] GetDictionaryOrdersInBytes()
        {
            var stream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            binaryWriter.Write(AxisInputChannels.Count);
            foreach (var pair in AxisInputChannels)
                binaryWriter.Write(pair.Key);

            binaryWriter.Write(ButtonInputChannels.Count);
            foreach (var pair in ButtonInputChannels)
                binaryWriter.Write(pair.Key);

            binaryWriter.Close();
            return stream.ToArray();
        }
        public byte[] FrameInputRecorderInBytes()
        {
            var stream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            
            foreach (var pair in AxisInputChannels)
            {
                binaryWriter.Write(pair.Value.Axis);
                binaryWriter.Write(pair.Value.AxisRaw);
            }
            foreach (var pair in ButtonInputChannels)
            {
                binaryWriter.Write(pair.Value.Button);
                binaryWriter.Write(pair.Value.ButtonDown);
                binaryWriter.Write(pair.Value.ButtonUp);
            }
            binaryWriter.Close();
            return stream.ToArray();
        }
    }
}
