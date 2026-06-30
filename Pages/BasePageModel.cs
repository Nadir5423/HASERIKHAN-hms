using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace HMS.Pages
{
    public abstract class BasePageModel : PageModel
    {
        public bool IsLoggedIn => HttpContext.Session.GetString("UserId") != null;
        public string CurrentUserRole => HttpContext.Session.GetString("UserRole") ?? "";
        public string CurrentUserType => HttpContext.Session.GetString("UserType") ?? ""; // "Customer" or "Staff"
        public string CurrentUserName => HttpContext.Session.GetString("UserName") ?? "";
        public int? CurrentUserId
        {
            get
            {
                var idStr = HttpContext.Session.GetString("UserId");
                return idStr != null ? int.Parse(idStr) : null;
            }
        }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        protected IActionResult? EnsureAuthorized(params string[] allowedRoles)
        {
            if (!IsLoggedIn)
            {
                return RedirectToPage("/Account/Login");
            }

            if (allowedRoles.Length > 0 && !allowedRoles.Contains(CurrentUserRole))
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            return null;
        }

        protected IActionResult? EnsureUserType(string userType)
        {
            if (!IsLoggedIn)
            {
                return RedirectToPage("/Account/Login");
            }

            if (CurrentUserType != userType)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            return null;
        }
    }
}
