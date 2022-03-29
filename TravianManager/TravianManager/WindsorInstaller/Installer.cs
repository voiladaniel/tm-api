// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Installer.cs" company="LEGO System A/S">
//   Copyright (c) LEGO System A/S. All rights reserved.
// </copyright>
// <summary>
//   The installer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using TravianManager.Core.Managers;

namespace TravianManager.WindsorInstaller
{
    using System.Data.SqlClient;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Core.Data;
    using Data.Data;
    using Microsoft.Extensions.DependencyInjection;
    using TravianManager.BusinessLogic;
    using TravianManager.BusinessLogic.Managers;
    using TravianManager.Core;
    using TravianManager.Core.Context;
    using TravianManager.Core.DataProvider;
    using TravianManager.Data.Sql;
    using TravianManager.Data.Sql.Context;

    /// <summary>
    /// <inheritdoc cref="Installer"/>
    /// </summary>
    public class Installer : IWindsorInstaller
    {
        /// <summary>
        /// The AppSettings.json configuration file, mapped into a POCO for type-safety.
        /// </summary>
        private readonly IAppSettingsPoco _appSettingsPoco;

        /// <summary>
        /// Initializes a new instance of the <see cref="Installer"/> class.
        /// </summary>
        /// <param name="appSettingsPoco">
        /// The app settings easy config POCO.
        /// </param>
        public Installer(IAppSettingsPoco appSettingsPoco)
        {
            _appSettingsPoco = appSettingsPoco;
        }

        /// <summary>
        /// <inheritdoc cref="Installer"/>
        /// </summary>
        /// <param name="container">
        /// the container.
        /// </param>
        /// <param name="store">
        /// The ConfigurationStore.
        /// </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAppSettingsPoco>()
                    .ImplementedBy<AppSettingsPoco>()
                    .DependsOn(_appSettingsPoco)
                    .LifestyleSingleton(),
                Component.For<IEntityFrameworkDbContext>()
                    .ImplementedBy<EntityFrameworkDbContext>()
                    .LifestyleScoped(),
                Component.For<IAccountManager>()
                    .ImplementedBy<AccountManager>()
                    .LifestyleScoped(),
                Component.For<IUserDataProvider>()
                    .ImplementedBy<UserDataProvider>()
                    .LifestyleScoped(),
                Component.For<IHelpers>()
                    .ImplementedBy<Helpers>()
                    .LifestyleScoped(),
                 Component.For<ITemplateManager>()
                    .ImplementedBy<TemplateManager>()
                    .LifestyleScoped(),
                 Component.For<ICalculator>()
                    .ImplementedBy<Calculator>()
                    .LifestyleScoped(),
                 Component.For<IPlanManager>()
                    .ImplementedBy<PlanManager>()
                    .LifestyleScoped(),
                 Component.For<IPlanDataProvider>()
                    .ImplementedBy<PlanDataProvider>()
                    .LifestyleScoped(),
                 Component.For<ITemplateDataProvider>()
                    .ImplementedBy<TemplateDataProvider>()
                    .LifestyleScoped());
        }
    }
}