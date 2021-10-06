using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InputDemoRecorder
{
    public class DemoFileLoader
    {
        static public FrameInputRecorder[] LoadedDemoFile;
        public const string DEMO_FILE_EXTENSION = ".owdemo";

        public static bool SaveDemoFile(string filePath, string demoName, params FrameInputRecorder[] recordedFrameInputs)
        {
            if (recordedFrameInputs.Length <= 0)
                return false;
            var stream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            //Metadata
            binaryWriter.Write(Application.unityVersion); // Unity version
            binaryWriter.Write(demoName); //Saved demo name
            binaryWriter.Write(DateTime.UtcNow.ToBinary()); //Time it was saved

            binaryWriter.Write(recordedFrameInputs[0].GetDictionaryOrdersInBytes()); //Dictionary Order

            binaryWriter.Write(recordedFrameInputs.Length); //Frame Inputs
            for (int i = 0; i < recordedFrameInputs.Length; i++)
                binaryWriter.Write(recordedFrameInputs[i].FrameInputRecorderInBytes());

            binaryWriter.Close();
            File.WriteAllBytes(Path.Combine(filePath, demoName + DEMO_FILE_EXTENSION), stream.ToArray());
            return true;
        }

        public static bool LoadDemoFile(string fileName)
        {
            byte[] fileBuffer = File.ReadAllBytes(fileName);
            var stream = new MemoryStream(fileBuffer);
            BinaryReader binaryReader = new BinaryReader(stream);

            //Metadata
            string unityVersion = binaryReader.ReadString();
            string demoName = binaryReader.ReadString();
            DateTime demoSaveTime = DateTime.FromBinary(binaryReader.ReadInt64());

            //Dictionary order
            string[] axisInputChannels = new string[binaryReader.ReadInt32()];
            for(int i=0; i< axisInputChannels.Length; i++)
                axisInputChannels[i] = binaryReader.ReadString();

            string[] buttonsInputChannels = new string[binaryReader.ReadInt32()];
            for (int i = 0; i < buttonsInputChannels.Length; i++)
                buttonsInputChannels[i] = binaryReader.ReadString();

            //Frame Inputs
            int amountOfFrameInputs = binaryReader.ReadInt32();
            LoadedDemoFile = new FrameInputRecorder[amountOfFrameInputs];
            for (int i = 0; i < LoadedDemoFile.Length; i++)
            {
                FrameInputRecorder frameInputRecorder = new FrameInputRecorder(i);
                for (int j =0; j< axisInputChannels.Length; j++)
                    frameInputRecorder.AddAxisInput(axisInputChannels[j], new AxisInputRecorder(binaryReader.ReadSingle(), binaryReader.ReadSingle()));

                for (int j = 0; j < buttonsInputChannels.Length; j++)
                    frameInputRecorder.AddButtonInput(buttonsInputChannels[j], new ButtonInputRecorder(binaryReader.ReadBoolean(), binaryReader.ReadBoolean(), binaryReader.ReadBoolean()));

                LoadedDemoFile[i] = frameInputRecorder;
            }

            return true;
        }
    }
}
