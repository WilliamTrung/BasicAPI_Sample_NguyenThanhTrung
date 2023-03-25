using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eStoreWeb.Extension;
using eStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eStoreWeb.Pages.Customer
{
    public class ProfileEditModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileEditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        [BindProperty]
        public Member Member { get; set; } = default!;

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
                if (member != null)
                {
                    Member = member;
                    return Page();
                }
            }
            return RedirectToPage("/Index");
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
            var response = await client.PutAsJsonAsync("members/profile-" + Member.MemberId.ToString(), Member);
            if (response.IsSuccessStatusCode)
            {
                var login = HttpContext.Session.GetLoginUser();
                if(login != null)
                {
                    return RedirectToPage("./ProfileView", new { id = login.MemberId.ToString() });
                }
                else
                { 
                    return RedirectToPage("/Index"); 
                }
            }
            else
            {
                return Page();
            }
        }
    }
}
