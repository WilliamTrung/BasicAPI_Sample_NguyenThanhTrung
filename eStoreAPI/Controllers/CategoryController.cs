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
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using eStoreAPI.Authorization;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;

namespace eStoreAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ODataController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [CustomAuthorize("Member","Administrator")]
        // GET: api/Category
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var result = await _unitOfWork.CategoryRepository.Get();            
            return Ok(result);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var find = await _unitOfWork.CategoryRepository.Get(predicate: c => c.CategoryId == id);
            var found = find.FirstOrDefault();
            if (found == null)
            {
                return NotFound();
            }
            return found;
        }
        [CustomAuthorize("Administrator")]
        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }
            try
            {
                var find = await _unitOfWork.CategoryRepository.GetById(id);
                if(find != null)
                {
                    await _unitOfWork.CategoryRepository.Update(category);
                } else
                {
                    return NotFound(id);
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return NoContent();
        }
        [CustomAuthorize("Administrator")]
        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            var find = await _unitOfWork.CategoryRepository.Get(predicate: c => c.CategoryName.ToLower() == category.CategoryName.ToLower());
            if (find == null || find.Count() <= 0)
            {
                await _unitOfWork.CategoryRepository.AddAsync(category);
                return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
            } else
            {
               return StatusCode(StatusCodes.Status409Conflict);
            }
        }
        [CustomAuthorize("Administrator")]
        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var find = await _unitOfWork.CategoryRepository.GetById(id);
            if (find == null)
            {
                return NotFound();
            }

            await _unitOfWork.CategoryRepository.Delete(find);
            return NoContent();
        }

    }
}
