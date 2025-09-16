// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using NUnit.Framework;

namespace HashDepot.Test;

[TestFixture]
public partial class XXH3Test
{
    const ulong prime32 = 2654435761U;
    const ulong prime64 = 11400714785074694797UL;

    const int testBufferSize = 2367;
    readonly byte[] testBuffer = new byte[testBufferSize];

    [SetUp]
    public void SetUp()
    {
        fillTestBuffer(testBuffer.AsSpan());
    }

    static void fillTestBuffer(Span<byte> buffer)
    {
        ulong byteGen = prime32;

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)(byteGen >> 56);
            byteGen *= prime64;
        }
    }

    static readonly object[][] seededTestData =
    [
        [    0,     0UL, 0x2D06800538D394C2UL ],  /* empty string */
        [    0, prime64, 0xA8A6B918B2F0364AUL ],
        [    1,     0UL, 0xC44BDFF4074EECDBUL ],  /*  1 -  3 */
        [    1, prime64, 0x032BE332DD766EF8UL ],
        [    6,     0UL, 0x27B56A84CD2D7325UL ],  /*  4 -  8 */
        [    6, prime64, 0x84589C116AB59AB9UL ],
        [   12,     0UL, 0xA713DAF0DFBB77E7UL ],  /*  9 - 16 */
        [   12, prime64, 0xE7303E1B2336DE0EUL ],
        [   24,     0UL, 0xA3FE70BF9D3510EBUL ],  /* 17 - 32 */
        [   24, prime64, 0x850E80FC35BDD690UL ],
        [   48,     0UL, 0x397DA259ECBA1F11UL ],  /* 33 - 64 */
        [   48, prime64, 0xADC2CBAA44ACC616UL ],
        [   80,     0UL, 0xBCDEFBBB2C47C90AUL ],  /* 65 - 96 */
        [   80, prime64, 0xC6DD0CB699532E73UL ],
        [  195,     0UL, 0xCD94217EE362EC3AUL ],  /* 129-240 */
        [  195, prime64, 0xBA68003D370CB3D9UL ],

        [  403,     0UL, 0xCDEB804D65C6DEA4UL ],  /* one block, last stripe is overlapping */
        [  403, prime64, 0x6259F6ECFD6443FDUL ],
        [  512,     0UL, 0x617E49599013CB6BUL ],  /* one block, finishing at stripe boundary */
        [  512, prime64, 0x3CE457DE14C27708UL ],
        [ 2048,     0UL, 0xDD59E2C3A5F038E0UL ],  /* 2 blocks, finishing at block boundary */
        [ 2048, prime64, 0x66F81670669ABABCUL ],
        [ 2099,     0UL, 0xC6B9D9B3FC9AC765UL ],  /* 2 blocks + 1 partial block, to detect off-by-one scrambling issues, like #816 */
        [ 2099, prime64, 0x184F316843663974UL ],
        [ 2240,     0UL, 0x6E73A90539CF2948UL ],  /* 3 blocks, finishing at stripe boundary */
        [ 2240, prime64, 0x757BA8487D1B5247UL ],
        [ 2367,     0UL, 0xCB37AEB9E5D361EDUL ],  /* 3 blocks, last stripe is overlapping */
        [ 2367, prime64, 0xD2DB3415B942B42AUL ]
    ];

    static readonly object[][] customSecretTestData =
    [
        [ 0, 0, 0x3559D64878C5C66CUL ],  /* empty string */
        [ 1, 0, 0x8A52451418B2DA4DUL ],  /*  1 -  3 */
        [ 6, 0, 0x82C90AB0519369ADUL ],  /*  4 -  8 */
        [ 12, 0, 0x14631E773B78EC57UL ],  /*  9 - 16 */
        [ 24, 0, 0xCDD5542E4A9D9FE8UL ],  /* 17 - 32 */
        [ 48, 0, 0x33ABD54D094B2534UL ],  /* 33 - 64 */
        [ 80, 0, 0xE687BA1684965297UL ],  /* 65 - 96 */
        [ 195, 0, 0xA057273F5EECFB20UL ],  /* 129-240 */

        [ 403, 0, 0x14546019124D43B8UL ],  /* one block, last stripe is overlapping */
        [ 512, 0, 0x7564693DD526E28DUL ],  /* one block, finishing at stripe boundary */
        [ 2048, 0, 0xD32E975821D6519FUL ],  /* >= 2 blodcks, at least one scrambling */
        [ 2367, 0, 0x293FA8E5173BB5E7UL ],  /* >= 2 blocks, at least one scrambling, last stripe unaligned */

        [ 64 * 10 * 3, 0, 0x751D2EC54BC6038BUL ]   /* exactly 3 fUL blocks, not a multiple of 256 */
    ];

    static readonly object[][] generateSecretTestData =
    [
        [ 0, 192, new byte[] { 0xE7, 0x8C, 0x77, 0x77, 0x00 } ],
        [ 1, 240, new byte[] { 0x2B, 0x3E, 0xDE, 0xC1, 0x00 } ],
        [ XXHash.XXH3.MinSecretSize - 1, 277, new byte[] { 0xE8, 0x39, 0x6C, 0xCC, 0x7B } ],
        [ XXHash.XXH3.SecretDefaultSize + 500, XXHash.XXH3.MaxSecretSize, new byte[] { 0xD6, 0x1C, 0x41, 0x17, 0xB3 } ],
    ];

    [Test]
    [TestCaseSource(nameof(seededTestData))]
    public void Hash64_SeededData(int len, ulong seed, ulong expectedResult)
    {
        var result = XXHash.XXH3.Hash64(testBuffer.AsSpan(0, len), seed);
        Assert.That(result, Is.EqualTo(expectedResult), $"Failed for len={len}, seed={seed}");
    }
}
