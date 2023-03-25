using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using System.Net.Http;
using eStoreWeb.Extension;
using eStoreWeb.Models;

namespace eStoreWeb.Pages.Products
{
    public class EditModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        [BindProperty]
        public Product? Product { get; set; } = default!;
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
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.GetAsync("products/" + ((int)id).ToString());
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                Product = await response.Content.ReadFromJsonAsync<Product>();
                await GetCategoriesAsync();
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
            var response = await client.PutAsJsonAsync("products/" + Product.ProductId.ToString(), Product);
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
