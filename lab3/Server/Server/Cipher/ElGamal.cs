using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Forms;

namespace crypto_test {
    public class ElGamal {
        private BigInteger _lowerBound = BigInteger.Pow(10, 20);
        private BigInteger _upperBound = BigInteger.Pow(10, 50);
        private BigInteger _q, _g, _h; // public key
        private BigInteger _p; // часть шифрованного сообщения
        private BigInteger _key; // private key

        private Utils.Progress _progress;

        private RichTextBox _textBox;
        private Form _form;

        public ElGamal() {
            _q = Utils.BigNumbersHelper.Rand(_lowerBound, _upperBound);
            _g = Utils.BigNumbersHelper.Rand(2, _q);
            _key = KeyGen();
            _h = BigInteger.ModPow(_g, _key, _q);
        }

        public ElGamal(ref RichTextBox textBox, ref Form form) {
            _textBox = textBox;
            _form = form;
            _q = Utils.BigNumbersHelper.Rand(_lowerBound, _upperBound);
            _g = Utils.BigNumbersHelper.Rand(2, _q);
            _key = KeyGen();
            _h = BigInteger.ModPow(_g, _key, _q);
        }

        private BigInteger KeyGen() {
            BigInteger key = Utils.BigNumbersHelper.Rand(_lowerBound, _q);
            while (Utils.BigNumbersHelper.Gcd(_q, key) != 1) {
                key = Utils.BigNumbersHelper.Rand(_lowerBound, _q);
            }
            return key;
        }

        public string Encrypt(string inputStr, bool isText) {
            if (_form != null) {
                _progress = new Utils.Progress(0, inputStr.Length, 1, "Шифрование");
                _progress.Show();
            }
            List<byte> bytes = new List<byte>();
            if (isText) {
                bytes = Encoding.Default.GetBytes(inputStr).ToList();
            }
            else {
                string[] bytesInStrings = inputStr.Split(' ');
                foreach (var ch in bytesInStrings) {
                    if (ch == "" || ch == " ")
                        continue;
                    bytes.Add(Byte.Parse(ch));
                }
            }

            BigInteger k = KeyGen();
            BigInteger s = BigInteger.ModPow(_h, k, _q);
            _p = BigInteger.ModPow(_g, k, _q);
            List<BigInteger> encryptedMessage = new List<BigInteger>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Count; ++i) {
                encryptedMessage.Add(s * bytes[i]);
                sb.Append(encryptedMessage[i].ToString() + '\n');
                if (_form != null) {
                    _progress.PerformStep();
                }
            }
            if (_form != null) {
                _progress.CloseFormSafe();
            }
            return sb.ToString();
        }

        public string Decrypt(string encryptedText) {
            string[] textNumb = encryptedText.Split('\n');
            List<BigInteger> message = new List<BigInteger>();
            for (int i = 0; i < textNumb.Length; ++i) {
                if (textNumb[i] != "" && textNumb[i] != "\r") {
                    message.Add(BigInteger.Parse(textNumb[i]));
                }
            }
            if (_form != null) {
                _progress = new Utils.Progress(0, message.Count, 1, "Дешифрование");
                _progress.Show();
            }
            BigInteger h = BigInteger.ModPow(_p, _key, _q);
            byte[] decryptedMessage = new byte[message.Count];
            for (int i = 0; i < message.Count; ++i) {
                decryptedMessage[i] = (byte)(message[i] / h);
                if (_form != null) {
                    _progress.PerformStep();
                }
            }
            if (_form != null) {
                _progress.CloseFormSafe();
                string curExt = WordPad.Helpers.ReadHelper.GetExtention(_form.Text);
                if (WordPad.Form1.TxtExts.Any(str => str == curExt)) {
                    return Encoding.Default.GetString(decryptedMessage);
                }
                else {
                    StringBuilder sb = new StringBuilder();
                    foreach (var b in decryptedMessage) {
                        sb.Append($"{b} ");
                    }
                    return sb.ToString();
                }
            }
            return Encoding.Default.GetString(decryptedMessage);
        }
    }
}
