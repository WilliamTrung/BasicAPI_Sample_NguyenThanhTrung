using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eStoreWeb.Extension;
using eStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace eStoreWeb.Pages.Customer
{
    public class OrderHistoryModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderHistoryModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Order= new List<Order>();
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var login = HttpContext.Session.GetLoginUser();
                if(login == null)
                {
                    return RedirectToPage("/Index");
                }
                var response = await client.GetAsync(requestUri: "orders/bymember-" + login.MemberId);
                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
                    if (orders != null)
                    {
                        Order = orders;
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
