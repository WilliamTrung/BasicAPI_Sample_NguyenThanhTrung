using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eStoreWeb.Models;
using eStoreWeb.Extension;

namespace eStoreWeb.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private HttpClient client;
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = _httpClientFactory.CreateClient("BaseClient");
        }
        private async Task InitAsync()
        {
            await GetProductsAsync();
            await GetMembersAsync();
            InitOrder();
        }
        private void InitOrder()
        {
            if(Order == null)
            {
                Order = new Order()
                {
                    OrderDate = DateTime.Today,
                    RequiredDate = DateTime.Today
                };
            } else
            {
                Order.OrderDate= DateTime.Today;
                Order.RequiredDate= DateTime.Today;
            }
            
        }
        private async Task GetMembersAsync()
        {
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
                        ViewData["MemberId"] = new SelectList(members, "MemberId", "Email");
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
        private async Task GetProductsAsync()
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
        private async Task<Product?> GetProductByIdAsync(int id)
        {
            Products = new List<Product>();
            try
            {
                client.AddTokenHeader(HttpContext.Session.GetString("token"));
                var response = await client.GetAsync(requestUri: "products/" + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<Product>();
                    if (product != null)
                    {
                        return product;
                    }
                }
                else
                {
                    ViewData["Error"] = response.StatusCode;
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            await InitAsync();
            var order = HttpContext.Session.GetOrder();
            if(order != null)
            {
                Order = order;
            }
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = new Order()
        {
            OrderDate = DateTime.Today,      
            RequiredDate = DateTime.Today,
            OrderDetails = new List<OrderDetail>()
        };
        private bool CheckValidDate()
        {
            bool valid = true ;
            string error = "";
            if(Order.RequiredDate < Order.OrderDate)
            {
                valid = false;
                error += "Required Date must not before Order Date!";
            }
            if(Order.ShippedDate < Order.OrderDate)
            {
                valid = false;
                error += "\nShip Date must after Order Date!";
            }
            if(Order.ShippedDate > Order.RequiredDate)
            {
                valid= false;
                error += "\nShip Date must before Required Date!";
            }
            if(!valid)
            {
                ViewData["Error"] = error;
            }
            return valid;
        }
        public List<Product> Products { get; set; } = null!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostCreateAsync()
        {
          if (!ModelState.IsValid && ModelState.ErrorCount > 1)
            {
                return Page();
            }
          if(CheckValidDate())
            {
                var order = HttpContext.Session.GetOrder();
                if (order != null && order.OrderDetails != null && order.OrderDetails.Count > 0)

                {
                    Order.OrderDetails = order.OrderDetails;
                    client.AddTokenHeader(HttpContext.Session.GetString("token"));
                    var response = await client.PostAsJsonAsync("orders", Order);
                    if (response.IsSuccessStatusCode)
                        return RedirectToPage("./Index");
                }
                else
                {
                    ViewData["Error"] = "No detail is added to this order!";
                }
                
            } 
            await InitAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostAddAsync(int id, int quantity, int? discount)
        {
            var product = await GetProductByIdAsync(id);
            var order = HttpContext.Session.GetOrder();
            if (order == null)
            {
                order = Order;
                order.OrderDetails = new List<OrderDetail>();
            }
            if (product == null)
            {
                ViewData["Error"] = "Product not found!";
                return Page();
            }
            //check condition
            var isValid = false;
            var isExist = false;
            var detail = new OrderDetail()
            {
                Discount = discount,
                ProductId = id,
                Quantity = quantity,
                UnitPrice = product.UnitPrice,
                Product = product,
            };
            if (order.OrderDetails.Any(d => d.ProductId == id))
            {
                isExist= true;
                var _detail = order.OrderDetails.First(d => d.ProductId == id);
                detail.Quantity += _detail.Quantity;
            }
            if(detail.Quantity <= product.UnitsInStock)
            {
                isValid= true;
            } else
            {
                ViewData["Error"] = ViewData["Error"] + "\nQuantity is at maximum!";
            }
            if (isValid)
            {
                if(isExist)
                {
                    var _detail = order.OrderDetails.First(d => d.ProductId == id);
                    _detail.Quantity+=quantity;
                    _detail.UnitPrice = detail.UnitPrice;
                } else
                {
                    order.OrderDetails.Add(detail);
                }
                
                HttpContext.Session.SetOrder(order);
            }
            Order = order;
            await InitAsync();
            return Page();
        }
        public async Task OnPostRemoveAsync(int id)
        {
            var order = HttpContext.Session.GetOrder();
            if (order != null)
            {
                var find = order.OrderDetails.FirstOrDefault(o => o.ProductId == id);
                if (find != null)
                {
                    order.OrderDetails.Remove(find);
                }
                HttpContext.Session.SetOrder(order);
            } else
            {
                order = Order;
                order.OrderDetails = new List<OrderDetail>();
            }

            Order = order;
            await InitAsync();
        }
    }
    
}
