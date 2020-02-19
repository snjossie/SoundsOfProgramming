using System.IO;
using Utility;

namespace ConvertToWav
{
    class ConvertToWave
    {
        static void Main(string[] args)
        {
            using (var streamIn = new FileStream(args[0], FileMode.Open))
            {
                var info = FileHeaderUtil.GetHeaderInfo(streamIn);

                uint chunkId = 0x46464952;
                uint chunkSize; // 36 + subchunk2Size
                uint format = 0x45564157;

                uint subchunk1Id = 0x20746d66;
                uint subchunk1Size = 16;
                uint AudioFormat = 1;
                uint channelCount = info.Channels == Channel.Mono ? 1u : 2u;
                uint sampleRate = 8000; // maybe?
                uint byteRate; // sampleRate * NumChannels * bitsPerSample / 8
                uint blockAlign; // NumChannels * bitsPerSample / 8
                uint bitsPerSample = (uint)info.SampleBits;  // 8, 16, 24, 32

                uint Subchunk2Id = 0x61746164;
                uint Subchunk2Size = (uint)info.SampleCount; // sampleCount * numChannels * bitsPerSample / 8

                byteRate = sampleRate * channelCount * bitsPerSample / 8;
                blockAlign = channelCount * bitsPerSample / 8;
                chunkSize = 36 + Subchunk2Size;

                byte[] headerBytes = new byte[44];
                headerBytes[0] = (byte)chunkId;
                headerBytes[1] = (byte)(chunkId >> 8);
                headerBytes[2] = (byte)(chunkId >> 16);
                headerBytes[3] = (byte)(chunkId >> 24);

                headerBytes[4] = (byte)chunkSize;
                headerBytes[5] = (byte)(chunkSize >> 8);
                headerBytes[6] = (byte)(chunkSize >> 16);
                headerBytes[7] = (byte)(chunkSize >> 24);

                headerBytes[8] = (byte)format;
                headerBytes[9] = (byte)(format >> 8);
                headerBytes[10] = (byte)(format >> 16);
                headerBytes[11] = (byte)(format >> 24);

                headerBytes[12] = (byte)subchunk1Id;
                headerBytes[13] = (byte)(subchunk1Id >> 8);
                headerBytes[14] = (byte)(subchunk1Id >> 16);
                headerBytes[15] = (byte)(subchunk1Id >> 24);

                headerBytes[16] = (byte)subchunk1Size;
                headerBytes[17] = (byte)(subchunk1Size >> 8);
                headerBytes[18] = (byte)(subchunk1Size >> 16);
                headerBytes[19] = (byte)(subchunk1Size >> 24);

                headerBytes[20] = (byte)AudioFormat;
                headerBytes[21] = (byte)(AudioFormat >> 8);

                headerBytes[22] = (byte)channelCount;
                headerBytes[23] = (byte)(channelCount >> 8);

                headerBytes[24] = (byte)sampleRate;
                headerBytes[25] = (byte)(sampleRate >> 8);
                headerBytes[26] = (byte)(sampleRate >> 16);
                headerBytes[27] = (byte)(sampleRate >> 24);

                headerBytes[28] = (byte)byteRate;
                headerBytes[29] = (byte)(byteRate >> 8);
                headerBytes[30] = (byte)(byteRate >> 16);
                headerBytes[31] = (byte)(byteRate >> 24);

                headerBytes[32] = (byte)blockAlign;
                headerBytes[33] = (byte)(blockAlign >> 8);

                headerBytes[34] = (byte)(bitsPerSample);
                headerBytes[35] = (byte)(bitsPerSample >> 8);

                headerBytes[36] = (byte)Subchunk2Id;
                headerBytes[37] = (byte)(Subchunk2Id >> 8);
                headerBytes[38] = (byte)(Subchunk2Id >> 16);
                headerBytes[39] = (byte)(Subchunk2Id >> 24);

                headerBytes[40] = (byte)Subchunk2Size;
                headerBytes[41] = (byte)(Subchunk2Size >> 8);
                headerBytes[42] = (byte)(Subchunk2Size >> 16);
                headerBytes[43] = (byte)(Subchunk2Size >> 24);

                using (var streamOut = new FileStream(args[1], FileMode.Create))
                {
                    streamOut.Write(headerBytes, 0, headerBytes.Length);

                    for (int i = 0; i < info.SampleCount; i++)
                    {
                        SampleUtils.WriteSample(SampleUtils.ReadSample(streamIn, info),
                                                streamOut,
                                                info);
                    }
                }
            }

        }
    }
}
