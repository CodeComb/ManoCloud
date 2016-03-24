using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mano.Community.Tests
{
    public class CodingNetTests
    {
        [Fact]
        public async void CodingNetTest()
        {
            var ret = await ProjectGetter.FromCodingNet("summer");
            Assert.Equal(9, ret.Count);
        }
    }
}
