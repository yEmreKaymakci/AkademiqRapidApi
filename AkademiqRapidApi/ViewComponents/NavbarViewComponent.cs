using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
