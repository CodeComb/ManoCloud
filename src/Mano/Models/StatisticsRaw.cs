using System;
using Newtonsoft.Json;

namespace Mano.Models
{
    public class StatisticsRaw
    {
        public string Technology { get; set; }
        public long Mine { get; set; }
        public long Total { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public TechnologyType Type { get; set; }

        [JsonIgnore]
        public long TotalDays
        {
            get
            {
                return (long)(DateTime.Now - Begin).TotalDays;
            }
        }

        [JsonIgnore]
        public string Display
        {
            get
            {
                if (TotalDays < 1)
                    return "1天";
                if (TotalDays < 30)
                    return TotalDays + "天";
                if (TotalDays < 365)
                    return (TotalDays / 30).ToString() + "个月";
                return (TotalDays / 365).ToString() + "年";
            }
        }

    }
}
