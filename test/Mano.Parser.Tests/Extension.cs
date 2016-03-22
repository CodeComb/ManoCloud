using System.ComponentModel.DataAnnotations;

namespace Mano.Parser.Tests
{
    public enum TechnologyType
    {
        编程语言,
        序列化格式,
        数据库,
        工具或框架,
        其他
    }

    public class Extension
    {
        [MaxLength(32)]
        public string Id { get; set; }

        [MaxLength(32)]
        public string Technology { get; set; }

        public TechnologyType Type { get; set; }
    }
}
