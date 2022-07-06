using AutoMapper;
using BLL.Models;
using BLL.Services.Generic;
using Core.Models;
using DAL.Repository;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BLL.Services
{
    public class RepairGroupService : GenericService<RepairGroup, RepairGroupDto>
    {
        public RepairGroupService(IMapper mapper, IRepository<RepairGroup> repository,
                           IValidator<RepairGroup> validator)
           : base(mapper, repository, validator)
        {
        }

        public async Task<IEnumerable<int>?> GetUserGroupsId(ClaimsPrincipal user)
        {
            var userId = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var groups = await _repository
                .GetQuery()
                .Include(_ => _.Users)
                .ToListAsync();
            return groups.Where(g => g.Users!.Select(u => u.Id).Contains(userId)).Select(sg => sg.Id);
        }

        public async Task<IEnumerable<RepairGroupDto>?> GetUserGroups(int userId)
        {
            var groups = await _repository
                .GetQuery()
                .Include(_ => _.Users)
                .ToListAsync();
            return _mapper.Map<IEnumerable<RepairGroupDto>>(groups.Where(g => g.Users!.Select(u => u.Id).Contains(userId)));
        }
    }
}
