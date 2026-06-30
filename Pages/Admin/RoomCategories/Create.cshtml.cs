using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HMS.Core;
using HMS.Models.Entities;

namespace HMS.Pages.Admin.RoomCategories
{
    public class CreateModel : BasePageModel
    {
        private readonly ICrudService<ROOM_CATEGORY> _service;

        public CreateModel(ICrudService<ROOM_CATEGORY> service)
        {
            _service = service;
        }

        [BindProperty]
        public ROOM_CATEGORY Category { get; set; } = new();

        public IActionResult OnGet()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Category.CreatedAt = DateTime.Now;
            Category.UpdatedAt = DateTime.Now;

            await _service.CreateAsync(Category);
            TempData["SuccessMessage"] = "Category created successfully!";
            return RedirectToPage("./Index");
        }
    }
}
