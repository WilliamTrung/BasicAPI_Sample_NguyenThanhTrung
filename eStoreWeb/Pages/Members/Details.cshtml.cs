using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eStoreWeb.Extension;
using eStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace eStoreWeb.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        public Member Member { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.GetAsync("members/" + ((int)id).ToString());
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                var member = await response.Content.ReadFromJsonAsync<Member>();
                if(member != null)
                {
                    Member = member;
                    return Page();
                }
            }
            return RedirectToPage("./Index");
        }
    }
}
