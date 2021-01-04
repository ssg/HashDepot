// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashDepot.Test
{
    // adapted from: http://www.isthe.com/chongo/src/fnv/test_fnv.c (public domain)
    public static class FnvVectors
    {
        public static IEnumerable<FnvTestVector> GetFnv1aTestVectors()
        {
            return getTestVectors(fnv1aResults32, fnv1aResults64);
        }

        private static byte[] toAsciiBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        private static byte[] toNullPaddedAsciiBytes(string text)
        {
            var result = new byte[text.Length + 1];
            var buffer = Encoding.ASCII.GetBytes(text);
            Array.Copy(buffer, result, text.Length);
            return result;
        }

        private static byte[] repeat10(string text)
        {
            return repeat(text, 10);
        }

        private static byte[] repeat500(string text)
        {
            return repeat(text, 500);
        }

        private static byte[] repeat10(byte[] buffer)
        {
            return repeat(buffer, 10);
        }

        private static byte[] repeat500(byte[] buffer)
        {
            return repeat(buffer, 500);
        }

        private static byte[] repeat(string text, int count)
        {
            var buffer = Encoding.ASCII.GetBytes(text);
            return repeat(buffer, count);
        }

        private static byte[] repeat(byte[] buffer, int count)
        {
            int length = buffer.Length;
            var result = new byte[length * count];
            for (int n = 0; n < count; n++)
            {
                Array.Copy(buffer, 0, result, n * length, length);
            }
            return result;
        }

        private static IEnumerable<FnvTestVector> getTestVectors(uint[] results32, ulong[] results64)
        {
            var bytes = getTestBytes();
            return Enumerable.Range(0, bytes.Length)
                .Select(n => new FnvTestVector(bytes[n], results32[n], results64[n]));
        }

        private static readonly ulong[] fnv1aResults64 = new ulong[]
        {
            0xcbf29ce484222325,
            0xaf63dc4c8601ec8c,
            0xaf63df4c8601f1a5,
            0xaf63de4c8601eff2,
            0xaf63d94c8601e773,
            0xaf63d84c8601e5c0,
            0xaf63db4c8601ead9,
            0x08985907b541d342,
            0xdcb27518fed9d577,
            0xdd120e790c2512af,
            0xcac165afa2fef40a,
            0x85944171f73967e8,
            0xaf63bd4c8601b7df,
            0x089be207b544f1e4,
            0x08a61407b54d9b5f,
            0x08a2ae07b54ab836,
            0x0891b007b53c4869,
            0x088e4a07b5396540,
            0x08987c07b5420ebb,
            0xdcb28a18fed9f926,
            0xdd1270790c25b935,
            0xcac146afa2febf5d,
            0x8593d371f738acfe,
            0x34531ca7168b8f38,
            0x08a25607b54a22ae,
            0xf5faf0190cf90df3,
            0xf27397910b3221c7,
            0x2c8c2b76062f22e0,
            0xe150688c8217b8fd,
            0xf35a83c10e4f1f87,
            0xd1edd10b507344d0,
            0x2a5ee739b3ddb8c3,
            0xdcfb970ca1c0d310,
            0x4054da76daa6da90,
            0xf70a2ff589861368,
            0x4c628b38aed25f17,
            0x9dd1f6510f78189f,
            0xa3de85bd491270ce,
            0x858e2fa32a55e61d,
            0x46810940eff5f915,
            0xf5fadd190cf8edaa,
            0xf273ed910b32b3e9,
            0x2c8c5276062f6525,
            0xe150b98c821842a0,
            0xf35aa3c10e4f55e7,
            0xd1ed680b50729265,
            0x2a5f0639b3dded70,
            0xdcfbaa0ca1c0f359,
            0x4054ba76daa6a430,
            0xf709c7f5898562b0,
            0x4c62e638aed2f9b8,
            0x9dd1a8510f779415,
            0xa3de2abd4911d62d,
            0x858e0ea32a55ae0a,
            0x46810f40eff60347,
            0xc33bce57bef63eaf,
            0x08a24307b54a0265,
            0xf5b9fd190cc18d15,
            0x4c968290ace35703,
            0x07174bd5c64d9350,
            0x5a294c3ff5d18750,
            0x05b3c1aeb308b843,
            0xb92a48da37d0f477,
            0x73cdddccd80ebc49,
            0xd58c4c13210a266b,
            0xe78b6081243ec194,
            0xb096f77096a39f34,
            0xb425c54ff807b6a3,
            0x23e520e2751bb46e,
            0x1a0b44ccfe1385ec,
            0xf5ba4b190cc2119f,
            0x4c962690ace2baaf,
            0x0716ded5c64cda19,
            0x5a292c3ff5d150f0,
            0x05b3e0aeb308ecf0,
            0xb92a5eda37d119d9,
            0x73ce41ccd80f6635,
            0xd58c2c132109f00b,
            0xe78baf81243f47d1,
            0xb0968f7096a2ee7c,
            0xb425a84ff807855c,
            0x23e4e9e2751b56f9,
            0x1a0b4eccfe1396ea,
            0x54abd453bb2c9004,
            0x08ba5f07b55ec3da,
            0x337354193006cb6e,
            0xa430d84680aabd0b,
            0xa9bc8acca21f39b1,
            0x6961196491cc682d,
            0xad2bb1774799dfe9,
            0x6961166491cc6314,
            0x8d1bb3904a3b1236,
            0x6961176491cc64c7,
            0xed205d87f40434c7,
            0x6961146491cc5fae,
            0xcd3baf5e44f8ad9c,
            0xe3b36596127cd6d8,
            0xf77f1072c8e8a646,
            0xe3b36396127cd372,
            0x6067dce9932ad458,
            0xe3b37596127cf208,
            0x4b7b10fa9fe83936,
            0xaabafe7104d914be,
            0xf4d3180b3cde3eda,
            0xaabafd7104d9130b,
            0xf4cfb20b3cdb5bb1,
            0xaabafc7104d91158,
            0xf4cc4c0b3cd87888,
            0xe729bac5d2a8d3a7,
            0x74bc0524f4dfa4c5,
            0xe72630c5d2a5b352,
            0x6b983224ef8fb456,
            0xe73042c5d2ae266d,
            0x8527e324fdeb4b37,
            0x0a83c86fee952abc,
            0x7318523267779d74,
            0x3e66d3d56b8caca1,
            0x956694a5c0095593,
            0xcac54572bb1a6fc8,
            0xa7a4c9f3edebf0d8,
            0x7829851fac17b143,
            0x2c8f4c9af81bcf06,
            0xd34e31539740c732,
            0x3605a2ac253d2db1,
            0x08c11b8346f4a3c3,
            0x6be396289ce8a6da,
            0xd9b957fb7fe794c5,
            0x05be33da04560a93,
            0x0957f1577ba9747c,
            0xda2cc3acc24fba57,
            0x74136f185b29e7f0,
            0xb2f2b4590edb93b2,
            0xb3608fce8b86ae04,
            0x4a3a865079359063,
            0x5b3a7ef496880a50,
            0x48fae3163854c23b,
            0x07aaa640476e0b9a,
            0x2f653656383a687d,
            0xa1031f8e7599d79c,
            0xa31908178ff92477,
            0x097edf3c14c3fb83,
            0xb51ca83feaa0971b,
            0xdd3c0d96d784f2e9,
            0x86cd26a9ea767d78,
            0xe6b215ff54a30c18,
            0xec5b06a1c5531093,
            0x45665a929f9ec5e5,
            0x8c7609b4a9f10907,
            0x89aac3a491f0d729,
            0x32ce6b26e0f4a403,
            0x614ab44e02b53e01,
            0xfa6472eb6eef3290,
            0x9e5d75eb1948eb6a,
            0xb6d12ad4a8671852,
            0x88826f56eba07af1,
            0x44535bf2645bc0fd,
            0x169388ffc21e3728,
            0xf68aac9e396d8224,
            0x8e87d7e7472b3883,
            0x295c26caa8b423de,
            0x322c814292e72176,
            0x8a06550eb8af7268,
            0xef86d60e661bcf71,
            0x9e5426c87f30ee54,
            0xf1ea8aa826fd047e,
            0x0babaf9a642cb769,
            0x4b3341d4068d012e,
            0xd15605cbc30a335c,
            0x5b21060aed8412e5,
            0x45e2cda1ce6f4227,
            0x50ae3745033ad7d4,
            0xaa4588ced46bf414,
            0xc1b0056c4a95467e,
            0x56576a71de8b4089,
            0xbf20965fa6dc927e,
            0x569f8383c2040882,
            0xe1e772fba08feca0,
            0x4ced94af97138ac4,
            0xc4112ffb337a82fb,
            0xd64a4fd41de38b7d,
            0x4cfc32329edebcbb,
            0x0803564445050395,
            0xaa1574ecf4642ffd,
            0x694bc4e54cc315f9,
            0xa3d7cb273b011721,
            0x577c2f8b6115bfa5,
            0xb7ec8c1a769fb4c1,
            0x5d5cfce63359ab19,
            0x33b96c3cd65b5f71,
            0xd845097780602bb9,
            0x84d47645d02da3d5,
            0x83544f33b58773a5,
            0x9175cbb2160836c5,
            0xc71b3bc175e72bc5,
            0x636806ac222ec985,
            0xb6ef0e6950f52ed5,
            0xead3d8a0f3dfdaa5,
            0x922908fe9a861ba5,
            0x6d4821de275fd5c5,
            0x1fe3fce62bd816b5,
            0xc23e9fccd6f70591,
            0xc1af12bdfe16b5b5,
            0x39e9f18f2f85e221,
        };

        /* FNV-1a 32 bit test vectors */

        private static readonly uint[] fnv1aResults32 = new uint[]
        {
            0x811c9dc5,
            0xe40c292c,
            0xe70c2de5,
            0xe60c2c52,
            0xe10c2473,
            0xe00c22e0,
            0xe30c2799,
            0x6222e842,
            0xa9f37ed7,
            0x3f5076ef,
            0x39aaa18a,
            0xbf9cf968,
            0x050c5d1f,
            0x2b24d044,
            0x9d2c3f7f,
            0x7729c516,
            0xb91d6109,
            0x931ae6a0,
            0x052255db,
            0xbef39fe6,
            0x6150ac75,
            0x9aab3a3d,
            0x519c4c3e,
            0x0c1c9eb8,
            0x5f299f4e,
            0xef8580f3,
            0xac297727,
            0x4546b9c0,
            0xbd564e7d,
            0x6bdd5c67,
            0xdd77ed30,
            0xf4ca9683,
            0x4aeb9bd0,
            0xe0e67ad0,
            0xc2d32fa8,
            0x7f743fb7,
            0x6900631f,
            0xc59c990e,
            0x448524fd,
            0xd49930d5,
            0x1c85c7ca,
            0x0229fe89,
            0x2c469265,
            0xce566940,
            0x8bdd8ec7,
            0x34787625,
            0xd3ca6290,
            0xddeaf039,
            0xc0e64870,
            0xdad35570,
            0x5a740578,
            0x5b004d15,
            0x6a9c09cd,
            0x2384f10a,
            0xda993a47,
            0x8227df4f,
            0x4c298165,
            0xfc563735,
            0x8cb91483,
            0x775bf5d0,
            0xd5c428d0,
            0x34cc0ea3,
            0xea3b4cb7,
            0x8e59f029,
            0x2094de2b,
            0xa65a0ad4,
            0x9bbee5f4,
            0xbe836343,
            0x22d5344e,
            0x19a1470c,
            0x4a56b1ff,
            0x70b8e86f,
            0x0a5b4a39,
            0xb5c3f670,
            0x53cc3f70,
            0xc03b0a99,
            0x7259c415,
            0x4095108b,
            0x7559bdb1,
            0xb3bf0bbc,
            0x2183ff1c,
            0x2bd54279,
            0x23a156ca,
            0x64e2d7e4,
            0x683af69a,
            0xaed2346e,
            0x4f9f2cab,
            0x02935131,
            0xc48fb86d,
            0x2269f369,
            0xc18fb3b4,
            0x50ef1236,
            0xc28fb547,
            0x96c3bf47,
            0xbf8fb08e,
            0xf3e4d49c,
            0x32179058,
            0x280bfee6,
            0x30178d32,
            0x21addaf8,
            0x4217a988,
            0x772633d6,
            0x08a3d11e,
            0xb7e2323a,
            0x07a3cf8b,
            0x91dfb7d1,
            0x06a3cdf8,
            0x6bdd3d68,
            0x1d5636a7,
            0xd5b808e5,
            0x1353e852,
            0xbf16b916,
            0xa55b89ed,
            0x3c1a2017,
            0x0588b13c,
            0xf22f0174,
            0xe83641e1,
            0x6e69b533,
            0xf1760448,
            0x64c8bd58,
            0x97b4ea23,
            0x9a4e92e6,
            0xcfb14012,
            0xf01b2511,
            0x0bbb59c3,
            0xce524afa,
            0xdd16ef45,
            0x60648bb3,
            0x7fa4bcfc,
            0x5053ae17,
            0xc9302890,
            0x956ded32,
            0x9136db84,
            0xdf9d3323,
            0x32bb6cd0,
            0xc8f8385b,
            0xeb08bfba,
            0x62cc8e3d,
            0xc3e20f5c,
            0x39e97f17,
            0x7837b203,
            0x319e877b,
            0xd3e63f89,
            0x29b50b38,
            0x5ed678b8,
            0xb0d5b793,
            0x52450be5,
            0xfa72d767,
            0x95066709,
            0x7f52e123,
            0x76966481,
            0x063258b0,
            0x2ded6e8a,
            0xb07d7c52,
            0xd0c71b71,
            0xf684f1bd,
            0x868ecfa8,
            0xf794f684,
            0xd19701c3,
            0x346e171e,
            0x91f8f676,
            0x0bf58848,
            0x6317b6d1,
            0xafad4c54,
            0x0f25681e,
            0x91b18d49,
            0x7d61c12e,
            0x5147d25c,
            0x9a8b6805,
            0x4cd2a447,
            0x1e549b14,
            0x2fe1b574,
            0xcf0cd31e,
            0x6c471669,
            0x0e5eef1e,
            0x2bed3602,
            0xb26249e0,
            0x2c9b86a4,
            0xe415e2bb,
            0x18a98d1d,
            0xb7df8b7b,
            0x241e9075,
            0x063f70dd,
            0x0295aed9,
            0x56a7f781,
            0x253bc645,
            0x46610921,
            0x7c1577f9,
            0x512b2851,
            0x76823999,
            0xc0586935,
            0xf3415c85,
            0x0ae4ff65,
            0x58b79725,
            0xdea43aa5,
            0x2bb3be35,
            0xea777a45,
            0x8f21c305,
            0x5c9d0865,
            0xfa823dd5,
            0x21a27271,
            0x83c5c6d5,
            0x813b0881,
        };

        private static byte[][] getTestBytes()
        {
            return buildBytes().ToArray();
        }

        private static IEnumerable<byte[]> buildBytes()
        {
            yield return toAsciiBytes("");
            yield return toAsciiBytes("a");
            yield return toAsciiBytes("b");
            yield return toAsciiBytes("c");
            yield return toAsciiBytes("d");
            yield return toAsciiBytes("e");
            yield return toAsciiBytes("f");
            yield return toAsciiBytes("fo");
            yield return toAsciiBytes("foo");
            yield return toAsciiBytes("foob");
            yield return toAsciiBytes("fooba");
            yield return toAsciiBytes("foobar");
            yield return toNullPaddedAsciiBytes("");
            yield return toNullPaddedAsciiBytes("a");
            yield return toNullPaddedAsciiBytes("b");
            yield return toNullPaddedAsciiBytes("c");
            yield return toNullPaddedAsciiBytes("d");
            yield return toNullPaddedAsciiBytes("e");
            yield return toNullPaddedAsciiBytes("f");
            yield return toNullPaddedAsciiBytes("fo");
            yield return toNullPaddedAsciiBytes("foo");
            yield return toNullPaddedAsciiBytes("foob");
            yield return toNullPaddedAsciiBytes("fooba");
            yield return toNullPaddedAsciiBytes("foobar");
            yield return toAsciiBytes("ch");
            yield return toAsciiBytes("cho");
            yield return toAsciiBytes("chon");
            yield return toAsciiBytes("chong");
            yield return toAsciiBytes("chongo");
            yield return toAsciiBytes("chongo ");
            yield return toAsciiBytes("chongo w");
            yield return toAsciiBytes("chongo wa");
            yield return toAsciiBytes("chongo was");
            yield return toAsciiBytes("chongo was ");
            yield return toAsciiBytes("chongo was h");
            yield return toAsciiBytes("chongo was he");
            yield return toAsciiBytes("chongo was her");
            yield return toAsciiBytes("chongo was here");
            yield return toAsciiBytes("chongo was here!");
            yield return toAsciiBytes("chongo was here!\n");
            yield return toNullPaddedAsciiBytes("ch");
            yield return toNullPaddedAsciiBytes("cho");
            yield return toNullPaddedAsciiBytes("chon");
            yield return toNullPaddedAsciiBytes("chong");
            yield return toNullPaddedAsciiBytes("chongo");
            yield return toNullPaddedAsciiBytes("chongo ");
            yield return toNullPaddedAsciiBytes("chongo w");
            yield return toNullPaddedAsciiBytes("chongo wa");
            yield return toNullPaddedAsciiBytes("chongo was");
            yield return toNullPaddedAsciiBytes("chongo was ");
            yield return toNullPaddedAsciiBytes("chongo was h");
            yield return toNullPaddedAsciiBytes("chongo was he");
            yield return toNullPaddedAsciiBytes("chongo was her");
            yield return toNullPaddedAsciiBytes("chongo was here");
            yield return toNullPaddedAsciiBytes("chongo was here!");
            yield return toNullPaddedAsciiBytes("chongo was here!\n");
            yield return toAsciiBytes("cu");
            yield return toAsciiBytes("cur");
            yield return toAsciiBytes("curd");
            yield return toAsciiBytes("curds");
            yield return toAsciiBytes("curds ");
            yield return toAsciiBytes("curds a");
            yield return toAsciiBytes("curds an");
            yield return toAsciiBytes("curds and");
            yield return toAsciiBytes("curds and ");
            yield return toAsciiBytes("curds and w");
            yield return toAsciiBytes("curds and wh");
            yield return toAsciiBytes("curds and whe");
            yield return toAsciiBytes("curds and whey");
            yield return toAsciiBytes("curds and whey\n");
            yield return toNullPaddedAsciiBytes("cu");
            yield return toNullPaddedAsciiBytes("cur");
            yield return toNullPaddedAsciiBytes("curd");
            yield return toNullPaddedAsciiBytes("curds");
            yield return toNullPaddedAsciiBytes("curds ");
            yield return toNullPaddedAsciiBytes("curds a");
            yield return toNullPaddedAsciiBytes("curds an");
            yield return toNullPaddedAsciiBytes("curds and");
            yield return toNullPaddedAsciiBytes("curds and ");
            yield return toNullPaddedAsciiBytes("curds and w");
            yield return toNullPaddedAsciiBytes("curds and wh");
            yield return toNullPaddedAsciiBytes("curds and whe");
            yield return toNullPaddedAsciiBytes("curds and whey");
            yield return toNullPaddedAsciiBytes("curds and whey\n");
            yield return toAsciiBytes("hi");
            yield return toNullPaddedAsciiBytes("hi");
            yield return toAsciiBytes("hello");
            yield return toNullPaddedAsciiBytes("hello");
            yield return new byte[] { 0xff, 0x00, 0x00, 0x01 };
            yield return new byte[] { 0x01, 0x00, 0x00, 0xff };
            yield return new byte[] { 0xff, 0x00, 0x00, 0x02 };
            yield return new byte[] { 0x02, 0x00, 0x00, 0xff };
            yield return new byte[] { 0xff, 0x00, 0x00, 0x03 };
            yield return new byte[] { 0x03, 0x00, 0x00, 0xff };
            yield return new byte[] { 0xff, 0x00, 0x00, 0x04 };
            yield return new byte[] { 0x04, 0x00, 0x00, 0xff };
            yield return new byte[] { 0x40, 0x51, 0x4e, 0x44 };
            yield return new byte[] { 0x44, 0x4e, 0x51, 0x40 };
            yield return new byte[] { 0x40, 0x51, 0x4e, 0x4a };
            yield return new byte[] { 0x4a, 0x4e, 0x51, 0x40 };
            yield return new byte[] { 0x40, 0x51, 0x4e, 0x54 };
            yield return new byte[] { 0x54, 0x4e, 0x51, 0x40 };
            yield return toAsciiBytes("127.0.0.1");
            yield return toNullPaddedAsciiBytes("127.0.0.1");
            yield return toAsciiBytes("127.0.0.2");
            yield return toNullPaddedAsciiBytes("127.0.0.2");
            yield return toAsciiBytes("127.0.0.3");
            yield return toNullPaddedAsciiBytes("127.0.0.3");
            yield return toAsciiBytes("64.81.78.68");
            yield return toNullPaddedAsciiBytes("64.81.78.68");
            yield return toAsciiBytes("64.81.78.74");
            yield return toNullPaddedAsciiBytes("64.81.78.74");
            yield return toAsciiBytes("64.81.78.84");
            yield return toNullPaddedAsciiBytes("64.81.78.84");
            yield return toAsciiBytes("feedface");
            yield return toNullPaddedAsciiBytes("feedface");
            yield return toAsciiBytes("feedfacedaffdeed");
            yield return toNullPaddedAsciiBytes("feedfacedaffdeed");
            yield return toAsciiBytes("feedfacedeadbeef");
            yield return toNullPaddedAsciiBytes("feedfacedeadbeef");
            yield return toAsciiBytes("line 1\nline 2\nline 3");
            yield return toAsciiBytes("chongo <Landon Curt Noll> /\\../\\");
            yield return toNullPaddedAsciiBytes("chongo <Landon Curt Noll> /\\../\\");
            yield return toAsciiBytes("chongo (Landon Curt Noll) /\\../\\");
            yield return toNullPaddedAsciiBytes("chongo (Landon Curt Noll) /\\../\\");
            yield return toAsciiBytes("http://antwrp.gsfc.nasa.gov/apod/astropix.html");
            yield return toAsciiBytes("http://en.wikipedia.org/wiki/Fowler_Noll_Vo_hash");
            yield return toAsciiBytes("http://epod.usra.edu/");
            yield return toAsciiBytes("http://exoplanet.eu/");
            yield return toAsciiBytes("http://hvo.wr.usgs.gov/cam3/");
            yield return toAsciiBytes("http://hvo.wr.usgs.gov/cams/HMcam/");
            yield return toAsciiBytes("http://hvo.wr.usgs.gov/kilauea/update/deformation.html");
            yield return toAsciiBytes("http://hvo.wr.usgs.gov/kilauea/update/images.html");
            yield return toAsciiBytes("http://hvo.wr.usgs.gov/kilauea/update/maps.html");
            yield return toAsciiBytes("http://hvo.wr.usgs.gov/volcanowatch/current_issue.html");
            yield return toAsciiBytes("http://neo.jpl.nasa.gov/risk/");
            yield return toAsciiBytes("http://norvig.com/21-days.html");
            yield return toAsciiBytes("http://primes.utm.edu/curios/home.php");
            yield return toAsciiBytes("http://slashdot.org/");
            yield return toAsciiBytes("http://tux.wr.usgs.gov/Maps/155.25-19.5.html");
            yield return toAsciiBytes("http://volcano.wr.usgs.gov/kilaueastatus.php");
            yield return toAsciiBytes("http://www.avo.alaska.edu/activity/Redoubt.php");
            yield return toAsciiBytes("http://www.dilbert.com/fast/");
            yield return toAsciiBytes("http://www.fourmilab.ch/gravitation/orbits/");
            yield return toAsciiBytes("http://www.fpoa.net/");
            yield return toAsciiBytes("http://www.ioccc.org/index.html");
            yield return toAsciiBytes("http://www.isthe.com/cgi-bin/number.cgi");
            yield return toAsciiBytes("http://www.isthe.com/chongo/bio.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/index.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/src/calc/lucas-calc");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/astro/venus2004.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/astro/vita.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/comp/c/expert.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/comp/calc/index.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/comp/fnv/index.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/math/number/howhigh.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/math/number/number.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/math/prime/mersenne.html");
            yield return toAsciiBytes("http://www.isthe.com/chongo/tech/math/prime/mersenne.html#largest");
            yield return toAsciiBytes("http://www.lavarnd.org/cgi-bin/corpspeak.cgi");
            yield return toAsciiBytes("http://www.lavarnd.org/cgi-bin/haiku.cgi");
            yield return toAsciiBytes("http://www.lavarnd.org/cgi-bin/rand-none.cgi");
            yield return toAsciiBytes("http://www.lavarnd.org/cgi-bin/randdist.cgi");
            yield return toAsciiBytes("http://www.lavarnd.org/index.html");
            yield return toAsciiBytes("http://www.lavarnd.org/what/nist-test.html");
            yield return toAsciiBytes("http://www.macosxhints.com/");
            yield return toAsciiBytes("http://www.mellis.com/");
            yield return toAsciiBytes("http://www.nature.nps.gov/air/webcams/parks/havoso2alert/havoalert.cfm");
            yield return toAsciiBytes("http://www.nature.nps.gov/air/webcams/parks/havoso2alert/timelines_24.cfm");
            yield return toAsciiBytes("http://www.paulnoll.com/");
            yield return toAsciiBytes("http://www.pepysdiary.com/");
            yield return toAsciiBytes("http://www.sciencenews.org/index/home/activity/view");
            yield return toAsciiBytes("http://www.skyandtelescope.com/");
            yield return toAsciiBytes("http://www.sput.nl/~rob/sirius.html");
            yield return toAsciiBytes("http://www.systemexperts.com/");
            yield return toAsciiBytes("http://www.tq-international.com/phpBB3/index.php");
            yield return toAsciiBytes("http://www.travelquesttours.com/index.htm");
            yield return toAsciiBytes("http://www.wunderground.com/global/stations/89606.html");
            yield return repeat10("21701");
            yield return repeat10("M21701");
            yield return repeat10("2^21701-1");
            yield return repeat10(new byte[] { 0x54, 0xc5 });
            yield return repeat10(new byte[] { 0xc5, 0x54 });
            yield return repeat10("23209");
            yield return repeat10("M23209");
            yield return repeat10("2^23209-1");
            yield return repeat10(new byte[] { 0x5a, 0xa9 });
            yield return repeat10(new byte[] { 0xa9, 0x5a });
            yield return repeat10("391581216093");
            yield return repeat10("391581*2^216093-1");
            yield return repeat10(new byte[] { 0x05, 0xf9, 0x9d, 0x03, 0x4c, 0x81 });
            yield return repeat10("FEDCBA9876543210");
            yield return repeat10(new byte[] { 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10 });
            yield return repeat10("EFCDAB8967452301");
            yield return repeat10(new byte[] { 0xef, 0xcd, 0xab, 0x89, 0x67, 0x45, 0x23, 0x01 });
            yield return repeat10("0123456789ABCDEF");
            yield return repeat10(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef });
            yield return repeat10("1032547698BADCFE");
            yield return repeat10(new byte[] { 0x10, 0x32, 0x54, 0x76, 0x98, 0xba, 0xdc, 0xfe });
            yield return repeat500(new byte[] { 0 });
            yield return repeat500(new byte[] { 0x07 });
            yield return repeat500("~");
            yield return repeat500(new byte[] { 0x7f });
        }
    }
}