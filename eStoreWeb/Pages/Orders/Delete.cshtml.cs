using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eStoreWeb.Extension;
using eStoreWeb.Models;

namespace eStoreWeb.Pages.Orders
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
      public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }            
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.GetAsync(requestUri: "orders/" + id.ToString());
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.DeleteAsync("orders/" + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    //delete successfully!
                }
                else
                {
                    ViewData["Error"] = response.StatusCode;
                    return Page();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToPage("./Index");
        }
    }
}
