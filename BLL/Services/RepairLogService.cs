using AutoMapper;
using BLL.Models;
using BLL.Services.Generic;
using Core.Models;
using DAL.Repository;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Security.Claims;

namespace BLL.Services
{
    [DisallowConcurrentExecution]
    public class RepairLogService : GenericService<RepairLog, RepairLogDto>
    {
        private readonly IRepository<RepairGroup> _repositoryRepairGroup;
        private readonly IRepository<User> _repositoryUser;

        public RepairLogService(IMapper mapper, IRepository<RepairLog> repository, IRepository<RepairGroup> repositoryRepairGroup, IRepository<User> repositoryUser,
                           IValidator<RepairLog> validator)
           : base(mapper, repository, validator)
        {
            _repositoryRepairGroup = repositoryRepairGroup;
            _repositoryUser = repositoryUser;
        }

        public async Task<IEnumerable<RepairLogDto>> GetItems_RepairGroups()
        {
            return _mapper.Map<IEnumerable<RepairLogDto>>(await _repository
                .GetQuery()
                .Include(_ => _.RepairGroups)
                .ToListAsync());
        }

        public async Task<IEnumerable<RepairLogDto>?> GetCorrespondingLogs(IEnumerable<int>? userRepairGroups)
        {
            if (userRepairGroups != null)
            {
                var logs = _mapper.Map<IEnumerable<RepairLogDto>>(await _repository
                                .GetQuery()
                                .Include(_ => _.RepairGroups)
                                .ToListAsync());
                return logs.Where(item => item.RepairGroups.Select(gr => gr.Id).Intersect(userRepairGroups).Any());
            }

            return null;
        }
        public async Task<IEnumerable<RepairLogDto>?> GetCorrespondingLogs(ClaimsPrincipal user)
        {
            if (user != null)
            {
                var userId = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var groups = await _repositoryRepairGroup
                    .GetQuery()
                    .Include(_ => _.Users)
                    .Where(g => g.Users!.Select(u => u.Id).Contains(userId)).Select(sg => sg.Id)
                    .ToListAsync();

                var logs = _mapper.Map<IEnumerable<RepairLogDto>>(await _repository
                                .GetQuery()
                                .Include(_ => _.RepairGroups)
                                .ToListAsync());
                return logs.Where(item => item.RepairGroups.Select(gr => gr.Id).Intersect(groups).Any() || item.AuthorId == userId);
            }

            return null;
        }

        public async Task<RepairLogDto> GetItem_RepairGroups_Comments_Author(int id)
        {
            return _mapper.Map<RepairLogDto>(await _repository
                .GetQuery()
                .Include(items => items.RepairGroups)
                .Include(items => items.Comments)
                .Include(items => items.Author)
                .Include(items => items.Executors)
                .FirstOrDefaultAsync(log => log.Id == id));
        }

        public async Task<int> AddCommentToLog(int logId, CommentDto comment)
        {
            var log = await _repository
                .GetQuery()
                .Include(log => log.Comments)
                .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                var commentMap = _mapper.Map<Comment>(comment);
                log.Comments?.Add(commentMap);

                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);

                return commentMap.Id;
            }

            throw new ArgumentNullException(nameof(logId), "Error");
        }

        public async Task EditGroupsIntoRepairLogAsync(int logId, IEnumerable<string> repairGroups)
        {
            var log = await _repository
                            .GetQuery()
                            .Include(log => log.RepairGroups)
                            .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                var addedGroups = repairGroups.Except(log.RepairGroups.Select(usergroup => usergroup.Name));

                var removedGroups = log.RepairGroups.Select(usergroup => usergroup.Name).Except(repairGroups);

                foreach (var gr in _repositoryRepairGroup.GetQuery())
                {
                    if (addedGroups.Any(ad => ad == gr.Name))
                        log.RepairGroups.Add(gr);
                    else if (removedGroups.Any(rem => rem == gr.Name))
                        log.RepairGroups.Remove(gr);
                }

                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }

        public async Task ToComplete(int logId)
        {
            var log = await _repository
                            .GetQuery()
                            .Include(log => log.Comments)
                            .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.Status = RepairStatus.Completed;
                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }

        public async Task ToChecking(int logId)
        {
            var log = await _repository
                            .GetQuery()
                            .Include(log => log.Comments)
                            .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.Status = RepairStatus.Check;
                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }

        public async Task ToExecute(int logId)
        {
            var log = await _repository
                .GetQuery()
                .Include(log => log.Comments)
                .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }
        
        public async Task ToWork(int logId)
        {
            var log = await _repository
                .GetQuery()
                .Include(log => log.Comments)
                .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.Status = RepairStatus.AtWork;
                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }

        public async Task ToRequest(int logId)
        {
            var log = await _repository
                            .GetQuery()
                            .Include(log => log.Comments)
                            .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.Status = RepairStatus.Request;
                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }

        public async Task ToArchive(int logId)
        {
            var log = await _repository
                            .GetQuery()
                            .Include(log => log.Comments)
                            .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.Status = RepairStatus.Archive;
                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }

        public async Task ToArchiveAllCompleted()
        {
            var logs = await _repository
                .GetQuery()
                .Where(_ => _.Status == RepairStatus.Completed)
                .ToListAsync();

            if (logs != null)
            {
                foreach (var l in logs)
                {
                    l.Status = RepairStatus.Archive;
                    await _repository.Update(l);
                }
            }
        }

        public async Task AddUserToLogExecutors(int logId, int userId)
        {
            var log = await _repository
                .GetQuery()
                .Include(log => log.Executors)
                .Include(log => log.Comments)
                .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.Executors?.Add(await _repositoryUser.Get(userId));

                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }

        public async Task AddUsersToLogExecutors(int logId, List<int> executors)
        {
            var log = await _repository
                            .GetQuery()
                            .Include(log => log.Executors)
                            .FirstOrDefaultAsync(log => log.Id == logId);

            if (log != null)
            {
                log.Executors?.Clear();

                foreach (var exec in executors)
                {
                    log.Executors?.Add(await _repositoryUser.Get(exec));
                }

                log.ChangedDate = DateTime.Now;

                await _repository.Update(log);
            }
        }
    }
}
