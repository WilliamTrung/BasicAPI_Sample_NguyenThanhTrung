using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObject;
using eStoreWeb.Models;
using eStoreWeb.Extension;
using Microsoft.EntityFrameworkCore;

namespace eStoreWeb.Pages.Products
{
    public class CreateModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        private List<Category> _categories = new List<Category>();
        private async Task GetCategoriesAsync()
        {
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.GetAsync(requestUri: "categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<Category>>();
                if (content != null)
                {
                    //var categories = JsonSerializer.Deserialize<List<Category>>(content, jsonOption);
                    //if(categories != null)
                    //{
                    _categories = content;
                    //}
                }
            }
            ViewData["CategoryId"] = new SelectList(_categories, "CategoryId", "CategoryName");
        }
        public async Task<IActionResult> OnGetAsync()
        {
            await GetCategoriesAsync();                        
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = null!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.PostAsJsonAsync("products", Product);
            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Index");
            return Page();
        }
    }
}
