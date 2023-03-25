using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eStoreWeb.Extension;
using eStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace eStoreWeb.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory= httpClientFactory;
            client = _httpClientFactory.CreateClient("BaseClient");
        }

        public Order Order { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.GetAsync(requestUri: "orders/"+id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var order = await response.Content.ReadFromJsonAsync<Order>();
                    if (order != null)
                    {
                        Order = order;
                    }
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
            return Page();
        }
    }
}
