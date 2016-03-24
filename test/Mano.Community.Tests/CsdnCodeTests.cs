using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mano.Community.Tests
{
    public class CsdnCodeTests
    {
        [Fact]
        public async void CsdnCodeTest()
        {
            var ret = await ProjectGetter.FromCsdnCode("Innost");
            Assert.Equal(5, ret.Count);
        }
    }
}
