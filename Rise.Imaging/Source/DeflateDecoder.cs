using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Rise.Imaging
{
    public class DeflateDecoder
    {
        static readonly byte[] lengthBits;
        static readonly ushort[] lengthBase;
        static readonly byte[] distBits;
        static readonly ushort[] distBase;

        static readonly byte[] clcidx = {
            16, 17, 18, 0, 8, 7, 9, 6,
            10, 5, 11, 4, 12, 3, 13, 2,
            14, 1, 15
        };

        static DeflateDecoder()
        {
            void Build(ref byte[] bits, ref ushort[] bas, int delta, int first)
            {
                bits = new byte[30];
                bas = new ushort[30];

                for (int i = 0; i < 30 - delta; ++i)
                    bits[i + delta] = (byte)(i / delta);

                for (int sum = first, i = 0; i < 30; ++i)
                {
                    bas[i] = (ushort)sum;
                    sum += 1 << bits[i];
                }
            }

            Build(ref lengthBits, ref lengthBase, 4, 3);
            Build(ref distBits, ref distBase, 2, 1);

            lengthBits[28] = 0;
            lengthBase[28] = 258;
        }

        byte[] source;
        int sourceInd;
        byte[] dest;
        int destCount;
        int tag;
        int bitCount;
        int curLen;
        int lzOff;
        ushort[] lTable = new ushort[16];
        ushort[] lTrans = new ushort[288];
        ushort[] dTable = new ushort[16];
        ushort[] dTrans = new ushort[288];
        ushort[] offs = new ushort[16];
        byte[] lengths = new byte[320];

        public byte[] Decode(byte[] source, int sourceIndex)
        {
            this.source = source;
            sourceInd = sourceIndex;

            if (dest == null)
                dest = new byte[source.Length.ToPowerOf2()];
            else if (source.Length > dest.Length)
                Array.Resize(ref dest, source.Length.ToPowerOf2());

            destCount = 0;
            bitCount = 0;
            curLen = 0;

            var res = false;
            while (!res)
                res = Uncompress();

            var results = new byte[destCount];
            Buffer.BlockCopy(dest, 0, results, 0, destCount);
            return results;
        }

        void BuildFixedTrees()
        {
            for (int i = 0; i < 7; ++i)
                lTable[i] = 0;

            lTable[7] = 24;
            lTable[8] = 152;
            lTable[9] = 112;

            for (int i = 0; i < 24; ++i)
                lTrans[i] = (ushort)(256 + i);

            for (ushort i = 0; i < 144; ++i)
                lTrans[24 + i] = i;

            for (int i = 0; i < 8; ++i)
                lTrans[24 + 144 + i] = (ushort)(280 + i);

            for (int i = 0; i < 112; ++i)
                lTrans[24 + 144 + 8 + i] = (ushort)(144 + i);

            for (int i = 0; i < 5; ++i)
                dTable[i] = 0;

            dTable[5] = 32;

            for (ushort i = 0; i < 32; ++i)
                dTrans[i] = i;
        }

        void BuildTree(ushort[] table, ushort[] trans, byte[] lens, int lensInd, int num)
        {
            for (int i = 0; i < 16; ++i)
                table[i] = 0;

            for (int i = 0; i < num; ++i)
                table[lens[lensInd + i]]++;

            table[0] = 0;

            for (ushort sum = 0, i = 0; i < 16; ++i)
            {
                offs[i] = sum;
                sum += table[i];
            }

            for (ushort i = 0; i < num; ++i)
                if (lens[lensInd + i] > 0)
                    trans[offs[lens[lensInd + i]]++] = i;
        }

        void Put(byte c)
        {
            if (destCount == dest.Length)
                Array.Resize(ref dest, destCount * 2);
            dest[destCount++] = c;
        }

        uint GetUintLE()
        {
            uint val = 0;
            val = val >> 8 | (uint)source[sourceInd++] << 24;
            val = val >> 8 | (uint)source[sourceInd++] << 24;
            val = val >> 8 | (uint)source[sourceInd++] << 24;
            val = val >> 8 | (uint)source[sourceInd++] << 24;
            return val;
        }

        uint GetUintBE()
        {
            uint val = 0;
            val = val << 8 | (uint)source[sourceInd++];
            val = val << 8 | (uint)source[sourceInd++];
            val = val << 8 | (uint)source[sourceInd++];
            val = val << 8 | (uint)source[sourceInd++];
            return val;
        }

        int GetBit()
        {
            if ((bitCount--) == 0)
            {
                tag = source[sourceInd++];
                bitCount = 7;
            }

            int bit = tag & 0x01;
            tag >>= 1;

            return bit;
        }

        int ReadBits(int num, int baseVal)
        {
            int val = 0;

            if (num != 0)
            {
                int limit = 1 << num;

                for (int mask = 1; mask < limit; mask *= 2)
                    if (GetBit() == 1)
                        val += mask;
            }

            return baseVal + val;
        }

        int DecodeSymbol(ushort[] table, ushort[] trans)
        {
            int sum = 0;
            int cur = 0;
            int len = 0;

            do
            {
                cur = 2 * cur + GetBit();
                ++len;
                sum += table[len];
                cur -= table[len];
            }
            while (cur >= 0);

            return trans[sum + cur];
        }

        void DecodeTrees()
        {
            int hlit = ReadBits(5, 257);
            int hdist = ReadBits(5, 1);
            int hclen = ReadBits(4, 4);

            for (int i = 0; i < 19; ++i)
                lengths[i] = 0;

            for (int i = 0; i < hclen; ++i)
            {
                int clen = ReadBits(3, 0);
                lengths[clcidx[i]] = (byte)clen;
            }

            BuildTree(lTable, lTrans, lengths, 0, 19);

            for (int num = 0; num < hlit + hdist; )
            {
                int sym = DecodeSymbol(lTable, lTrans);

                switch (sym)
                {
                    case 16:
                        {
                            byte prev = lengths[num - 1];
                            for (int length = ReadBits(2, 3); length > 0; --length)
                                lengths[num++] = prev;
                        }
                        break;
                    case 17:
                        for (int length = ReadBits(3, 3); length > 0; --length)
                            lengths[num++] = 0;
                        break;
                    case 18:
                        for (int length = ReadBits(7, 11); length > 0; --length)
                            lengths[num++] = 0;
                        break;
                    default:
                        lengths[num++] = (byte)sym;
                        break;
                }
            }

            BuildTree(lTable, lTrans, lengths, 0, hlit);
            BuildTree(dTable, dTrans, lengths, hlit, hdist);
        }

        bool InflateBlockData()
        {
            if (curLen == 0)
            {
                int sym = DecodeSymbol(lTable, lTrans);

                if (sym < 256)
                {
                    Put((byte)sym);
                    return false;
                }

                if (sym == 256)
                    return true;

                sym -= 257;
                curLen = ReadBits(lengthBits[sym], lengthBase[sym]);

                int dist = DecodeSymbol(dTable, dTrans);
                lzOff = -ReadBits(distBits[dist], distBase[dist]);
            }

            Put(dest[destCount + lzOff]);
            curLen--;

            return false;
        }

        bool InflateUncompressedBlock()
        {
            if (curLen == 0)
            {
                int length = source[sourceInd++] + 256 * source[sourceInd++];
                int invlength = source[sourceInd++] + 256 * source[sourceInd++];

                if (length != (~invlength & 0x0000ffff))
                    throw new Exception("Data error.");

                curLen = length + 1;
                bitCount = 0;
            }

            --curLen;
            if (curLen == 0)
                return true;

            Put(source[sourceInd++]);
            return false;
        }

        bool Uncompress()
        {
            int bType = -1;
            int bFinal = 0;
            bool res = false;

            while (true)
            {
                if (bType == -1)
                {
                    bFinal = GetBit();
                    bType = ReadBits(2, 0);

                    if (bType == 1)
                        BuildFixedTrees();
                    else if (bType == 2)
                        DecodeTrees();
                }
                switch (bType)
                {
                    case 0: res = InflateUncompressedBlock(); break;
                    case 1: case 2: res = InflateBlockData(); break;
                    default: throw new Exception("Data error.");
                }

                if (res)
                {
                    if (bFinal == 0)
                        bType = -1;
                    else
                        return true;
                }
            }
        }
    }
}
