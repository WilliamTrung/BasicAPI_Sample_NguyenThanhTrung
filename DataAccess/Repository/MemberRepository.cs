using BusinessObject;
using BusinessObject.Models;
using DataAccess.IRepository;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        public MemberRepository(eStoreDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
        }

        public async Task<Member?> LoginAsync(string email, string password)
        {
            var find = await Get(predicate: m => m.Email== email && m.Password == password);
            var found = find.FirstOrDefault();
            return found;
        }
        public override async Task<Member?> GetById(int id)
        {
            var find = await Get(predicate: m => m.MemberId == id);
            var found = find.FirstOrDefault();
            return found;
        }
    }
}
