using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace Mano.Community
{
    public static class Http
    {
        public static async Task<string> Get(string url)
        {
            var client = new HttpClient();
            var result = await client.GetAsync(url);
            return await result.Content.ReadAsStringAsync();
        }
    }
}
