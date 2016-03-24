using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mano.Community.Tests
{
    public class GitHubTests
    {
        [Fact]
        public async void RegexTest()
        {
            var ret = await ProjectGetter.FromGitHub("Kagamine");
            Assert.Equal(64, ret.Count);
        }
    }
}
