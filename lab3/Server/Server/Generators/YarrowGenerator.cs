using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace crypto_test {
    public class YarrowGenerator : Generator {

        public YarrowGenerator(ref RichTextBox textBox) : base(ref textBox) {
            GenerateSequenceAbstract = GenerateSequenceImplementation;
            GenerateNextAbstract = Next;
        }

        private ulong Next() {
            return 0;
        }

        private string GenerateSequenceImplementation(int numbers, ref Utils.Progress progressForm) {
            StringBuilder sb = new StringBuilder();

            return sb.ToString();
        }
    }
}
