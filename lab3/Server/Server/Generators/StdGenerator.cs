using System;
using System.Text;
using System.Windows.Forms;

namespace crypto_test
{
    public class StdGenerator
    {
        private RichTextBox _textBox;
        private Random _rand;

        public StdGenerator()
        {
            _rand = new Random();
        }

        public ulong Next()
        {
            return (ulong)_rand.Next();
        }

        private string GenerateSequenceImplementation(int numbers)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < numbers; ++i)
            {
                sb.Append(_rand.Next(0, 2));
            }

            return sb.ToString();
        }
    }
}
