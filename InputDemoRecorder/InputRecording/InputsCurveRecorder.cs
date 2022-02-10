using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InputDemoRecorder
{
    public struct InputsCurveRecorder
    {
        public Dictionary<InputConsts.InputCommandType, AnimationCurve[]> InputCurves;

        public static InputsCurveRecorder empty { get { return new InputsCurveRecorder() { InputCurves = new Dictionary<InputConsts.InputCommandType, AnimationCurve[]>() }; } }
        public bool IsEmpty()
        {
            if(InputCurves == null)
                return true;
            return InputCurves.Values.Count <= 0;
        }
        private void CreateKey(InputConsts.InputCommandType commandType, int size, out AnimationCurve[]  curves)
        {
            if (InputCurves.TryGetValue(commandType, out var curvesFromKey))
                curves = curvesFromKey;
            else
            {
                curves = new AnimationCurve[size];
                for (int i = 0; i < curves.Length; i++)
                    curves[i] = new AnimationCurve();
                InputCurves.Add(commandType, curves);
            }
        }
        public void AddValue(InputConsts.InputCommandType commandType, float frameTime, params float[] values)
        {
            CreateKey(commandType, values.Length, out var curves);
            for (int i = 0; i < curves.Length && i < values.Length; i++)
                curves[i].AddKey(frameTime, values[i]);
        }
        public void AddLineValue(InputConsts.InputCommandType commandType, float begginingTime, float endTime, params float[] values)
        {
            CreateKey(commandType, values.Length, out var curves);
            for (int i = 0; i < curves.Length && i < values.Length; i++)
            {
                //Padding to make them "square waves"
                curves[i].AddKey(begginingTime - Time.unscaledDeltaTime / 2f, 0f);
                curves[i].AddKey(endTime + Time.unscaledDeltaTime / 2f, 0f);

                curves[i].AddKey(begginingTime, values[i]);
                curves[i].AddKey(endTime, values[i]);
            }
        }
        public static InputsCurveRecorder InputsCurveRecorderFromBytes(BinaryReader reader)
        {
            InputsCurveRecorder inputsCurveRecorder = new InputsCurveRecorder() { InputCurves = new Dictionary<InputConsts.InputCommandType, AnimationCurve[]>() };
            int count = reader.ReadInt32();
            for(int i =0; i< count; i++)
            {
                InputConsts.InputCommandType commandType = (InputConsts.InputCommandType)reader.ReadInt32();
                int amountOfAnimationCurves = reader.ReadInt32();
                AnimationCurve[] curves = new AnimationCurve[amountOfAnimationCurves];
                for (int j = 0; j < amountOfAnimationCurves; j++)
                {
                    int amountOfKeys = reader.ReadInt32();
                    Keyframe[] keyframes = new Keyframe[amountOfKeys];
                    for (int k = 0; k < amountOfKeys; k++)
                    {
                        keyframes[k] = new Keyframe
                        {
                            time = reader.ReadSingle(),
                            value = reader.ReadSingle()
                        };
                    }
                    curves[j] = new AnimationCurve(keyframes);
                }
                inputsCurveRecorder.InputCurves.Add(commandType, curves);
            }
            return inputsCurveRecorder;
        }

        public byte[] InputsCurveRecorderInBytes()
        {
            var stream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            binaryWriter.Write(InputCurves.Count);
            foreach (var pair in InputCurves)
            {
                binaryWriter.Write((int)pair.Key);
                binaryWriter.Write(pair.Value.Length);
                for (int i = 0; i < pair.Value.Length; i++)
                {
                    binaryWriter.Write(pair.Value[i].length);
                    foreach (var key in pair.Value[i].keys) //We only care about the whole value, and not its tangents
                    {
                        binaryWriter.Write(key.time);
                        binaryWriter.Write(key.value);
                    }
                }
            }
            binaryWriter.Close();
            return stream.ToArray();
        }
    }
}
