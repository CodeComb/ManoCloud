using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mano.Community.Tests
{
    public class GitOscTests
    {
        [Fact]
        public async void GitOscTest()
        {
            var ret = await ProjectGetter.FromGitOSC("nele");
            Assert.Equal(18, ret.Count);
        }
    }
}
