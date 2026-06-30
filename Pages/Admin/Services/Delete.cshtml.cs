using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Services
{
    public class DeleteModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public DeleteModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SERVICE Service { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
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
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            var service = await _context.Services.FindAsync(Service.ServiceId);
            if (service == null)
            {
                TempData["ErrorMessage"] = "Service not found.";
                return RedirectToPage("/Admin/Services/Index");
            }

            // Guard: check if the service is currently in use by any customer (active bookings)
            var isUsing = await _context.BookingServices.AnyAsync(bs => 
                bs.ServiceId == service.ServiceId && 
                bs.Booking!.BookingStatus != Constants.BookingStatus.Cancelled && 
                bs.Booking!.BookingStatus != Constants.BookingStatus.CheckedOut);
            if (isUsing)
            {
                var dialog = new HMS.Models.ViewModels.DialogModel
                {
                    Title = "Active Bookings Exist",
                    Message = "This service is currently in use by a customer. Please cancel the booking(s) first before deleting the service.",
                    ConfirmButtonText = "OK",
                    ShowCancel = false
                };
                TempData["DialogModel"] = dialog;
                return Page();
            }

            // Perform soft-delete
            service.IsAvailable = false;
            
            string deletedSuffix = $"_Del_{service.ServiceId}";
            int maxNameLength = 100 - deletedSuffix.Length;
            if (service.ServiceName.Length > maxNameLength)
            {
                service.ServiceName = service.ServiceName.Substring(0, maxNameLength) + deletedSuffix;
            }
            else
            {
                service.ServiceName = service.ServiceName + deletedSuffix;
            }

            _context.Entry(service).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Service deleted successfully!";
            return RedirectToPage("/Admin/Services/Index");
        }
    }
}
