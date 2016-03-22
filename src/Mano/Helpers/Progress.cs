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
            return (Days * 100 / days).ToString("0.00") + '%';
        }

        public static string PercentF<TModel>(this IHtmlHelper<TModel> self, long Days)
        {
            var days = self.ViewBag.TotalF;
            return (Days * 100 / days).ToString("0.00") + '%';
        }
    }
}
