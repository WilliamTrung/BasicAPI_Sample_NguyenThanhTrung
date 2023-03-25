using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using BusinessObject.Models;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using eStoreAPI.Authorization;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;

namespace eStoreAPI.Controllers
{
    [CustomAuthorize]
    [Route("api/products")]
    [ApiController]
    public class ProductController : ODataController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Product
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var result = await _unitOfWork.ProductRepository.Get();
            return Ok(result);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var find = await _unitOfWork.ProductRepository.Get(predicate: p => p.ProductId == id, includeProperties: "Category");
            var found = find.FirstOrDefault();
           
            if (found == null)
            {
                return NotFound();
            }

            return found;
        }
        [CustomAuthorize("Administrator")]
        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            //_context.Entry(product).State = EntityState.Modified;

            try
            {
                var find = await _unitOfWork.ProductRepository.GetById(id);
                if (find != null)
                {
                    await _unitOfWork.ProductRepository.Update(product);
                }
                else
                {
                    return NotFound(id);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }
        [CustomAuthorize("Administrator")]
        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (await _unitOfWork.ProductRepository.GetById(product.ProductId) == null)
            {
                await _unitOfWork.ProductRepository.AddAsync(product);
                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }
        }
        [CustomAuthorize("Administrator")]
        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var find = await _unitOfWork.ProductRepository.GetById(id);
            if (find == null)
            {
                return NotFound();
            }

            await _unitOfWork.ProductRepository.Delete(find);
            return NoContent();
        }
    }
}
