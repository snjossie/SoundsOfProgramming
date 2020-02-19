using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Utility.Test
{
    [TestFixture]
    public class SampleUtilsTests
    {
        [Test]
        public void EightBitSample_ReadOneSample_ReturnsSample()
        {
            var stream = new MemoryStream(new byte[] { 0x1 });
            var info = new FileInfo { Channels = Channel.Mono, Frequency = 1, SampleBits = 8, SampleCount = 1 };

            var actual = SampleUtils.ReadSample(stream, info);

            Assert.AreEqual(0x1, actual);
        }

        [Test]
        public void SixteenBitSample_ReadOneSample_ReturnsSample()
        {
            var stream = new MemoryStream(new byte[] { 0x1, 0x2 });
            var info = new FileInfo { Channels = Channel.Mono, Frequency = 1, SampleBits = 16, SampleCount = 1 };

            var actual = SampleUtils.ReadSample(stream, info);

            const int expected = 0x1 | (0x2 << 8);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ThirtyTwoBitSample_ReadOneSample_ReturnsSample()
        {
            var stream = new MemoryStream(new byte[] { 0x1, 0x2, 0x3, 0x4 });
            var info = new FileInfo { Channels = Channel.Mono, Frequency = 1, SampleBits = 32, SampleCount = 1 };

            var actual = SampleUtils.ReadSample(stream, info);

            const int expected = 0x1 | (0x2 << 8) | (0x3 << 16) | (0x4 << 24);

            Assert.AreEqual(expected, actual);
        }
    }
}
