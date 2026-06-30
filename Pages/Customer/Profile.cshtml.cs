using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Customer
{
    public class ProfileModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public ProfileModel(HotelDbContext context)
        {
            _context = context;
        }

        public CUSTOMER Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Customer);
            if (authResult != null) return authResult;

            var customerId = CurrentUserId;
            if (!customerId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId.Value);

            if (customer == null)
            {
                return NotFound();
            }

            Customer = customer;
            return Page();
        }
    }
}
