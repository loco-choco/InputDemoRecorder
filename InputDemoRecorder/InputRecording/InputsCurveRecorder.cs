using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InputDemoRecorder
{
    public struct InputsCurveRecorder
    {
        private int lastFrame;
        public int LastFrame() => lastFrame;

        public Dictionary<InputConsts.InputCommandType, List<Vector2>> InputCurves;

        public int Seed;

        public static InputsCurveRecorder empty => new InputsCurveRecorder() { InputCurves = new Dictionary<InputConsts.InputCommandType, List<Vector2>>() };
        public bool IsEmpty()
        {
            if (InputCurves == null)
                return true;
            return InputCurves.Values.Count == 0;
        }
        private void CreateKey(InputConsts.InputCommandType commandType, out List<Vector2> curve)
        {
            if (InputCurves.TryGetValue(commandType, out var curveFromKey))
                curve = curveFromKey;
            else
            {
                curve = new List<Vector2>();
                InputCurves.Add(commandType, curve);
            }
        }
        public void AddValue(InputConsts.InputCommandType commandType, Vector2 value)
        {
            CreateKey(commandType, out var curves);
            curves.Add(value);

            lastFrame = (lastFrame < curves.Count) ? curves.Count : lastFrame;
        }
        public void AddLineValue(InputConsts.InputCommandType commandType, float begginingTime, float endTime, Vector2 value)
        {
            CreateKey(commandType, out var curves);
            int initialFrame = (int) (begginingTime / Time.unscaledDeltaTime);
            int endFrame = (int) (endTime / Time.unscaledDeltaTime);

            if (initialFrame >= curves.Count - 1) //If curves isn't big enough to have initialFrame, we pad with zeros
            {
                int j = curves.Count;
                while (j < initialFrame)
                {
                    curves.Add(Vector2.zero);
                    j++;
                }
            }
            int i = initialFrame;

            while (i <= endFrame && i < curves.Count)
            {
                curves[i] = value;
                i++;
            }
            while (i < endFrame) //This means that we need to make curves bigger
            {
                curves.Add(value);
                i++;
            }
            lastFrame = (lastFrame < curves.Count) ? curves.Count : lastFrame;
        }
        public static InputsCurveRecorder InputsCurveRecorderFromBytes(BinaryReader reader)
        {
            InputsCurveRecorder inputsCurveRecorder = empty;

            inputsCurveRecorder.Seed = reader.ReadInt32();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                InputConsts.InputCommandType commandType = (InputConsts.InputCommandType)reader.ReadInt32();

                int amountOfValues = reader.ReadInt32();
                List<Vector2> values = new List<Vector2>(amountOfValues);
                for (int j = 0; j < amountOfValues; j++)
                {
                    float valueX = reader.ReadSingle();
                    float valueY = reader.ReadSingle();
                    values.Add(new Vector2(valueX, valueY));
                }
                inputsCurveRecorder.lastFrame = (inputsCurveRecorder.lastFrame < values.Count) ? values.Count : inputsCurveRecorder.lastFrame;
                inputsCurveRecorder.InputCurves.Add(commandType, values);
            }
            return inputsCurveRecorder;
        }

        public byte[] InputsCurveRecorderInBytes()
        {
            var stream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            binaryWriter.Write(Seed);

            binaryWriter.Write(InputCurves.Count);
            foreach (var pair in InputCurves)
            {
                binaryWriter.Write((int)pair.Key);
                var curve = pair.Value;
                binaryWriter.Write(curve.Count);
                foreach (var value in curve)
                {
                    binaryWriter.Write(value.x);
                    binaryWriter.Write(value.y);
                }
            }
            binaryWriter.Close();
            return stream.ToArray();
        }
    }
}
