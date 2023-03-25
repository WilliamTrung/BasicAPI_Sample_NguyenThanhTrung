using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eStoreWeb.Extension;
using eStoreWeb.Models;

namespace eStoreWeb.Pages.Members
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

        public IList<Member> Member { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Member = new List<Member>();
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.GetAsync(requestUri: "members");
                if (response.IsSuccessStatusCode)
                {
                    var members = await response.Content.ReadFromJsonAsync<List<Member>>();
                    //if (content != null)
                    //{
                    //var products = JsonSerializer.Deserialize<List<Product>>(content, jsonOption);
                    if (members != null)
                    {
                        Member = members;
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
    }
}
