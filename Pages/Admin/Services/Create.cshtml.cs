using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Services
{
    public class CreateModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public CreateModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SERVICE Service { get; set; } = new();

        public IActionResult OnGet()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Only Admins can add services
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if service name already exists
            if (await _context.Services.AnyAsync(s => s.ServiceName == Service.ServiceName && !s.ServiceName.Contains("_Del_")))
            {
                ModelState.AddModelError("Service.ServiceName", "Service name already exists.");
                return Page();
            }

            _context.Services.Add(Service);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Service added successfully!";
            return RedirectToPage("/Admin/Services/Index");
        }
    }
}
