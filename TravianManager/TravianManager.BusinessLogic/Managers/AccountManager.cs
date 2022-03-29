using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TravianManager.Core;
using TravianManager.Core.Data;
using TravianManager.Core.DataProvider;
using TravianManager.Core.Managers;
using TravianManager.Data.Data;

namespace TravianManager.BusinessLogic.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly ICalculator _calculator;
        private readonly IAppSettingsPoco _appSettingsPoco;
        private readonly ILogger _logger;
        private readonly IUserDataProvider _userDataProvider;

        public AccountManager(ICalculator calculator, IAppSettingsPoco appSettingsPoco, ILogger logger, IUserDataProvider userDataProvider)
        {
            _calculator = calculator;
            _appSettingsPoco = appSettingsPoco;
            _logger = logger;
            _userDataProvider = userDataProvider;
        }

        public async Task<Tuple<bool, User>> Login(User user)
        {
            _logger.LogInformation("Starting Login");
            try
            {
                return await _userDataProvider.Login(user);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<bool> UpdateDefender(Defender account)
        {
            _logger.LogInformation("Starting Login");
            try
            {
                var result = _userDataProvider.UpdateDefender(account);
                await _calculator.RefreshDataPerTemplate(account.TemplateID).ConfigureAwait(false);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<bool> DeleteDefender(Defender account)
        {
            _logger.LogInformation("Starting Login");
            try
            {
                var result = _userDataProvider.DeleteDefender(account);
                await _calculator.RefreshDataPerTemplate(account.TemplateID).ConfigureAwait(false);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }
    }
}
