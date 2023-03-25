using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using eStoreWeb.Extension;
using eStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eStoreWeb.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("BaseClient");
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Order= new List<Order>();
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.GetAsync(requestUri: "orders");
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
        }
        public async Task OnPostStatisticAsync(DateTime? startdate, DateTime? enddate)
        {
            Order = new List<Order>();
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                if(enddate != null && startdate != null && enddate < startdate)
                {
                    var temp = startdate;
                    startdate = enddate;
                    enddate = temp;
                }
                var value = new Dictionary<string, string>()
                {
                     {"startdate", startdate==null?"":startdate.ToString() },
                    {"enddate", enddate==null?"":enddate.ToString() }
                };
                if(startdate != null)
                {
                    ViewData["startdate"] = startdate;
                } else if(enddate != null)
                {
                    ViewData["startdate"] = "Today";
                }
                if(enddate != null)
                {
                    ViewData["enddate"] = enddate;
                } else if(startdate != null)
                {
                    ViewData["enddate"] = "Today";
                }
                
                var response = await client.GetAsync("orders/statistic" + HttpRequestSupport.GetQueryPath(value));
                var response_total = await client.GetAsync("orders/statistic-value" + HttpRequestSupport.GetQueryPath(value));
                if (response.IsSuccessStatusCode && response_total.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
                    if (orders != null)
                    {
                        Order = orders;
                    }
                    decimal total = Math.Round(Convert.ToDecimal(await response_total.Content.ReadAsStringAsync()), 0); //Decimal.Parse(await response_total.Content.ReadAsStringAsync());                    
                    ViewData["sales"] = total;
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
