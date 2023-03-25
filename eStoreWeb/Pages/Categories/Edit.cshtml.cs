using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using eStoreWeb.Extension;
using eStoreWeb.Models;

namespace eStoreWeb.Pages.Categories
{
    public class EditModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory= httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
            
        }

        [BindProperty]
        public Category? Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.GetAsync("categories/" + ((int)id).ToString());
            if(response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                Category = await response.Content.ReadFromJsonAsync<Category>();
                return Page();
            }
            return RedirectToPage("./Index");
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.PutAsJsonAsync("categories/"+Category.CategoryId.ToString(), Category);
            if(response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            } else
            {
                return Page();
            }           
            
        }
    }
}
