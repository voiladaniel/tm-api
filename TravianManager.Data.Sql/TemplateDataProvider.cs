using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
using TravianManager.Data.Sql.Context;

namespace TravianManager.Data.Sql
{
    public class TemplateDataProvider : ITemplateDataProvider
    {
        private readonly IServiceScopeFactory scopeFactory;

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
        public TemplateDataProvider(ILogger logger, IEntityFrameworkDbContext entityFrameworkDbContext, IHelpers helpers)
        {
            this._logger = logger;
            this._entityFrameworkDbContext = entityFrameworkDbContext;
            this._helpers = helpers;
        }
        public async Task DeletePlan(Template plan)
        {
            try
            {
                var attackers = await _entityFrameworkDbContext.Set<Attacker>().Where(x => x.TemplateID.Equals(plan.TemplateID)).ToListAsync();
                var defenders = await _entityFrameworkDbContext.Set<Defender>().Where(x => attackers.Any(p => p.AttackerID.Equals(x.AttackerID))).ToListAsync();

                _entityFrameworkDbContext.Set<Defender>().RemoveRange(defenders);
                _entityFrameworkDbContext.Set<Attacker>().RemoveRange(attackers);

                _entityFrameworkDbContext.Set<Account>().RemoveRange(_entityFrameworkDbContext.Set<Account>().Where(x => attackers.Any(p => p.AccountID.Equals(x.AccountID))).ToList());
                await _entityFrameworkDbContext.SaveChangesAsync();

                var entity = _entityFrameworkDbContext.Set<Template>().Where(x => x.TemplateID.Equals(plan.TemplateID)).FirstOrDefault();
                if (entity != null)
                {
                    _entityFrameworkDbContext.Set<Template>().Remove(entity);
                    await _entityFrameworkDbContext.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                this._logger.LogError($"Adding the data to dbo.Plans failed: {e.Message}.");
                throw;
            }
        }
        public Template AddPlan(Template plan)
        {
            try
            {
                var newPlan = new Template();

                var entity = _entityFrameworkDbContext.Set<Template>().Where(x => x.TemplateID.Equals(plan.TemplateID)).FirstOrDefault();
                if (entity == null)
                {
                    newPlan.Name = plan.Name;
                    newPlan.UserID = plan.UserID;

                    _entityFrameworkDbContext.Set<Template>().Add(newPlan);
                    _entityFrameworkDbContext.SaveChanges();
                }

                return newPlan;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Adding the data to dbo.Plans failed: {e.Message}.");
                throw;
            }
        }

        public async Task<IEnumerable<Template>> GetDefensePlans(int userID)
        {

            try
            {
                var defensePlans = _entityFrameworkDbContext.Set<Template>()
                    .Where(x => x.UserID == userID)
                    .ToListAsync();

                return await defensePlans;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public bool AddDefender(Defender defender)
        {
                try
                {
                    var entity = _entityFrameworkDbContext.Set<Account>().Where(x => x.AccountID.Equals(defender.AccountID)).FirstOrDefault();
                    if (entity == null)
                    {
                        var account = new Account
                        {
                            AccountType = 0,
                            Name = defender.Account.Name,
                            XCoord = defender.Account.XCoord,
                            YCoord = defender.Account.YCoord,
                            UserID = defender.Account.UserID
                        };

                        _entityFrameworkDbContext.Set<Account>().Add(account);
                        _entityFrameworkDbContext.SaveChanges();
                        return false;
                    }
                    _entityFrameworkDbContext.Set<Defender>().Add(defender);
                    _entityFrameworkDbContext.SaveChanges();

                    return true;
                }
                catch (Exception e)
                {
                    this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                    throw;
                }
        }

        public async Task DeleteDefender(int DefenderID)
        {
            try
            {
                var defender = _entityFrameworkDbContext.Set<Defender>().Where(x => x.DefenderID.Equals(DefenderID)).FirstOrDefault();
                _entityFrameworkDbContext.Set<Defender>().Remove(defender);
                await _entityFrameworkDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task DeleteDefenders(int AttackerID)
        {
            try
            {
                var defenders = _entityFrameworkDbContext.Set<Defender>().Where(x => x.AttackerID.Equals(AttackerID)).ToList();
                _entityFrameworkDbContext.Set<Defender>().RemoveRange(defenders);
                await _entityFrameworkDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task DeleteAttacker(int AttackerID)
        {
            try
            {
                var attacker = _entityFrameworkDbContext.Set<Attacker>().Where(x => x.AttackerID.Equals(AttackerID)).FirstOrDefault();
                var defenders = _entityFrameworkDbContext.Set<Defender>().Where(x => x.AttackerID.Equals(AttackerID));
                _entityFrameworkDbContext.Set<Defender>().RemoveRange(defenders);
                _entityFrameworkDbContext.Set<Attacker>().Remove(attacker);
                _entityFrameworkDbContext.Set<Account>().Remove(_entityFrameworkDbContext.Set<Account>().Where(x => x.AccountID.Equals(attacker.AccountID)).FirstOrDefault());
                await _entityFrameworkDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task UpdateAttacker(Attacker attacker)
        {
            try
            {
                var entity = _entityFrameworkDbContext.Set<Attacker>().Where(x => x.AttackerID.Equals(attacker.AttackerID)).FirstOrDefault();
                if (entity == null)
                {
                    var account = new Account {
                        AccountType = 1,
                        Name = attacker.Account.Name,
                        XCoord = attacker.Account.XCoord,
                        YCoord = attacker.Account.YCoord
                    };

                   var accunt = _entityFrameworkDbContext.Set<Account>().Add(account);
                   attacker.AccountID = account.AccountID;
                   _entityFrameworkDbContext.Set<Attacker>().Add(attacker);
                }
                else
                {
                    entity.NotBeforeTime = attacker.NotBeforeTime;
                    entity.TroopSpeed = attacker.TroopSpeed;
                    entity.TournamentSquare = attacker.TournamentSquare;
                    entity.SpeedArtifact = attacker.SpeedArtifact;
                }

                await _entityFrameworkDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public void UpdateDefender(Defender defender)
        {
            try
            {
                _entityFrameworkDbContext.Set<Defender>().Update(defender);
                _entityFrameworkDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public void UpdateDefenders(IEnumerable<Defender> defenders)
        {
            try
            {
                _entityFrameworkDbContext.Set<Defender>().UpdateRange(defenders);
                _entityFrameworkDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }


        public async Task<IEnumerable<Attacker>> GetAttackers(int templateID, int userId)
        {
         
                try
                {
                var attackers = _entityFrameworkDbContext.Set<Attacker>()
                    .Include(x => x.Account)
                    .Include(x => x.Defender).ThenInclude(x => x.Account)
                    .Where(x => x.TemplateID == templateID)
                    .ToListAsync();

                return await attackers;
                }
                catch (Exception e)
                {
                    this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                    throw;
                }
        }

        public Attacker GetAttackerById(int templateID, int attackerID)
        {

            try
            {
                var attacker = _entityFrameworkDbContext.Set<Attacker>()
                    .Include(x => x.Account)
                    .Include(x => x.Defender).ThenInclude(x => x.Account)
                    .Where(x => x.TemplateID.Equals(templateID) && x.AttackerID.Equals(attackerID))
                    .FirstOrDefault();

                return attacker;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetDefenders(int templateID, int userId)
        {
                try
                {
                    var defenders = _entityFrameworkDbContext.Set<Account>()
                        .Where(x => x.AccountType.Equals(0) && x.UserID.Equals(userId))
                        .ToListAsync();

                    return await defenders;
                }
                catch (Exception e)
                {
                    this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                    throw;
                }
        }

        public Coordinate GetCoordinates(int accountID)
        {
            try
            {
                var account = _entityFrameworkDbContext.Set<Account>()
                    .Where(x => x.AccountID.Equals(accountID))
                    .FirstOrDefault();

                var coordinates = new Coordinate
                {
                    XCoordinate = Convert.ToInt32(account.XCoord),
                    YCoordinate = Convert.ToInt32(account.YCoord)
                };

                return coordinates;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public int GetAccountIdByAttacker(int attackerID)
        {
            try
            {
                var account = _entityFrameworkDbContext.Set<Attacker>()
                    .Where(x => x.AttackerID.Equals(attackerID))
                    .Select(x => x.AccountID)
                    .FirstOrDefault();

                return account;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public Attacker GetAttacker(int attackerID)
        {
            try
            {
                var account = _entityFrameworkDbContext.Set<Attacker>()
                    .Include(x => x.Account)
                    .Include(x => x.Defender).ThenInclude(x => x.Account)
                    .Where(x => x.AttackerID.Equals(attackerID))
                    .FirstOrDefault();

                return account;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public Account GetAccount(int accountID)
        {
            try
            {
                var account = _entityFrameworkDbContext.Set<Account>()
                    .Where(x => x.AccountID.Equals(accountID))
                    .FirstOrDefault();

                return account;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

        public async Task<Attacker> GetSpiesAsync(int attackerID)
        {
            try
            {
                var account = _entityFrameworkDbContext.Set<Attacker>()
                    .Include(x => x.Account)
                    .Include(x => x.Defender).ThenInclude(x => x.Account)
                    .Where(x => x.AttackerID.Equals(attackerID))
                    .FirstOrDefaultAsync();

                return await account;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }
        public Setting GetSetting(int templateID)
        {
            try
            {
                var setting = _entityFrameworkDbContext.Set<Setting>()
                    .Where(x => x.TemplateID.Equals(templateID))
                    .FirstOrDefault();

                if (setting == null)
                {
                    var newPlanSetting = new Setting
                    {
                        TimeInterval = 0,
                        TemplateID = templateID
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
        public async Task<Setting> GetSettings(int templateID)
        {
                try
                {
                    var setting = _entityFrameworkDbContext.Set<Setting>()
                        .Where(x => x.TemplateID.Equals(templateID))
                        .FirstOrDefaultAsync();

                if (setting == null)
                {
                    var newPlanSetting = new Setting
                    {
                        TimeInterval = 0,
                        TemplateID = templateID
                    };

                    return newPlanSetting;
                }

                return await setting;
                }
                catch (Exception e)
                {
                    this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                    throw;
                }
        }
        public async Task UpdateSettings(Setting settings)
        {
            try
            {
                var entity = _entityFrameworkDbContext.Set<Setting>().Where(x => x.SettingID.Equals(settings.SettingID)).FirstOrDefault();

                if (entity == null)
                {
                    var newPlanSetting = new Setting
                    {
                        TimeInterval = settings.TimeInterval,
                        TemplateID = settings.TemplateID
                    };

                    _entityFrameworkDbContext.Set<Setting>().Add(newPlanSetting);
                    await _entityFrameworkDbContext.SaveChangesAsync();
                }
                else
                {
                    entity.TimeInterval = settings.TimeInterval;

                    await _entityFrameworkDbContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"Getting the data from config.AuthorizationRoles on config database failed: {e.Message}.");
                throw;
            }
        }

    }
}
