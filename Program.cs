using System;
using System.IO;

namespace LZ4Wtf
{
    internal class Program
    {
        static byte[] LZ4(byte[] Data, int Len)
        {
            var Decomp = new byte[Len];
            int CompressedPos = 0, DecompressedPos = 0;
            int GetLength(int Length)
            {
                byte CurByte;
                if (Length == 15) do Length += CurByte = Data[CompressedPos++]; while (CurByte == 255);
                return Length;
            }
            do
            {
                byte Tok = Data[CompressedPos++];
                int EncCount = (Tok >> 0) & 15, Lcnt = (Tok >> 4) & 15;
                Lcnt = GetLength(Lcnt);
                Buffer.BlockCopy(Data, CompressedPos, Decomp, DecompressedPos, Lcnt);
                CompressedPos += Lcnt;
                DecompressedPos += Lcnt;
                if (CompressedPos >= Data.Length) break;
                int Bk = Data[CompressedPos++] << 0 | Data[CompressedPos++] << 8;
                EncCount = GetLength(EncCount) + 4;
                int EncPos = DecompressedPos - Bk;
                if (EncCount <= Bk)
                {
                    Buffer.BlockCopy(Decomp, EncPos, Decomp, DecompressedPos, EncCount);
                    DecompressedPos += EncCount;
                }
                else while (EncCount-- > 0) Decomp[DecompressedPos++] = Decomp[EncPos++];
            }
            while (CompressedPos < Data.Length && DecompressedPos < Decomp.Length);
            return Decomp;
        }

        static void Main(string[] args)
        {
            var Name = new FileInfo(args[0]).Name.Split('.')[0];

            using (var FileToDecode = File.OpenRead(args[0]))
            {
                using (var Rd = new BinaryReader(FileToDecode))
                {
                    var Len = Rd.ReadInt32();
                    File.WriteAllBytes($"{Name}.nro", LZ4(Rd.ReadBytes((int)FileToDecode.Length - 4), Len));
                }
            }
        }
    }
}