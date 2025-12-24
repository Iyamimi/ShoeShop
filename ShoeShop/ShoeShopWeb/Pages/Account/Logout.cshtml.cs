using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShoeShopWeb.Pages.Account
{
    public class Logout : PageModel
    {
        //Выход
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Products/Index");
        }
    }
}
