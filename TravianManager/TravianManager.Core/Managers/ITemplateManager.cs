using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravianManager.Core.Data;

namespace TravianManager.Core.Managers
{
    public interface ITemplateManager
    {
        Task<IEnumerable<Attacker>> GetAttackers(int templateId, int userId);

        Task<IEnumerable<Account>> GetDefenders(int templateId, int userId);

        Task<bool> AddDefender(Defender account);

        Task DeleteDefender(int DefenderID);

        Task UpdateAttacker(Attacker attacker);

        Task<Setting> GetSettings(int templateId);

        Task UpdateSettings(Setting settings);

        Task DeleteAttacker(int AttackerID);

        Task DeleteDefenders(int AttackerID);

        Task<Attacker> GetSpies(int AttackerID);

        Task<IEnumerable<Template>> GetDefensePlans(int userID);

        Task<Template> AddPlan(Template plan);

        Task DeletePlan(Template plan);
    }
}
