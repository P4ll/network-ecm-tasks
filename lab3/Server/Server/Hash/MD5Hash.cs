using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading;

namespace crypto_test {
    public class MD5Hash {
        private RichTextBox _textBox;
        private Form _form;

        private readonly static uint[] T = new uint[64]
            {   0xd76aa478,0xe8c7b756,0x242070db,0xc1bdceee,
                0xf57c0faf,0x4787c62a,0xa8304613,0xfd469501,
                0x698098d8,0x8b44f7af,0xffff5bb1,0x895cd7be,
                0x6b901122,0xfd987193,0xa679438e,0x49b40821,
                0xf61e2562,0xc040b340,0x265e5a51,0xe9b6c7aa,
                0xd62f105d,0x2441453,0xd8a1e681,0xe7d3fbc8,
                0x21e1cde6,0xc33707d6,0xf4d50d87,0x455a14ed,
                0xa9e3e905,0xfcefa3f8,0x676f02d9,0x8d2a4c8a,
                0xfffa3942,0x8771f681,0x6d9d6122,0xfde5380c,
                0xa4beea44,0x4bdecfa9,0xf6bb4b60,0xbebfbc70,
                0x289b7ec6,0xeaa127fa,0xd4ef3085,0x4881d05,
                0xd9d4d039,0xe6db99e5,0x1fa27cf8,0xc4ac5665,
                0xf4292244,0x432aff97,0xab9423a7,0xfc93a039,
                0x655b59c3,0x8f0ccc92,0xffeff47d,0x85845dd1,
                0x6fa87e4f,0xfe2ce6e0,0xa3014314,0x4e0811a1,
                0xf7537e82,0xbd3af235,0x2ad7d2bb,0xeb86d391 };

        public MD5Hash(ref RichTextBox textBox, ref Form form) {
            _textBox = textBox;
            _form = form;
        }

        public MD5Hash() { }

        public string GetHash(string message, bool isText) {
            List<byte> bytes = new List<byte>();
            if (isText) {
                bytes = Encoding.ASCII.GetBytes(message).ToList();
            }
            else {
                string[] bytesInStrings = message.Split(' ');
                foreach (var ch in bytesInStrings) {
                    if (ch == "" || ch == " ")
                        continue;
                    bytes.Add(Byte.Parse(ch));
                }
            }

            ulong messageLength = (ulong)bytes.Count() * 8;
            // add 80h
            bytes.Add(128);
            // bytes.Count * 8 % 64 * 8 != 56 * 8
            while (bytes.Count % 64 != 56)
                bytes.Add(0);
            // Добавим в конце биты длины сообщений: вначале 4 младшие байта, затем 4 старших
            for (int i = 8; i > 0; i--)
                bytes.Add((byte)(messageLength >> ((8 - i) * 8) & 0xff));
            // Начальные значения
            uint a = 0x67452301; // 67452301h
            uint b = 0xEFCDAB89; // EFCDAB89h
            uint c = 0x98BADCFE; // 98BADCFEh
            uint d = 0x10325476;  // 10325476h

            uint aa;
            uint bb;
            uint cc;
            uint dd;

            for (int i = 0; i < bytes.Count; i += 64) {
                // Получим массив X, сост из 16 блоков по 32 бита.
                // Суммарно за шаг цикла обрабатываются 512 бит = 64 байта.
                uint[] x = GetXArr(ref bytes, i, i + 63);
                // Заносим значения предыдущей операции
                aa = a;
                bb = b;
                cc = c;
                dd = d;
                // state 1
                Trans(1, ref a, b, c, d, x[0], 7, 1);   Trans(1, ref d, a, b, c, x[1], 12, 2);  Trans(1, ref c, d, a, b, x[2], 17, 3);  Trans(1, ref b, c, d, a, x[3], 22, 4);
                Trans(1, ref a, b, c, d, x[4], 7, 5);   Trans(1, ref d, a, b, c, x[5], 12, 6);  Trans(1, ref c, d, a, b, x[6], 17, 7);  Trans(1, ref b, c, d, a, x[7], 22, 8);
                Trans(1, ref a, b, c, d, x[8], 7, 9);   Trans(1, ref d, a, b, c, x[9], 12, 10); Trans(1, ref c, d, a, b, x[10], 17, 11);Trans(1, ref b, c, d, a, x[11], 22, 12);
                Trans(1, ref a, b, c, d, x[12], 7, 13); Trans(1, ref d, a, b, c, x[13], 12, 14);Trans(1, ref c, d, a, b, x[14], 17, 15);Trans(1, ref b, c, d, a, x[15], 22, 16);
                // state 2
                Trans(2, ref a, b, c, d, x[1], 5, 17);  Trans(2, ref d, a, b, c, x[6], 9, 18);  Trans(2, ref c, d, a, b, x[11], 14, 19);Trans(2, ref b, c, d, a, x[0], 20, 20);
                Trans(2, ref a, b, c, d, x[5], 5, 21);  Trans(2, ref d, a, b, c, x[10], 9, 22); Trans(2, ref c, d, a, b, x[15], 14, 23);Trans(2, ref b, c, d, a, x[4], 20, 24);
                Trans(2, ref a, b, c, d, x[9], 5, 25);  Trans(2, ref d, a, b, c, x[14], 9, 26); Trans(2, ref c, d, a, b, x[3], 14, 27); Trans(2, ref b, c, d, a, x[8], 20, 28);
                Trans(2, ref a, b, c, d, x[13], 5, 29); Trans(2, ref d, a, b, c, x[2], 9, 30);  Trans(2, ref c, d, a, b, x[7], 14, 31); Trans(2, ref b, c, d, a, x[12], 20, 32);
                // state 3
                Trans(3, ref a, b, c, d, x[5], 4, 33);  Trans(3, ref d, a, b, c, x[8], 11, 34); Trans(3, ref c, d, a, b, x[11], 16, 35);Trans(3, ref b, c, d, a, x[14], 23, 36);
                Trans(3, ref a, b, c, d, x[1], 4, 37);  Trans(3, ref d, a, b, c, x[4], 11, 38); Trans(3, ref c, d, a, b, x[7], 16, 39); Trans(3, ref b, c, d, a, x[10], 23, 40);
                Trans(3, ref a, b, c, d, x[13], 4, 41); Trans(3, ref d, a, b, c, x[0], 11, 42); Trans(3, ref c, d, a, b, x[3], 16, 43); Trans(3, ref b, c, d, a, x[6], 23, 44);
                Trans(3, ref a, b, c, d, x[9], 4, 45);  Trans(3, ref d, a, b, c, x[12], 11, 46);Trans(3, ref c, d, a, b, x[15], 16, 47);Trans(3, ref b, c, d, a, x[2], 23, 48);
                // state 4
                Trans(4, ref a, b, c, d, x[0], 6, 49);  Trans(4, ref d, a, b, c, x[7], 10, 50); Trans(4, ref c, d, a, b, x[14], 15, 51);Trans(4, ref b, c, d, a, x[5], 21, 52);
                Trans(4, ref a, b, c, d, x[12], 6, 53); Trans(4, ref d, a, b, c, x[3], 10, 54); Trans(4, ref c, d, a, b, x[10], 15, 55);Trans(4, ref b, c, d, a, x[1], 21, 56);
                Trans(4, ref a, b, c, d, x[8], 6, 57);  Trans(4, ref d, a, b, c, x[15], 10, 58);Trans(4, ref c, d, a, b, x[6], 15, 59); Trans(4, ref b, c, d, a, x[13], 21, 60);
                Trans(4, ref a, b, c, d, x[4], 6, 61);  Trans(4, ref d, a, b, c, x[11], 10, 62);Trans(4, ref c, d, a, b, x[2], 15, 63); Trans(4, ref b, c, d, a, x[9], 21, 64);
                // Суммируем результаты
                a = aa + a;
                b = bb + b;
                c = cc + c;
                d = dd + d;
            }
            return ReverseByte(a).ToString("x8") + ReverseByte(b).ToString("x8") + 
                ReverseByte(c).ToString("x8") + ReverseByte(d).ToString("x8");
        }

        private void Trans(int state, ref uint a, uint b, uint c, uint d, uint k, ushort s, int i) {
            switch (state) {
                case 1:
                    /* [abcd k s i] a = b + ((a + F(b,c,d) + X[k] + T[i]) <<< s). */
                    a = b + ShiftL((a + ( (b & c) | (~b & d) ) + k + T[i - 1]), s);
                    break;
                case 2:
                    /* [abcd k s i] a = b + ((a + G(b,c,d) + X[k] + T[i]) <<< s). */
                    a = b + ShiftL((a + ( (b & d) | (~d & c) ) + k + T[i - 1]), s);
                    break;
                case 3:
                    /* [abcd k s i] a = b + ((a + H(b,c,d) + X[k] + T[i]) <<< s). */
                    a = b + ShiftL((a + ( b ^ c ^ d ) + k + T[i - 1]), s);
                    break;
                case 4:
                    /* [abcd k s i] a = b + ((a + I(b,c,d) + X[k] + T[i]) <<< s). */
                    a = b + ShiftL((a + ( c ^ (~d | b) ) + k + T[i - 1]), s);
                    break;
            }
        }

        private uint ShiftL(uint num, ushort steps) {
            return ((num >> 32 - steps) | (num << steps));
        }

        private uint ReverseByte(uint uiNumber) {
            return (((uiNumber & 0x000000ff) << 24) |
                        (uiNumber >> 24) |
                    ((uiNumber & 0x00ff0000) >> 8) |
                    ((uiNumber & 0x0000ff00) << 8));
        }

        private uint[] GetXArr(ref List<byte> arr, int beg, int end) {
            uint[] x = new uint[16];
            for (int i = beg, j = 0; i <= end && j < 16; i += 4, ++j) {
                x[j] = ((uint)arr[i + 3] << 24) | ((uint)arr[i + 2] << 16) | ((uint)arr[i + 1] << 8) | (uint)arr[i];
            }
            return x;
        }
    }
}
