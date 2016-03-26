using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;

namespace Mano.Helpers
{
    public static class Progress
    {
        public static string PercentP<TModel>(this IHtmlHelper<TModel> self, long Days)
        {
            var days = self.ViewBag.TotalP;
            if (days == 0)
                return "0.00%";
            return (Days * 100 / days).ToString("0.00") + '%';
        }

        public static string PercentF<TModel>(this IHtmlHelper<TModel> self, long Days)
        {
            var days = self.ViewBag.TotalF;
            if (days == 0)
                return "0.00%";
            return (Days * 100 / days).ToString("0.00") + '%';
        }

        public static string Percent<TModel>(this IHtmlHelper<TModel> self, long Max, long Actual)
        {
            if (Max == 0)
                return "0.00%";
            return (Actual * 100 / Max).ToString("0.00") + '%';
        }
    }
}
