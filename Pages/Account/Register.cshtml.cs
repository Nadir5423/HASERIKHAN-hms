using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;

namespace HMS.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly HotelDbContext _context;

        public RegisterModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Name { get; set; } = null!;

        [BindProperty]
        public string Email { get; set; } = null!;

        [BindProperty]
        public string Phone { get; set; } = null!;

        [BindProperty]
        public string? Address { get; set; }

        [BindProperty]
        public string Username { get; set; } = null!;

        [BindProperty]
        public string Password { get; set; } = null!;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields.";
                return Page();
            }

            // Check if email already exists
            if (await _context.Customers.AnyAsync(c => c.Email == Email))
            {
                ErrorMessage = "Email is already registered.";
                return Page();
            }

            // Check if username already exists
            if (await _context.Customers.AnyAsync(c => c.Username == Username))
            {
                ErrorMessage = "Username is already taken.";
                return Page();
            }

            var customer = new CUSTOMER
            {
                Name = Name,
                Email = Email,
                Phone = Phone,
                Address = Address,
                Username = Username,
                PasswordHash = Password, // Plain comparison to match script
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
}
