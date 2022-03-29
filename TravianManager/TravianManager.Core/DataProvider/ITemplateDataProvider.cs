using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TravianManager.Core.Data;

namespace TravianManager.Core.DataProvider
{
    public interface ITemplateDataProvider
    {
        Task<IEnumerable<Attacker>> GetAttackers(int templateID, int userId);

        Task<IEnumerable<Account>> GetDefenders(int templateID, int userId);

        bool AddDefender(Defender defender);

        Coordinate GetCoordinates(int accountID);

        int GetAccountIdByAttacker(int attackerID);

        Attacker GetAttacker(int attackerID);

        Attacker GetAttackerById(int templateID, int attackerID);

        void UpdateDefender(Defender defender);

        Task DeleteDefender(int DefenderID);

        Task UpdateAttacker(Attacker attacker);

        Setting GetSetting(int templateID);

        Task<Setting> GetSettings(int templateID);

        Task UpdateSettings(Setting settings);

        void UpdateDefenders(IEnumerable<Defender> defenders);

        Task DeleteAttacker(int AttackerID);

        Task DeleteDefenders(int AttackerID);

        Task<Attacker> GetSpiesAsync(int attackerID);

        Account GetAccount(int accountID);

        Task<IEnumerable<Template>> GetDefensePlans(int userID);

        Template AddPlan(Template plan);

        Task DeletePlan(Template plan);
    }
}
