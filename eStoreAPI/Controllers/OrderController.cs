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
using eStoreAPI.Authorization;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;

namespace eStoreAPI.Controllers
{

    [Route("api/orders")]
    [ApiController]
    public class OrderController : ODataController
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [CustomAuthorize("Administrator")]
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var result = await _unitOfWork.OrderRepository.Get(includeProperties: "Member");
            return Ok(result);
        }
        [HttpGet("statistic")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(DateTime? startdate, DateTime? enddate)
        {
            var result = await _unitOfWork.OrderRepository.Get(includeProperties: "Member");
            result.OrderByDescending(o => o.OrderDate);
            if(startdate != null)
            {
                result = result.Where(o => o.OrderDate >= startdate);
            }
            if(enddate != null)
            {
                result = result.Where(o => o.OrderDate <= enddate);
            }
            return Ok(result);
        }
        [HttpGet("statistic-value")]
        public async Task<ActionResult<decimal>> GetStatisticValue(DateTime? startdate, DateTime? enddate)
        {
            var result = await _unitOfWork.OrderRepository.Get(includeProperties: "Member");
            result.OrderByDescending(o => o.OrderDate);
            if (startdate != null)
            {
                result = result.Where(o => o.OrderDate >= startdate);
            }
            if (enddate != null)
            {
                result = result.Where(o => o.OrderDate <= enddate);
            }
            decimal calc = 0;
            foreach(var order in result)
            {
                calc += _unitOfWork.OrderRepository.Total(order);
            }
            return Ok(calc);
        }
        [CustomAuthorize("Administrator","Member")]
        [HttpGet("bymember-{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByMember(int id)
        {
            var result = await _unitOfWork.OrderRepository.Get(predicate: o => o.MemberId ==id, includeProperties: "Member");
            return Ok(result);
        }

        [CustomAuthorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var find = await _unitOfWork.OrderRepository.Get(predicate: p => p.OrderId == id, includeProperties: "Member");
            var found = find.FirstOrDefault();

            if (found == null)
            {
                return NotFound();
            }

            return found;
        }
        [CustomAuthorize]
        // PUT: api/Order/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            //_context.Entry(product).State = EntityState.Modified;

            try
            {
                var find = await _unitOfWork.OrderRepository.GetById(id);
                if (find != null)
                {
                    await _unitOfWork.OrderRepository.Update(order);
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
        [CustomAuthorize]
        // POST: api/Order
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder([Bind("ShippedDate")]Order order)
        {
            await _unitOfWork.OrderRepository.AddAsync(order);
            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }
        [CustomAuthorize("Administrator")]
        // DELETE: api/Order/5
        [HttpDelete()]
        public async Task<IActionResult> DeleteOrder([FromQuery]int id)
        {
            var find = await _unitOfWork.OrderRepository.GetById(id);
            if (find == null)
            {
                return NotFound();
            }

            await _unitOfWork.OrderRepository.Delete(find);
            return NoContent();
        }
    }
}
