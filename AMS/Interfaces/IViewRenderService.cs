using Microsoft.AspNetCore.Mvc;

namespace AMS.Interfaces
{
    public interface IViewRenderService
    {
        Task<string> RenderViewAsync(ControllerContext context, string viewName, object model);

    }
}
