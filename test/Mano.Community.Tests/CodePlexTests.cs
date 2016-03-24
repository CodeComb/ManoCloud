using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mano.Community.Tests
{
    public class CodePlexTests
    {
        [Fact]
        public async void CodePlexTest()
        {
            var ret = await ProjectGetter.FromCodePlex("jiangzm");
            Assert.Equal(43, ret.Count);
        }
    }
}
