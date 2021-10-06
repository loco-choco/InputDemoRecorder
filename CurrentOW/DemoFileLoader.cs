using System;
using System.IO;
using UnityEngine;

namespace InputDemoRecorder
{
    public class DemoFileLoader
    {
        public const string DEMO_FILE_EXTENSION = ".owdemo";

        public static bool SaveDemoFile(string filePath, string demoName, InputsCurveRecorder recordedFrameInputs)
        {
            if (recordedFrameInputs.InputCurves.Count <= 0)
                return false;
            var stream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            //Metadata
            binaryWriter.Write(Application.unityVersion); // Unity version
            binaryWriter.Write(demoName); //Saved demo name
            binaryWriter.Write(DateTime.UtcNow.ToBinary()); //Time it was saved

            binaryWriter.Write(recordedFrameInputs.InputsCurveRecorderInBytes());

            binaryWriter.Close();
            File.WriteAllBytes(Path.Combine(filePath, demoName + DEMO_FILE_EXTENSION), stream.ToArray());
            return true;
        }

        public static bool LoadDemoFile(string fileName, out InputsCurveRecorder loadedDemoFile)
        {
            byte[] fileBuffer = File.ReadAllBytes(fileName);
            var stream = new MemoryStream(fileBuffer);
            BinaryReader binaryReader = new BinaryReader(stream);

            //Metadata
            string unityVersion = binaryReader.ReadString();
            string demoName = binaryReader.ReadString();
            DateTime demoSaveTime = DateTime.FromBinary(binaryReader.ReadInt64());

            loadedDemoFile = InputsCurveRecorder.InputsCurveRecorderFromBytes(binaryReader);
            return true;
        }
    }
}
