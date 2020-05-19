using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace crypto_test {
    public class SquareGenerator : Generator {
        private RichTextBox _textBox;
        
        private const ulong Seed = 19;
        private const ulong AConst = 1103515245;
        private const ulong BConst = 65539;//65539;48271
        private const ulong CConst = 12345; //12345;
        private const ulong MConst = ((ulong)1 << 31) - 1;
        private ulong _curAns = Seed;

        public SquareGenerator(ref RichTextBox textBox) : base(ref textBox) {
            GenerateSequenceAbstract = GenerateSequenceImplementation;
            GenerateNextAbstract = Next;
            _textBox = textBox;
        }

        public ulong Next() {
            //_curAns = ((AConst * AConst * _curAns) % MConst + BConst * _curAns + CConst) % MConst;
            _curAns = Add(Mult(BConst, _curAns), CConst);
            //_curAns = Add(Add(Mult(Mult(AConst, AConst), _curAns), Mult(BConst, _curAns)), CConst);
            Console.WriteLine(_curAns);
            return _curAns;
        }

        private ulong Mult(ulong num1, ulong num2) {
            return (num1 * num2) % MConst;
        }

        private ulong Add(ulong num1, ulong num2) { 
            return (num1 + num2) % MConst;
        }
    }
}
