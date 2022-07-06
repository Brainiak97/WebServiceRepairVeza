using AutoMapper;
using BLL.Models;
using BLL.Services.Generic;
using Core.Models;
using DAL.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class UserService : GenericService<User, UserDto>
    {
        private readonly IRepository<RepairGroup> _repositoryRepairGroup;
        private readonly UserManager<User> _userManager;

        public UserService(IMapper mapper, IRepository<User> repository, IRepository<RepairGroup> repositoryRepairGroup,
                           IValidator<User> validator, UserManager<User> userManager)
           : base(mapper, repository, validator)
        {
            _userManager = userManager;
            _repositoryRepairGroup = repositoryRepairGroup;
        }

        public async Task<IdentityResult> Create(UserDto item, string password)
        {
            var user = _mapper.Map<User>(item);
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<UserDto?> GetUserWithRepairGroups(int userId)
        {
            return _mapper.Map<UserDto>(await _repository
                .GetQuery()
                .Include(user => user.RepairGroups)
                .FirstOrDefaultAsync(user => user.Id == userId));
        }

        public async Task<UserDto> GetUserDetails(int userId)
        {
            return _mapper.Map<UserDto>(await _repository
                .GetQuery()
                .Include(user => user.RepairGroups)
                .Include(user => user.LogAuthors)
                .Include(user => user.LogExecutors)
                .FirstOrDefaultAsync(user => user.Id == userId));
        }

        public async Task<IEnumerable<UserDto>?> GetUsersDetails()
        {
            return _mapper.Map<IEnumerable<UserDto>?>(await _repository
                .GetQuery()
                .Include(user => user.RepairGroups)
                .Include(user => user.LogAuthors)
                .Include(user => user.LogExecutors)
                .ToListAsync());
        }

        public async Task<IEnumerable<UserDto>?> GetEmployersByRepairGroupsIds(List<int> userGroupsId)
        {
            var users = await _repository
                .GetQuery()
                .Include(user => user.RepairGroups)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users.Where(user => user.RepairGroups != null
            && user.RepairGroups.Select(rg => rg.Id).Intersect(userGroupsId).Any()
            && _userManager.IsInRoleAsync(user, "employee").Result));
        }

        public async Task<IEnumerable<int>?> GetUsersIdsByRepairGroupsIds(List<int> repairGroupsId)
        {
            var qwe = await _repository
                .GetQuery()
                .Include(user => user.RepairGroups)
                .ToListAsync();
            return qwe.Where(user => user.RepairGroups != null && user.RepairGroups.Select(rg => rg.Id).Intersect(repairGroupsId).Any()).Select(user => user.Id);
        }

        public async Task EditGroupsIntoUserAsync(int userId, IEnumerable<string> groups)
        {
            var user = await _repository
                .GetQuery()
                .Include(user => user.RepairGroups)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (user != null && user.RepairGroups != null)
            {
                var addedGroups = groups.Except(user.RepairGroups.Select(usergroup => usergroup.Name));

                var removedGroups = user.RepairGroups.Select(usergroup => usergroup.Name).Except(groups);

                foreach (var gr in _repositoryRepairGroup.GetQuery())
                {
                    if (addedGroups.Any(ad => ad == gr.Name))
                        user.RepairGroups.Add(gr);
                    else if (removedGroups.Any(rem => rem == gr.Name))
                        user.RepairGroups.Remove(gr);
                }

                await _repository.Update(user);
            }
        }

        public async Task<IdentityResult> SetNewPasswordAsync(string userName, string newPassword)
        {
            var targetUser = await _userManager.FindByNameAsync(userName);

            targetUser.PasswordHash = _userManager.PasswordHasher.HashPassword(targetUser, newPassword);
            return await _userManager.UpdateAsync(targetUser);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var targetUser = await _userManager.FindByNameAsync(userName);
            return await _userManager.ChangePasswordAsync(targetUser, oldPassword, newPassword);
        }

        public async Task<IdentityResult> UpdateUser(UserDto user)
        {
            var targetUser = await _userManager.FindByNameAsync(user.UserName);
            targetUser.UserName = user.UserName;
            targetUser.Name = user.Name;
            targetUser.SurName = user.SurName;
            targetUser.MiddleName = user.MiddleName;
            targetUser.PhoneNumber = user.PhoneNumber;

            return await _userManager.UpdateAsync(targetUser);
        }

        public async Task<IList<string>> GetUserRoles(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return await _userManager.GetRolesAsync(user);
        }
    }
}
