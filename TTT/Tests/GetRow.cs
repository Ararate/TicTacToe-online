using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTT.Utility;

namespace Tests
{
    public class GetRow
    {
        private char[,] a = new char[3, 3] { { 'A', 'S', 'D' },
                                             { 'Z', 'X', 'C' },
                                             { 'E', 'R', 'T' } };
        [Fact]
        public void GetRow_0()
        {
            char[] res = a.GetRow(0);

            Assert.Equal("ASD", new String(res));
        }
        [Fact]
        public void GetRow_2()
        {
            char[] res = a.GetRow(2);

            Assert.Equal("ERT", new String(res));
        }

    }
}
