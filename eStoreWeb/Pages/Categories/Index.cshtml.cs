using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using eStoreWeb.Extension;
using eStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace eStoreWeb 
{ 
    public class IndexModel : PageModel
    {
        private HttpClient client;
        private readonly JsonSerializerOptions jsonOption;
        private readonly IHttpClientFactory httpClientFactory;
        public IndexModel(JsonSerializerOptions jsonOption, IHttpClientFactory httpClientFactory)
        {
            this.jsonOption = jsonOption;
            this.httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        public IList<Category> Category { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Category = new List<Category>();
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.GetAsync(requestUri: "categories");
                if(response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<List<Category>>();
                    if(content != null)
                    {
                        //var categories = JsonSerializer.Deserialize<List<Category>>(content, jsonOption);
                        //if(categories != null)
                        //{
                            Category = content;
                        //}
                    }
                } else
                {
                    ViewData["Error"] = response.StatusCode;
                }

            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
    }
}
