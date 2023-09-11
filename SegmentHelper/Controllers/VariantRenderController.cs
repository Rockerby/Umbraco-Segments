using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace SegmentHelper.Controllers
{
    public class VariantRenderController : Umbraco.Cms.Web.Common.Controllers.RenderController
    {
        private readonly IVariationContextAccessor _variationContextAccessor;
        public VariantRenderController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor, IVariationContextAccessor variationContextAccessor)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            this._variationContextAccessor = variationContextAccessor;
        }

        public override IActionResult Index()
        {
            // This is where we change which segment is to be used.

            // We're using a simple query string check to test, but this
            // could be done using cookies or checking Member data

            if (HttpContext.Request.QueryString.HasValue)
            {
                // Feed the selected segment into the variation context which will trickle
                // down into all subsequent content requests
                _variationContextAccessor.VariationContext = new VariationContext(_variationContextAccessor.VariationContext.Culture,
                    HttpContext.Request.QueryString.Value.Replace("?", ""));
            }

            //if (this.User.Identity.IsAuthenticated && _variationContextAccessor.VariationContext != null)
            //{
            //    _variationContextAccessor.VariationContext = new VariationContext(_variationContextAccessor.VariationContext.Culture,
            //        "memberSegment");
            //}

            return base.Index();
        }
    }
}
