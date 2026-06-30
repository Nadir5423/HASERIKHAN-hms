using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HMS.Core;
using HMS.Models.Entities;

namespace HMS.Pages.Admin.RoomCategories
{
    public class IndexModel : BasePageModel
    {
        private readonly ICrudService<ROOM_CATEGORY> _service;

        public IndexModel(ICrudService<ROOM_CATEGORY> service)
        {
            _service = service;
        }

        public List<ROOM_CATEGORY> RoomCategories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            RoomCategories = await _service.GetAllAsync();
            return Page();
        }
    }
}
