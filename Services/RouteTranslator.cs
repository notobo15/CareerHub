using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public class RouteTranslator : DynamicRouteValueTransformer
    {
        private readonly Dictionary<string, Dictionary<string, string>> _localizedRoutes;

        public RouteTranslator()
        {
            _localizedRoutes = new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string> { ["home"] = "home", ["index"] = "index" },
                ["vi"] = new Dictionary<string, string> { ["home"] = "trang-chu", ["index"] = "trang-chu" }
            };
        }

        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var culture = values["culture"]?.ToString()?.ToLower() ?? "en";
            var controller = values["controller"]?.ToString()?.ToLower();
            var action = values["action"]?.ToString()?.ToLower();

            if (_localizedRoutes.ContainsKey(culture))
            {
                var reverseLookup = _localizedRoutes[culture];

                // Reverse map from localized name to real controller/action
                var realController = reverseLookup.FirstOrDefault(x => x.Value == controller).Key ?? controller;
                var realAction = reverseLookup.FirstOrDefault(x => x.Value == action).Key ?? action;

                return ValueTask.FromResult(new RouteValueDictionary(new
                {
                    culture,
                    controller = realController,
                    action = realAction
                }));
            }

            return ValueTask.FromResult(values);
        }
    }
}
