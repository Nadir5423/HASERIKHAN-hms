using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Services
{
    public class EditModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public EditModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SERVICE Service { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Admins and Managers can edit services
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager);
            if (authResult != null) return authResult;

            Service = await _context.Services.FindAsync(id);
            if (Service == null)
            {
                TempData["ErrorMessage"] = "Service not found.";
                return RedirectToPage("/Admin/Services/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager);
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if service name already exists
            if (await _context.Services.AnyAsync(s => s.ServiceName == Service.ServiceName && s.ServiceId != Service.ServiceId && !s.ServiceName.Contains("_Del_")))
            {
                ModelState.AddModelError("Service.ServiceName", "Service name already exists.");
                return Page();
            }

            // Guard: check if the service is currently in use by any customer (active bookings)
            var isUsing = await _context.BookingServices.AnyAsync(bs => 
                bs.ServiceId == Service.ServiceId && 
                bs.Booking!.BookingStatus != Constants.BookingStatus.Cancelled && 
                bs.Booking!.BookingStatus != Constants.BookingStatus.CheckedOut);
            if (isUsing)
            {
                var dialog = new HMS.Models.ViewModels.DialogModel
                {
                    Title = "Active Bookings Exist",
                    Message = "This service is currently in use by a customer. You cannot update the service properties or availability while it is in use.",
                    ConfirmButtonText = "OK",
                    ShowCancel = false
                };
                TempData["DialogModel"] = dialog;
                return Page();
            }

            // Apply updates
            _context.Attach(Service).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Service updated successfully!";
            return RedirectToPage("/Admin/Services/Index");
        }
    }
}
