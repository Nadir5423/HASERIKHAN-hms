using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Customer
{
    public class EditProfileModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public EditProfileModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CUSTOMER Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Customer);
            if (authResult != null) return authResult;

            var customerId = CurrentUserId;
            if (!customerId.HasValue) return RedirectToPage("/Account/Login");

            var customer = await _context.Customers.FindAsync(customerId.Value);
            if (customer == null) return NotFound();

            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Customer);
            if (authResult != null) return authResult;

            var customerId = CurrentUserId;
            if (!customerId.HasValue) return RedirectToPage("/Account/Login");

            // Remove validation for fields we don't edit
            ModelState.Remove("Customer.PasswordHash");
            ModelState.Remove("Customer.Username");
            ModelState.Remove("Customer.Email");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var customerToUpdate = await _context.Customers.FindAsync(customerId.Value);
            if (customerToUpdate == null) return NotFound();

            customerToUpdate.Name = Customer.Name;
            customerToUpdate.Phone = Customer.Phone;
            customerToUpdate.Address = Customer.Address;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToPage("./Profile");
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = ex.GetBaseException().Message;
                return Page();
            }
        }
    }
}
