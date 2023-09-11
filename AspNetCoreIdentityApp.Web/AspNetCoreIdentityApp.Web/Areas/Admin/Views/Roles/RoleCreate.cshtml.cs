using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCoreIdentityApp.Repository.Models;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Views.Roles
{
    public class RoleCreateModel : PageModel
    {
        private readonly AspNetCoreIdentityApp.Repository.Models.AppDbContext _context;

        public RoleCreateModel(AspNetCoreIdentityApp.Repository.Models.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public AppRole AppRole { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Roles == null || AppRole == null)
            {
                return Page();
            }

            _context.Roles.Add(AppRole);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
