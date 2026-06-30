using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Data;

namespace HMS.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HotelDbContext _context;

        public LoginModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; } = null!;

        [BindProperty]
        public string Password { get; set; } = null!;

        [BindProperty]
        public string UserType { get; set; } = "Customer"; // "Customer" or "Staff"

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var type = HttpContext.Session.GetString("UserType");
                if (type == "Customer")
                    return RedirectToPage("/Customer/Index");
                else
                    return RedirectToPage("/Admin/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Username and Password are required.";
                Username = "";
                Password = "";
                return Page();
            }

            if (UserType == "Customer")
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Username == Username && c.IsActive);

                if (customer != null && customer.PasswordHash == Password) // Matches sql script plain text
                {
                    HttpContext.Session.SetString("UserId", customer.CustomerId.ToString());
                    HttpContext.Session.SetString("UserName", customer.Name);
                    HttpContext.Session.SetString("UserRole", "Customer");
                    HttpContext.Session.SetString("UserType", "Customer");

                    TempData["SuccessMessage"] = $"Welcome back, {customer.Name}!";
                    return RedirectToPage("/Customer/Index");
                }
            }
            else // Staff
            {
                var staff = await _context.HotelStaffs
                    .FirstOrDefaultAsync(s => s.Username == Username && s.IsActive);

                if (staff != null && staff.PasswordHash == Password) // Matches sql script plain text
                {
                    HttpContext.Session.SetString("UserId", staff.StaffId.ToString());
                    HttpContext.Session.SetString("UserName", staff.Name);
                    HttpContext.Session.SetString("UserRole", staff.Role);
                    HttpContext.Session.SetString("UserType", "Staff");

                    TempData["SuccessMessage"] = $"Welcome back, {staff.Name}!";
                    return RedirectToPage("/Admin/Index");
                }
            }

            ErrorMessage = "Invalid username or password.";
            Username = "";
            Password = "";
            return Page();
        }
    }
}
