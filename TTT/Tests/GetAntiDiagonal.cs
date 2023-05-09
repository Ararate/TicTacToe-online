using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTT.Utility;

namespace Tests
{
    public class GetAntiDiagonal
    {
        private char[,] a = new char[3, 3] { { 'A', 'S', 'D' },
                                             { 'Z', 'X', 'C' },
                                             { 'E', 'R', 'T' } };
        [Fact]
        public void ArrayExtGetAntiDiag_x2_y2()
        {
            char[] res = a.GetAntiDiag(2, 2);

            Assert.Equal("T", new String(res));
        }
        [Fact]
        public void ArrayExtGetAntiDiag_x2_y0()
        {
            char[] res = a.GetAntiDiag(2, 0);

            Assert.Equal("EXD", new String(res));
        }
        [Fact]
        public void ArrayExtGetAntiDiag_x2_y1()
        {
            char[] res = a.GetAntiDiag(2, 1);

            Assert.Equal("RC", new String(res));
        }
        [Fact]
        public void ArrayExtGetAntiDiag_x0_y0()
        {
            char[] res = a.GetAntiDiag(0, 0);

            Assert.Equal("A", new String(res));
        }
        [Fact]
        public void ArrayExtGetAntiDiag_x0_y2()
        {
            char[] res = a.GetAntiDiag(0, 2);

            Assert.Equal("EXD", new String(res));
        }
    }
}
