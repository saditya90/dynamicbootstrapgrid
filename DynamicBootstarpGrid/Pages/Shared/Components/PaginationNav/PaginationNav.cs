using Microsoft.AspNetCore.Mvc;

namespace DynamicBootstarpGrid.Pages.Shared.Components.PaginationNav
{
    public class PaginationNav : ViewComponent
    {
        public IViewComponentResult Invoke(Models.Pagination pagination)
        { 
            return View("Nav", pagination);
        }
    }
}
