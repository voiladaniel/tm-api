// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsPoco.cs" company="LEGO System A/S">
//   Copyright (c) LEGO System A/S. All rights reserved.
// </copyright>
// <summary>
// The POCO for mapping the AppSettings.json file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TravianManager.Data.Data
{
    using Core.Data;

    /// <summary>
    /// The POCO for mapping the AppSettings.json file.
    /// </summary>
    public class AppSettingsPoco : IAppSettingsPoco
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the CORS allowed origins.
        /// </summary>
        public string[] CorsAllowedOrigins { get; set; }
    }
}