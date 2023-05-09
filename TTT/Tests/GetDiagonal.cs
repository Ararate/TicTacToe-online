using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTT.Utility;

namespace Tests
{
    public class GetDiagonal
    {
        private char[,] a = new char[3, 3] { { 'A', 'S', 'D' },
                                             { 'Z', 'X', 'C' },
                                             { 'E', 'R', 'T' } };
        [Fact]
        public void ArrayExtGetMainDiag_x2_y2()
        {
            char[] res = a.GetMainDiag(2, 2);

            Assert.Equal("AXT", new String(res));
        }
        [Fact]
        public void ArrayExtGetMainDiag_x0_y2()
        {
            char[] res = a.GetMainDiag(0, 2);

            Assert.Equal("E", new String(res));
        }
        [Fact]
        public void ArrayExtGetMainDiag_x2_y1()
        {
            char[] res = a.GetMainDiag(2, 1);

            Assert.Equal("SC", new String(res));
        }
        [Fact]
        public void ArrayExtGetMainDiag_x0_y0()
        {
            char[] res = a.GetMainDiag(0, 0);

            Assert.Equal("AXT", new String(res));
        }
    }
}
