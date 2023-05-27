using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTT.Utility;

namespace Tests
{
    public class Has3InRow
    {
        [Fact]
        public void Has3InRow_3_Empty_Chars()
        {
            char[] array = new char[3] { ' ', ' ', ' ' };
            bool res = array.Has3InRow();

            Assert.True(res);
        }
        [Fact]
        public void Has3InRow_Empty_X_Empty()
        {
            char[] array = new char[3] { ' ', 'X', ' ' };
            bool res = array.Has3InRow();

            Assert.False(res);
        }
        [Fact]
        public void Has3InRow_0_0_X()
        {
            char[] array = new char[3] { '0', '0', 'X' };
            bool res = array.Has3InRow();

            Assert.False(res);
        }

    }
}
