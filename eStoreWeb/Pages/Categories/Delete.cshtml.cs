using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using BusinessObject.Models;
using System.Net.Http;
using eStoreWeb.Extension;

namespace eStoreWeb.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        [BindProperty]
        public Category? Category { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.GetAsync("categories/" + ((int)id).ToString());
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                Category = await response.Content.ReadFromJsonAsync<Category>();
                return Page();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.DeleteAsync("categories/" + Category.CategoryId.ToString());
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                return Page();
            }
        }
    }
}
