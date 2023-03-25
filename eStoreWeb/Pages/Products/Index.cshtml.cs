using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using System.Text.Json;
using eStoreWeb.Extension;
using eStoreWeb.Models;

namespace eStoreWeb.Pages.Products
{
    public class IndexModel : PageModel
    {
        private HttpClient client;        
        private readonly IHttpClientFactory httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        public IList<Product> Products { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Products = new List<Product>();
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.GetAsync(requestUri: "products");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                    //if (content != null)
                    //{
                        //var products = JsonSerializer.Deserialize<List<Product>>(content, jsonOption);
                        if (products != null)
                        {
                            Products = products;
                        }
                    //}
                }
                else
                {
                    ViewData["Error"] = response.StatusCode;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task OnPostSearchAsync(string? name, double? price)
        {
            await OnGetAsync();
            if (name != null)
            {
                Products = Products.Where(p => p.ProductName.ToLower().Contains(name.ToLower())).ToList();
            }

            if (price != null && price > 0)
            {
                Products = Products.Where(p => p.UnitPrice <= (decimal)price).OrderByDescending(p => p.UnitPrice).ToList();
            }
        }
    }
}
