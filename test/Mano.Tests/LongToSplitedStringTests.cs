using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mano.Helpers;
using Xunit;

namespace Mano.Tests
{
    public class LongToSplitedStringTests
    {
        [Theory]
        [InlineData(123456, "123,456")]
        [InlineData(57438295, "57,438,295")]
        [InlineData(157438295, "157,438,295")]
        [InlineData(1157438295, "1,157,438,295")]
        [InlineData(0, "0")]
        [InlineData(123, "123")]
        [InlineData(12, "12")]
        [InlineData(1234, "1,234")]
        [InlineData(12345, "12,345")]
        public void SplitTest(long num, string ans)
        {
            Assert.Equal(ans, num.ToSplitedString());
        }
    }
}
