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
        public Member Member { get; set; }

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
                if(member!= null)
                {
                    Member = member;
                    return Page();
                }                
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            client.AddTokenHeader(HttpContext.Session.GetString("token"));
            var response = await client.DeleteAsync("members/" + Member.MemberId.ToString());
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
