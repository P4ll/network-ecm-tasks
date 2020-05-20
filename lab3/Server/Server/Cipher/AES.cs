using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace crypto_test
{
    public class AES
    {
        private RichTextBox _textBox;
        private Form _form;
        private byte[] _sBox =  {0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
                                0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
                                0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
                                0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
                                0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
                                0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
                                0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
                                0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
                                0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
                                0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
                                0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
                                0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
                                0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
                                0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
                                0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
                                0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16 };

        private byte[] _invSBox =   {0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
                                    0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
                                    0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
                                    0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
                                    0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
                                    0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
                                    0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
                                    0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
                                    0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
                                    0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
                                    0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
                                    0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
                                    0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
                                    0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
                                    0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
                                    0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d };
        public int Nk { get; set; } = 4;
        public int Nb { get; set; } = 4;
        public int Nr { get; set; } = 10;
        private List<byte> _keysExp;

        public AES()
        {

        }

        public AES(ref RichTextBox textBox, ref Form form)
        {
            _textBox = textBox;
            _form = form;
        }

        /// <summary>
        /// Производит шифрование строки message с помощью хешированного пароля pass (128 бит)
        /// </summary>
        /// <param name="message">Строка сообщения</param>
        /// <param name="pass">Пароль в виде строки</param>
        /// <param name="isText">Если true, сообщение будет трактоваться как текст,
        /// иначе - поток байт с побелом в качестве разделителя</param>
        /// <returns></returns>
        public string Encrypt(string message, string pass, bool isText)
        {
            List<byte> bytes = new List<byte>();
            if (isText)
            {
                bytes = Encoding.Default.GetBytes(message).ToList();
            }
            else
            {
                string[] bytesInStrings = message.Split(' ');
                foreach (var ch in bytesInStrings)
                {
                    if (ch == "" || ch == " ")
                        continue;
                    bytes.Add(Byte.Parse(ch));
                }
            }
            // добавим в конец размер в 4 байтах
            // добавим 0, пока количество % 128 != 0 (бит)
            int initLength = bytes.Count;
            for (int i = 3; i >= 0; --i)
                bytes.Add((byte)((initLength >> (i * 8)) & 0xff));
            while (bytes.Count % 16 != 0)
                bytes.Add(0);
            byte[] passInByte = GetBytesFromHash(pass);
            _keysExp = KeysExpansion(ref passInByte);
            // шифруются каждый из 128-ми битных блоков
            byte[] inputBytes = bytes.ToArray();
            for (int i = 0; i < bytes.Count; i += 16)
            {
                EncryptBlock(ref inputBytes, i, i + 15);
            }
            return BitConverter.ToString(inputBytes).Replace("-", string.Empty);
        }

        private void EncryptBlock(ref byte[] bytes, int st, int en)
        {
            AddRoundKey(ref bytes, st, ref _keysExp, 0);
            for (int i = 1; i <= Nr - 1; ++i)
            {
                SubBytes(ref bytes, st, en);
                ShiftRows(ref bytes, st, en);
                MixCols(ref bytes, st, en);
                AddRoundKey(ref bytes, st, ref _keysExp, i * Nb);
                //OutData(ref bytes, st, $"{i} encrypt");
            }
            //OutData(ref bytes, st, "out cycle encr");
            SubBytes(ref bytes, st, en);
            ShiftRows(ref bytes, st, en);
            AddRoundKey(ref bytes, st, ref _keysExp, Nr * Nb);
            //OutData(ref bytes, st, "out encr");
        }

        public string Decrypt(string text, string pass)
        {
            if (text.Length % 32 != 0)
            {
                return "Can't decrypt";
            }
            // byte[] bytes = Encoding.Default.GetBytes(text);
            byte[] bytes = GetBytesFromHash(text);
            byte[] passInByte = GetBytesFromHash(pass);
            _keysExp = KeysExpansion(ref passInByte);
            for (int i = 0; i < bytes.Length; i += 16)
            {
                DecryptBlock(ref bytes, i, i + 15);
            }
            // поток байт имеет следующую структуру: b[0]..b[k], s[4]..s[0], 0..0
            // b - байты сообщения, s - 4 байта размера сообщения
            int messageRightIndex = bytes.Length - 1;
            while (bytes[messageRightIndex] == 0 && messageRightIndex > 0)
            {
                --messageRightIndex;
            }
            if (messageRightIndex == 0)
            {
                return "";
            }
            messageRightIndex -= 4;
            byte[] decryptedMessage = new byte[messageRightIndex + 1];
            for (int i = 0; i < decryptedMessage.Length; ++i)
            {
                decryptedMessage[i] = bytes[i];
            }
            return Encoding.Default.GetString(decryptedMessage);
        }
        private void DecryptBlock(ref byte[] bytes, int st, int en)
        {
            //OutData(ref bytes, st, "init decr");
            AddRoundKey(ref bytes, st, ref _keysExp, Nr * Nb);
            InvShiftRow(ref bytes, st, en);
            InvSubBytes(ref bytes, st, en);
            //OutData(ref bytes, st, "pre cycle decr");

            for (int i = Nr - 1; i >= 1; --i)
            {
                AddRoundKey(ref bytes, st, ref _keysExp, i * Nb);
                InvMixCols(ref bytes, st, en);
                InvShiftRow(ref bytes, st, en);
                InvSubBytes(ref bytes, st, en);
                //OutData(ref bytes, st, $"{i} decr");
            }
            AddRoundKey(ref bytes, st, ref _keysExp, 0);
        }

        private void DecryptBlockOld(ref byte[] bytes, int st, int en)
        {
            AddRoundKey(ref bytes, st, ref _keysExp, Nr * Nb);
            for (int i = Nr - 1; i >= 1; --i)
            {
                InvSubBytes(ref bytes, st, en);
                InvShiftRow(ref bytes, st, en);
                InvMixCols(ref bytes, st, en);
                AddRoundKey(ref bytes, st, ref _keysExp, i * Nb);
            }
            InvSubBytes(ref bytes, st, en);
            InvShiftRow(ref bytes, st, en);
            AddRoundKey(ref bytes, st, ref _keysExp, 0);
        }

        private void OutData(ref byte[] bytes, int st, string name)
        {
            Console.WriteLine(name);
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    Console.Write($"{bytes[i * 4 + j + st]} ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Производит трансформацию входного потока байт через SBox [st, end]
        /// </summary>
        /// <param name="bytes">ссылка на массив байт</param>
        /// <param name="st">индекс начала трансформации</param>
        /// <param name="end">индекс конца</param>
        public void SubBytes(ref byte[] bytes, int st, int end)
        {
            for (int i = st; i <= end; ++i)
            {
                bytes[i] = _sBox[bytes[i]];
            }
        }

        public void InvSubBytes(ref byte[] bytes, int st, int en)
        {
            for (int i = st; i <= en; ++i)
            {
                bytes[i] = _invSBox[bytes[i]];
            }
        }

        public void ShiftRows(ref byte[] bytes, int st, int end)
        {
            for (int i = 1; i < 4; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    byte buf = 0;
                    for (int k = 0; k < 4; ++k)
                    {
                        if (k == 0)
                        {
                            buf = bytes[i * 4 + k + st];
                        }
                        if (k == 3)
                        {
                            bytes[i * 4 + k + st] = buf;
                            continue;
                        }
                        bytes[i * 4 + k + st] = bytes[i * 4 + k + st + 1];
                    }
                }
            }
        }

        public void InvShiftRow(ref byte[] bytes, int st, int en)
        {
            for (int i = 1; i < 4; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    byte buf = 0;
                    for (int k = 3; k >= 0; --k)
                    {
                        if (k == 3)
                        {
                            buf = bytes[i * 4 + k + st];
                        }
                        if (k == 0)
                        {
                            bytes[i * 4 + k + st] = buf;
                            continue;
                        }
                        bytes[i * 4 + k + st] = bytes[i * 4 + k + st - 1];
                    }
                }
            }
        }

        public void MixCols(ref byte[] bytes, int st, int end)
        {
            for (int i = 0; i < 4; ++i)
            {
                int[] buff = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    buff[j] = bytes[j * 4 + i + st];
                }

                bytes[0 * 4 + i + st] = (byte)(ByteMult(0x2, (byte)buff[0]) ^ ByteMult(0x3, (byte)buff[1]) ^ buff[2] ^ buff[3]);
                bytes[1 * 4 + i + st] = (byte)(buff[0] ^ ByteMult(0x2, (byte)buff[1]) ^ ByteMult(0x3, (byte)buff[2]) ^ buff[3]);
                bytes[2 * 4 + i + st] = (byte)(buff[0] ^ buff[1] ^ ByteMult(0x2, (byte)buff[2]) ^ ByteMult(0x3, (byte)buff[3]));
                bytes[3 * 4 + i + st] = (byte)(ByteMult(0x3, (byte)buff[0]) ^ buff[1] ^ buff[2] ^ ByteMult(0x2, (byte)buff[3]));

                //bytes[0 * 4 + i + st] = (byte)((XTime(buff[0]) ^ XTime(buff[1]) ^ buff[1] ^ buff[2] ^ buff[3]) & 0xff);
                //bytes[1 * 4 + i + st] = (byte)((buff[0] ^ XTime(buff[1]) ^ XTime(buff[2]) ^ buff[2] ^ buff[3]) & 0xff);
                //bytes[2 * 4 + i + st] = (byte)((buff[0] ^ buff[1] ^ XTime(buff[2]) ^ XTime(buff[3]) ^ buff[3]) & 0xff);
                //bytes[3 * 4 + i + st] = (byte)((XTime(buff[0]) ^ buff[0] ^ buff[1] ^ buff[2] ^ XTime(buff[3])) & 0xff);
            }
        }

        public void InvMixCols(ref byte[] bytes, int st, int en)
        {
            for (int i = 0; i < 4; ++i)
            {
                int[] buff = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    buff[j] = bytes[j * 4 + i + st];
                }

                bytes[0 * 4 + i + st] = (byte)(ByteMult(0xE, (byte)buff[0]) ^ ByteMult(0xB, (byte)buff[1]) ^ ByteMult(0xD, (byte)buff[2]) ^ ByteMult(0x9, (byte)buff[3]));
                bytes[1 * 4 + i + st] = (byte)(ByteMult(0x9, (byte)buff[0]) ^ ByteMult(0xE, (byte)buff[1]) ^ ByteMult(0xB, (byte)buff[2]) ^ ByteMult(0xD, (byte)buff[3]));
                bytes[2 * 4 + i + st] = (byte)(ByteMult(0xD, (byte)buff[0]) ^ ByteMult(0x9, (byte)buff[1]) ^ ByteMult(0xE, (byte)buff[2]) ^ ByteMult(0xB, (byte)buff[3]));
                bytes[3 * 4 + i + st] = (byte)(ByteMult(0xB, (byte)buff[0]) ^ ByteMult(0xD, (byte)buff[1]) ^ ByteMult(0x9, (byte)buff[2]) ^ ByteMult(0xE, (byte)buff[3]));
            }
        }

        public void AddRoundKey(ref byte[] bytes, int st, ref List<byte> keys, int stKey)
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    bytes[j * 4 + i + st] ^= keys[i * 4 + j + stKey];
                }
            }
        }

        private int XTime(int x)
        {
            int hb = x & 0x80;
            int shiftL = (x << 1) & 0xff;
            return hb == 0 ? (byte)shiftL : (byte)(shiftL ^ 0x1b);
        }

        /// <summary>
        /// Умножение многочлена в GF(2^8) на x
        /// </summary>
        /// <param name="x">Многочлен, умножаемый на x</param>
        /// <param name="y">Степень x</param>
        /// <returns></returns>
        private int XTimePow(int x, int y)
        {
            if (y == 0) return x;
            for (int i = 0; i < y; ++i)
            {
                x = XTime(x);
            }
            return x;
        }

        public List<byte> KeysExpansion(ref byte[] bytes, bool isDecrypt = false)
        {
            List<byte> ans = new List<byte>();
            for (int i = 0; i < bytes.Length; ++i)
            {
                ans.Add(bytes[i]);
            }
            for (int i = Nk; i < Nb * (Nr + 1); ++i)
            {
                byte[] temp = new byte[4];
                for (int j = 0, k = (i - 1) * Nk; j < 4; ++j, ++k)
                {
                    temp[j] = ans[k];
                }
                if (i % Nk == 0)
                {
                    RotWord(ref temp);
                    SubWord(ref temp);
                    XorRCon(ref temp, i / Nk);
                }
                for (int j = 0; j < temp.Length; ++j)
                {
                    ans.Add((byte)((int)ans[(i - Nk) * Nk + j] ^ temp[j]));
                }
            }

            if (isDecrypt)
            {
                byte[] key = ans.ToArray();
                for (int i = 1; i <= (Nr - 1); ++i)
                {
                    InvMixCols(ref key, i * Nb, i * Nb + 16);
                }
                return new List<byte>(key);
            }
            return ans;
        }

        private void XorRCon(ref byte[] bytes, int val)
        {
            int binPow = 2;
            for (int i = 0; i < val - 1; ++i)
            {
                binPow = XTime(binPow);
            }
            bytes[0] = (byte)((int)bytes[0] ^ binPow);
            for (int i = 1; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)((int)bytes[i] ^ 0);
            }
        }

        private void RotWord(ref byte[] bytes)
        {
            byte tElem = bytes.First();
            for (int i = 0; i < bytes.Length - 1; ++i)
            {
                bytes[i] = bytes[i + 1];
            }
            bytes[bytes.Length - 1] = tElem;
        }

        private void SubWord(ref byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = _sBox[bytes[i]];
            }
        }

        public int ByteMult(byte a, byte b)
        {
            int res = 0;
            int bb = b;
            for (int i = 0; i < 8; ++i)
            {
                if (bb == 0) break;
                if ((bb & 1) == 1)
                {
                    res ^= XTimePow(a, i);
                }
                bb >>= 1;
            }
            return res;
        }

        public byte[] GetBytesFromHash(string str)
        {
            return Enumerable.Range(0, str.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(str.Substring(x, 2), 16))
                     .ToArray();
        }
    }
}
