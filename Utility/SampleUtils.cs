using System.IO;

namespace Utility
{
    public static class SampleUtils
    {
        public static uint ReadSample(Stream fileStream, FileInfo fileInfo)
        {
            uint returnBits = 0;
            int readBits;

            unchecked
            {
                for (int bits = 0; bits < fileInfo.SampleBits; bits += 8)
                {
                    readBits = fileStream.ReadByte();
                    ThrowIfEndOfStream(readBits);

                    returnBits |= (uint)(readBits << bits);
                }
            }

            return returnBits;
        }

        public static void WriteSample(uint sample, Stream outputStream, FileInfo fileInfo)
        {
            // TODO: if sample is greater than the value that can be contained
            //       in that number of bits, then throw exception
            unchecked
            {
                for (int bits = 0; bits < fileInfo.SampleBits; bits += 8)
                {
                    byte sampleToWrite = (byte)(sample >> bits);
                    outputStream.WriteByte(sampleToWrite);
                }
            }
        }

        private static void ThrowIfEndOfStream(int readBits)
        {
            if (readBits == -1)
            {
                throw new EndOfStreamException();
            }
        }
    }
}
