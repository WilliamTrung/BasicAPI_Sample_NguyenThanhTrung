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
using System.ComponentModel.DataAnnotations;

namespace eStoreAPI.Controllers
{
    [CustomAuthorize]
    [Route("api/members")]
    [ApiController]
    public class MemberController : ODataController
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Member
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var result = await _unitOfWork.MemberRepository.Get();
            return Ok(result);
        }

        // GET: api/Member/5
        [CustomAuthorize("Member","Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var find = await _unitOfWork.MemberRepository.Get(predicate: m => m.MemberId == id);
            var found = find.FirstOrDefault();

            if (found == null)
            {
                return NotFound();
            }

            return found;
        }
        [CustomAuthorize("Member")]
        // PUT: api/Member/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("profile-{id}")]
        public async Task<IActionResult> PutMemberProfile(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }
            try
            {
                var find = await _unitOfWork.MemberRepository.GetById(id);
                if (find != null)
                {
                    await _unitOfWork.MemberRepository.Update(member);
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
        // PUT: api/Member/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }

            //_context.Entry(product).State = EntityState.Modified;

            try
            {
                var find = await _unitOfWork.MemberRepository.GetById(id);
                if (find != null)
                {
                    await _unitOfWork.MemberRepository.Update(member);
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
        // POST: api/Member
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember([Bind("Email,CompanyName")]Member member)
        {
            var find = await _unitOfWork.MemberRepository.Get(predicate: m => m.Email == member.Email);
            if (find == null || find.Count() <= 0)
            {
                await _unitOfWork.MemberRepository.AddAsync(member);
                return CreatedAtAction("GetMember", new { id = member.MemberId }, member);
            }
            return StatusCode(StatusCodes.Status409Conflict);

        }
        [CustomAuthorize("Administrator")]
        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var find = await _unitOfWork.MemberRepository.GetById(id);
            if (find == null)
            {
                return NotFound();
            }

            await _unitOfWork.MemberRepository.Delete(find);
            return NoContent();
        }
    }
}
