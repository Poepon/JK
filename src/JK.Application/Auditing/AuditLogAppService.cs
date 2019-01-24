using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Linq.Extensions;
using JK.Auditing.Dto;
using JK.Auditing.Exporting;
using JK.Authorization.Users;
using JK.Dto;
using JK.EntityHistory;
using Microsoft.EntityFrameworkCore;
using EntityHistoryHelper = JK.EntityHistory.EntityHistoryHelper;

namespace JK.Auditing
{
    [DisableAuditing]
    [AbpAuthorize()]
    public class AuditLogAppService : JKAppServiceBase, IAuditLogAppService
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IRepository<EntityChange, long> _entityChangeRepository;
        private readonly IRepository<EntityChangeSet, long> _entityChangeSetRepository;
        private readonly IRepository<EntityPropertyChange, long> _entityPropertyChangeRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly INamespaceStripper _namespaceStripper;
        private readonly IAbpStartupConfiguration _abpStartupConfiguration;
        private readonly IAuditLogListExcelExporter _auditLogListExcelExporter;
        public AuditLogAppService(
            IRepository<AuditLog, long> auditLogRepository,
            IRepository<User, long> userRepository,
            INamespaceStripper namespaceStripper,
            IRepository<EntityChange, long> entityChangeRepository,
            IRepository<EntityChangeSet, long> entityChangeSetRepository,
            IRepository<EntityPropertyChange, long> entityPropertyChangeRepository,
            IAuditLogListExcelExporter auditLogListExcelExporter,
            IAbpStartupConfiguration abpStartupConfiguration)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
            _namespaceStripper = namespaceStripper;
            _entityChangeRepository = entityChangeRepository;
            _entityChangeSetRepository = entityChangeSetRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
            _auditLogListExcelExporter = auditLogListExcelExporter;
            _abpStartupConfiguration = abpStartupConfiguration;
        }

        #region audit logs

        public async Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input)
        {
            var query = CreateAuditLogAndUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(results);

            return new PagedResultDto<AuditLogListDto>(resultCount, auditLogListDtos);
        }


        private List<AuditLogListDto> ConvertToAuditLogListDtos(List<AuditLogAndUser> results)
        {
            return results.Select(
                result =>
                {
                    var auditLogListDto = ObjectMapper.Map<AuditLogListDto>(result.AuditLog);
                    auditLogListDto.UserName = result.User?.UserName;
                    auditLogListDto.ServiceName = _namespaceStripper.StripNameSpace(auditLogListDto.ServiceName);
                    return auditLogListDto;
                }).ToList();
        }

        private IQueryable<AuditLogAndUser> CreateAuditLogAndUsersQuery(GetAuditLogsInput input)
        {
            var query = from auditLog in _auditLogRepository.GetAll()
                        join user in _userRepository.GetAll() on auditLog.UserId equals user.Id into userJoin
                        from joinedUser in userJoin.DefaultIfEmpty()
                        where auditLog.ExecutionTime >= input.StartDate && auditLog.ExecutionTime <= input.EndDate
                        select new AuditLogAndUser { AuditLog = auditLog, User = joinedUser };

            query = query
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                .WhereIf(!input.ServiceName.IsNullOrWhiteSpace(), item => item.AuditLog.ServiceName.Contains(input.ServiceName))
                .WhereIf(!input.MethodName.IsNullOrWhiteSpace(), item => item.AuditLog.MethodName.Contains(input.MethodName))
                .WhereIf(!input.BrowserInfo.IsNullOrWhiteSpace(), item => item.AuditLog.BrowserInfo.Contains(input.BrowserInfo))
                .WhereIf(input.MinExecutionDuration.HasValue && input.MinExecutionDuration > 0, item => item.AuditLog.ExecutionDuration >= input.MinExecutionDuration.Value)
                .WhereIf(input.MaxExecutionDuration.HasValue && input.MaxExecutionDuration < int.MaxValue, item => item.AuditLog.ExecutionDuration <= input.MaxExecutionDuration.Value)
                .WhereIf(input.HasException == true, item => item.AuditLog.Exception != null && item.AuditLog.Exception != "")
                .WhereIf(input.HasException == false, item => item.AuditLog.Exception == null || item.AuditLog.Exception == "");
            return query;
        }

        #endregion

        #region entity changes 
        public List<NameValueDto> GetEntityHistoryObjectTypes()
        {
            var entityHistoryObjectTypes = new List<NameValueDto>();

            if (AbpSession.TenantId == null)
            {
                foreach (var fullName in EntityHistoryHelper.HostSideTrackedTypes.Select(x => x.FullName))
                {
                    entityHistoryObjectTypes.Add(new NameValueDto(L(fullName), fullName));
                }
            }
            else
            {
                foreach (var fullName in EntityHistoryHelper.TenantSideTrackedTypes.Select(x => x.FullName))
                {
                    entityHistoryObjectTypes.Add(new NameValueDto(L(fullName), fullName));
                }
            }
            return entityHistoryObjectTypes;
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input)
        {
            var query = CreateEntityChangesAndUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

            return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input)
        {
            var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                        join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                        join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                        where entityChange.EntityTypeFullName == input.EntityTypeFullName && entityChange.EntityId == input.EntityId
                        select new EntityChangeAndUser
                        {
                            EntityChange = entityChange,
                            User = user
                        };

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

            return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
        }

        public async Task<List<EntityPropertyChangeDto>> GetEntityPropertyChanges(long entityChangeId)
        {
            var entityPropertyChanges = (await _entityPropertyChangeRepository.GetAllListAsync())
                .Where(epc => epc.EntityChangeId == entityChangeId);

            return ObjectMapper.Map<List<EntityPropertyChangeDto>>(entityPropertyChanges);
        }

        private List<EntityChangeListDto> ConvertToEntityChangeListDtos(List<EntityChangeAndUser> results)
        {
            return results.Select(
                result =>
                {
                    var entityChangeListDto = new EntityChangeListDto
                    {
                        ChangeTime = result.EntityChange.ChangeTime,
                        ChangeType = L(result.EntityChange.ChangeType.ToString()),
                        EntityChangeSetId = result.EntityChange.EntityChangeSetId,
                        EntityTypeFullName = L(result.EntityChange.EntityTypeFullName),
                        Id = result.EntityChange.Id,
                        UserId = result.User.Id,
                        UserName = result.User?.UserName,
                        Reason = result.EntityChangeSet.Reason
                    };

                    return entityChangeListDto;
                }).ToList();
        }

        private IQueryable<EntityChangeAndUser> CreateEntityChangesAndUsersQuery(GetEntityChangeInput input)
        {
            var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                        join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                        join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                        where entityChange.ChangeTime >= input.StartDate && entityChange.ChangeTime <= input.EndDate
                        select new EntityChangeAndUser
                        {
                            EntityChangeSet = entityChangeSet,
                            EntityChange = entityChange,
                            User = user
                        };

            query = query
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                .WhereIf(!input.EntityTypeFullName.IsNullOrWhiteSpace(), item => item.EntityChange.EntityTypeFullName.Contains(input.EntityTypeFullName));

            return query;
        }

        public async Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input)
        {
            var auditLogs = await CreateAuditLogAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(al => al.AuditLog.ExecutionTime)
                .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(auditLogs);

            return _auditLogListExcelExporter.ExportToFile(auditLogListDtos);
        }

        public async Task<FileDto> GetEntityChangesToExcel(GetEntityChangeInput input)
        {
            var entityChanges = await CreateEntityChangesAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(ec => ec.EntityChange.EntityChangeSetId)
                .ThenByDescending(ec => ec.EntityChange.ChangeTime)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(entityChanges);

            return _auditLogListExcelExporter.ExportToFile(entityChangeListDtos);
        }

        #endregion
    }
}
