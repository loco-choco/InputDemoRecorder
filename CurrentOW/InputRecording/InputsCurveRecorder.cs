using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InputDemoRecorder
{
    public struct InputsCurveRecorder
    {
        public Dictionary<InputConsts.InputCommandType, AnimationCurve[]> InputCurves;
        
        public void AddValue(InputConsts.InputCommandType commandType, float frameTime, params float[] values)
        {
            AnimationCurve[] curves;

            if (InputCurves.TryGetValue(commandType, out var curvesFromKey))
                curves = curvesFromKey;
            else
            {
                curves = new AnimationCurve[values.Length];
                InputCurves.Add(commandType, curves);
            }

            for (int i = 0; i < curves.Length && i < values.Length; i++)
                curves[i].AddKey(frameTime, values[i]);
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
