// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppSettingsPoco.cs" company="LEGO System A/S">
//   Copyright (c) LEGO System A/S. All rights reserved.
// </copyright>
// <summary>
// The interface for POCO for mapping the AppSettings.json file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TravianManager.Core.Data
{
    /// <summary>
    /// The interface for POCO for mapping the AppSettings.json file.
    /// </summary>
    public interface IAppSettingsPoco
    {
        string ConnectionString { get; set; }

        /// Gets or sets the CORS allowed origins.
        /// </summary>
        string[] CorsAllowedOrigins { get; set; }
    }
}