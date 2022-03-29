using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TravianManager.Core;
using TravianManager.Core.Context;
using TravianManager.Core.Data;
using TravianManager.Core.DataProvider;

namespace TravianManager.Data.Sql
{
    public class PlanDataProvider : IPlanDataProvider
    {
        private readonly IEntityFrameworkDbContext _entityFrameworkDbContext;

        private readonly IHelpers _helpers;
        /// <summary>
        /// The Logger.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// The Logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// /Initializes a new instance of the <see cref="AuthorizationDataProvider"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="connectionString">
        /// The Database context.
        /// </param>
        public PlanDataProvider(ILogger logger, IEntityFrameworkDbContext entityFrameworkDbContext, IHelpers helpers)
        {
            this._logger = logger;
            this._entityFrameworkDbContext = entityFrameworkDbContext;
            this._helpers = helpers;
        }

        public bool AddPlan(Plan plan)
        {
            try
            {
                var entity = _entityFrameworkDbContext.Set<Plan>().Where(x => x.PlanID.Equals(plan.PlanID)).FirstOrDefault();
                if (entity == null)
                {
                    var newPlan = new Plan
                    {
                        Name = plan.Name,
                        Password = plan.Password,
                        UserID = plan.UserID
                    };

                    _entityFrameworkDbContext.Set<Plan>().Add(newPlan);
                    _entityFrameworkDbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Adding the data to dbo.Plans failed: {e.Message}.");
                throw;
            }
        }

        public async Task<IEnumerable<Plan>> GetPlans(int userID)
        {

            try
            {
                var attackers = _entityFrameworkDbContext.Set<Plan>()
                    .Where(x => x.UserID == userID)
                    .ToListAsync();

                return await attackers;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<Plan> GetPlanData(int planID)
        {

            try
            {
                var plan = _entityFrameworkDbContext.Set<Plan>()
                    .Where(x => x.PlanID == planID).FirstOrDefaultAsync();

                return await plan;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<Target> AddOrUpdateTarget(Target target)
        {
            try
            {
                var entity = _entityFrameworkDbContext.Set<Target>()
                    .Where(x => x.TargetID.Equals(target.TargetID))
                    .Include(x => x.Account)
                    .FirstOrDefault();

                if (entity == null)
                {
                    var account = new Account
                    {
                        AccountType = 3,
                        Name = target.Account.Name,
                        XCoord = target.Account.XCoord,
                        YCoord = target.Account.YCoord
                    };
                    _entityFrameworkDbContext.Set<Account>().Add(account);
                    target.AccountID = account.AccountID;
                    _entityFrameworkDbContext.Set<Target>().Add(target);
                }
                else
                {
                    entity.Account.Name = target.Account.Name;
                    entity.Account.XCoord = target.Account.XCoord;
                    entity.Account.YCoord = target.Account.YCoord;
                }

                await _entityFrameworkDbContext.SaveChangesAsync();


                var retTarget = _entityFrameworkDbContext.Set<Target>()
                       .Include(x => x.Account)
                       .Include(x => x.PlanDefender).ThenInclude(x => x.PlanAttacker).ThenInclude(x => x.Account)
                       .Where(x => x.TargetID == target.TargetID)
                       .FirstOrDefaultAsync();

                return await retTarget;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Adding the data to dbo.Plans failed: {e.Message}.");
                throw;
            }
        }

        public async Task<PlanAttacker> AddOrUpdatePlanAttacker(PlanAttacker planAttacker)
        {
            try
            {
                var entity = _entityFrameworkDbContext.Set<PlanAttacker>()
                    .Where(x => x.PlanAttackerID.Equals(planAttacker.PlanAttackerID))
                    .Include(x => x.Account)
                    .FirstOrDefault();

                if (entity == null)
                {
                    var account = new Account
                    {
                        AccountType = 4,
                        Name = planAttacker.Account.Name,
                        XCoord = planAttacker.Account.XCoord,
                        YCoord = planAttacker.Account.YCoord
                    };

                    _entityFrameworkDbContext.Set<Account>().Add(account);
                    planAttacker.AccountID = account.AccountID;
                    _entityFrameworkDbContext.Set<PlanAttacker>().Add(planAttacker);
                }
                else
                {
                    entity.Account.Name = planAttacker.Account.Name;
                    entity.Account.XCoord = planAttacker.Account.XCoord;
                    entity.Account.YCoord = planAttacker.Account.YCoord;
                    entity.TournamentSquare = planAttacker.TournamentSquare;
                    entity.TroopSpeed = planAttacker.TroopSpeed;
                    entity.SpeedArtifact = planAttacker.SpeedArtifact;

                }

                await _entityFrameworkDbContext.SaveChangesAsync();

                var retPlanAttacker = _entityFrameworkDbContext.Set<PlanAttacker>()
                        .Include(x => x.Account)
                        .Where(x => x.PlanAttackerID == planAttacker.PlanAttackerID)
                        .FirstOrDefaultAsync();

                return await retPlanAttacker;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Adding the data to dbo.Plans failed: {e.Message}.");
                throw;
            }
        }

        
        public async Task DeletePlanAttacker(int PlantAttackerID)
        {
            try
            {
                var planDefenders = _entityFrameworkDbContext.Set<PlanDefender>()
                    .Where(x => x.PlanAttackerID.Equals(PlantAttackerID)).ToList();
                _entityFrameworkDbContext.Set<PlanDefender>().RemoveRange(planDefenders);

                var planAttacker = _entityFrameworkDbContext.Set<PlanAttacker>()
                    .Where(x => x.PlanAttackerID.Equals(PlantAttackerID)).FirstOrDefault();
                _entityFrameworkDbContext.Set<PlanAttacker>().Remove(planAttacker);

                _entityFrameworkDbContext.Set<Account>().Remove(_entityFrameworkDbContext.Set<Account>().Where(x => x.AccountID.Equals(planAttacker.AccountID)).FirstOrDefault());
                await _entityFrameworkDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task DeleteTarget(int TargetID)
        {
            try
            {
                var target = _entityFrameworkDbContext.Set<Target>().Where(x => x.TargetID.Equals(TargetID)).FirstOrDefault();

                //var defenders = _entityFrameworkDbContext.Set<Defender>().Where(x => x.AttackerID.Equals(AttackerID));
                //_entityFrameworkDbContext.Set<Defender>().RemoveRange(defenders);
                _entityFrameworkDbContext.Set<Target>().Remove(target);
                _entityFrameworkDbContext.Set<Account>().Remove(_entityFrameworkDbContext.Set<Account>().Where(x => x.AccountID.Equals(target.AccountID)).FirstOrDefault());
                await _entityFrameworkDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }


        public async Task<IEnumerable<Target>> GetTargets(int planId)
        {
            try
            {
                var targets = _entityFrameworkDbContext.Set<Target>()
                        .Include(x => x.Account)
                        .Include(x => x.PlanDefender).ThenInclude(x => x.PlanAttacker).ThenInclude(x => x.Account)
                        .Where(x => x.PlanID == planId)
                        .ToListAsync();


                return await targets;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<IEnumerable<PlanAttacker>> GetPlanAttackers(int planId)
        {
            try
            {
                var planAttackers = _entityFrameworkDbContext.Set<PlanAttacker>()
                        .Include(x => x.Account)
                        .Where(x => x.PlanID == planId)
                        .ToListAsync();

                return await planAttackers;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<PlanDefender> AddOrUpdateAttackPlan(AttackPlanData attackPlanData)
        {
            try
            {
                var target = _entityFrameworkDbContext.Set<Target>().Where(x => x.TargetID.Equals(attackPlanData.TargetID)).FirstOrDefault();
                var account = _entityFrameworkDbContext.Set<Account>().Where(x => x.AccountID.Equals(target.AccountID)).FirstOrDefault();

                var planDefen = _entityFrameworkDbContext.Set<PlanDefender>().Where(x => x.PlanDefenderID.Equals(attackPlanData.PlanDefenderID)).FirstOrDefault();

                if (planDefen == null)
                {
                    var planDefender = new PlanDefender
                    {
                        PlanAttackerID = attackPlanData.PlanAttackerID,
                        AccountID = account.AccountID,
                        TargetID = attackPlanData.TargetID,
                        PlanID = attackPlanData.PlanID,
                        ArrivingTime = attackPlanData.ArrivingTime,
                        AttackType = attackPlanData.AttackType ? 1 : 0
                    };

                    _entityFrameworkDbContext.Set<PlanDefender>().Add(planDefender);
                    await _entityFrameworkDbContext.SaveChangesAsync();

                    var retPlanDefender = _entityFrameworkDbContext.Set<PlanDefender>()
                        .Where(x => x.PlanDefenderID.Equals(planDefender.PlanDefenderID))
                        .Include(x => x.Account).Include(x => x.PlanAttacker).ThenInclude(x => x.Account)
                        .FirstOrDefaultAsync();

                    return await retPlanDefender;
                }
                else
                {
                    planDefen.ArrivingTime = attackPlanData.ArrivingTime;
                    planDefen.AttackType = attackPlanData.AttackType ? 1 : 0;

                    _entityFrameworkDbContext.Set<PlanDefender>().Update(planDefen);
                    await _entityFrameworkDbContext.SaveChangesAsync();

                    var retPlanDefender = _entityFrameworkDbContext.Set<PlanDefender>()
                        .Where(x => x.PlanDefenderID.Equals(planDefen.PlanDefenderID))
                        .Include(x => x.Account).Include(x => x.PlanAttacker).ThenInclude(x => x.Account)
                        .FirstOrDefaultAsync();

                    return await retPlanDefender;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<IEnumerable<PlanDefender>> AddOrUpdateAllAttackPlan(AttackPlanData attackPlanData)
        {
            try
            {
                var targets = _entityFrameworkDbContext.Set<Target>()
                    .Where(x => x.PlanID.Equals(attackPlanData.PlanID))
                    .Include(x => x.Account)
                    .ToList();

                foreach (var target in targets)
                {
                    var planDefender = new PlanDefender
                    {
                        PlanAttackerID = attackPlanData.PlanAttackerID,
                        AccountID = target.Account.AccountID,
                        TargetID = target.TargetID,
                        PlanID = attackPlanData.PlanID,
                        ArrivingTime = attackPlanData.ArrivingTime,
                        AttackType = attackPlanData.AttackType ? 1 : 0
                    };

                    _entityFrameworkDbContext.Set<PlanDefender>().Add(planDefender);
                }
                await _entityFrameworkDbContext.SaveChangesAsync();

                var retPlanDefenders = _entityFrameworkDbContext.Set<PlanDefender>()
                    .Where(x => x.PlanID.Equals(attackPlanData.PlanID))
                    .Include(x => x.Account).Include(x => x.PlanAttacker).ThenInclude(x => x.Account)
                    .ToListAsync();

                return await retPlanDefenders;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task DeletePlanDefender(int PlanDefenderID)
        {
            try
            {
                var planDefender = _entityFrameworkDbContext.Set<PlanDefender>().Where(x => x.PlanDefenderID.Equals(PlanDefenderID)).FirstOrDefault();
                _entityFrameworkDbContext.Set<PlanDefender>().Remove(planDefender);
                await _entityFrameworkDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public Target GetTarget(int targetID)
        {
            try
            {
                var target = _entityFrameworkDbContext.Set<Target>()
                    .Include(x => x.Account)
                    .Include(x => x.PlanDefender).ThenInclude(x => x.Account)
                    .Where(x => x.TargetID.Equals(targetID))
                    .FirstOrDefault();

                return target;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public PlanDefender GetPlanDefender(int planDefenderID)
        {
            try
            {
                var target = _entityFrameworkDbContext.Set<PlanDefender>()
                    .Include(x => x.Account)
                    .Include(x => x.PlanAttacker).ThenInclude(x => x.Account)
                    .Where(x => x.PlanDefenderID.Equals(planDefenderID))
                    .FirstOrDefault();

                return target;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public void UpdatePlanDefender(PlanDefender planDefender)
        {
            try
            {
                _entityFrameworkDbContext.Set<PlanDefender>().Update(planDefender);
                _entityFrameworkDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public void UpdatePlanDefenders(List<PlanDefender> planDefender)
        {
            try
            {
                _entityFrameworkDbContext.Set<PlanDefender>().UpdateRange(planDefender);
                _entityFrameworkDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public PlanSetting GetPlanSetting(int planID)
        {
            try
            {
                var setting = _entityFrameworkDbContext.Set<PlanSetting>()
                    .Where(x => x.PlanID.Equals(planID))
                    .FirstOrDefault();

                return setting;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<PlanSetting> GetPlanSettings(int planID)
        {
            try
            {
                var setting = _entityFrameworkDbContext.Set<PlanSetting>()
                    .Where(x => x.PlanID.Equals(planID))
                    .FirstOrDefault();

                if (setting == null)
                {
                    var newPlanSetting = new PlanSetting
                    {
                        TimeBuffer = 0,
                        Message = "",
                        SafeTime = 0,
                        IncludeTTA = 0,
                        IncludeTTL = 0,
                        FakeMessage = "",
                        RealMessage = "",
                        TTAMessage = "",
                        TTLMessage = "",
                        ServerSpeed = 1
                    };

                    return newPlanSetting;
                }
                return setting;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }
        public async Task UpdatePlanSettings(PlanSetting planSetting)
        {
            try
            {
                var entity = _entityFrameworkDbContext.Set<PlanSetting>().Where(x => x.PlanSettingID.Equals(planSetting.PlanSettingID)).FirstOrDefault();

                if(entity == null)
                {
                    var newPlanSetting = new PlanSetting
                    {
                        PlanID = planSetting.PlanID,
                        TimeBuffer = planSetting.TimeBuffer,
                        Message = planSetting.Message,
                        SafeTime = planSetting.SafeTime,
                        IncludeTTA = planSetting.IncludeTTA,
                        IncludeTTL = planSetting.IncludeTTL,
                        FakeMessage = planSetting.FakeMessage,
                        RealMessage = planSetting.RealMessage,
                        TTAMessage = planSetting.TTAMessage,
                        TTLMessage = planSetting.TTLMessage,
                        ServerSpeed = planSetting.ServerSpeed
                    };

                    _entityFrameworkDbContext.Set<PlanSetting>().Add(newPlanSetting);

                    await _entityFrameworkDbContext.SaveChangesAsync();
                }
                else
                {
                    entity.TimeBuffer = planSetting.TimeBuffer;
                    entity.Message = planSetting.Message;
                    entity.SafeTime = planSetting.SafeTime;
                    entity.IncludeTTA = planSetting.IncludeTTA;
                    entity.IncludeTTL = planSetting.IncludeTTL;
                    entity.FakeMessage = planSetting.FakeMessage;
                    entity.RealMessage = planSetting.RealMessage;
                    entity.TTAMessage = planSetting.TTAMessage;
                    entity.TTLMessage = planSetting.TTLMessage;
                    entity.ServerSpeed = planSetting.ServerSpeed;

                    await _entityFrameworkDbContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public PlanSetting GetNotAsyncPlanSetting(int planID)
        {
            try
            {
                var setting = _entityFrameworkDbContext.Set<PlanSetting>()
                    .Where(x => x.PlanID.Equals(planID))
                    .FirstOrDefault();
                if(setting == null)
                {
                    var newPlanSetting = new PlanSetting
                    {
                        TimeBuffer = 0,
                        Message = "",
                        SafeTime = 0,
                        IncludeTTA = 0,
                        IncludeTTL = 0,
                        FakeMessage = "",
                        RealMessage = "",
                        TTAMessage = "",
                        TTLMessage = "",
                        ServerSpeed = 1
                    };

                    return newPlanSetting;
                }
                return setting;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }
    }
}
