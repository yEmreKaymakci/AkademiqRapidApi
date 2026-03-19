using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
