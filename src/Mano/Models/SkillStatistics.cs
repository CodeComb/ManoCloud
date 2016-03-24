using System;

namespace Mano.Models
{
    public class SkillStatistics
    {
        public string Skill { get; set; }

        public DateTime Begin { get; set; }

        public long Count { get; set; }
        
        public long TotalDays
        {
            get
            {
                return (long)(DateTime.Now - Begin).TotalDays;
            }
        }
        
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
