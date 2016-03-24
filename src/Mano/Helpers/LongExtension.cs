using System.Text;

namespace Mano.Helpers
{
    public static class LongExtension
    {
        public static string ToSplitedString(this long self)
        {
            var str = self.ToString();
            var ret = new StringBuilder();
            var offset = str.Length % 3;
            for (var i = 0; i < str.Length; i++)
            {
                if (i > 0 && i < str.Length - 1 && (i - offset) % 3 == 0)
                {
                    ret.Append(',');
                }
                ret.Append(str[i]);
            }
            return ret.ToString();
        }
    }
}
