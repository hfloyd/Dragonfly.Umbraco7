namespace Dragonfly.UmbracoModels.MvcFakes
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class FakeController : ControllerBase
    {
        protected override void Execute(RequestContext requestContext)
        {
            throw new InvalidOperationException();
        }

        protected override void ExecuteCore()
        {
            throw new InvalidOperationException();
        }
    }
}
