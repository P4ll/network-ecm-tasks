using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypto_test {
    class LFSR {
        private ulong _sCur = uint.MaxValue;
        private int[] _taps;

        public LFSR() { 
            
        }

        public LFSR(ulong seed) {
            _sCur = seed;
        }

        public LFSR(ulong seed, int[] taps) {
            _sCur = seed;
            _taps = taps;
        }

        public LFSR(int[] taps) {
            _sCur = ulong.MaxValue;
            _taps = taps;
        }

        public ulong GetNextBitStd() {
            _sCur = ((((_sCur >> 0) ^ (_sCur >> 1) ^ (_sCur >> 2) ^ (_sCur >> 4) ^ (_sCur >> 6) ^ (_sCur >> 31)) & 1) << 31) | (_sCur >> 1);
            return _sCur & 1;
        }

        public ulong GetNextBit() {
            if (_taps.Length > 0) {
                ulong sum = _sCur >> _taps[0];
                for (int i = 1; i < _taps.Length; ++i) {
                    sum ^= _sCur >> _taps[i];
                }
                sum = (sum & 1) << 63;
                _sCur = sum | (_sCur >> 1);
            }
            return _sCur & 1;
        }
    }
}
