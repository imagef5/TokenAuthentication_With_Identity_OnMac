using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace ApiClient.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
		//private static ISettings AppSettings => CrossSettings.Current;
		private static ISettings AppSettings
		{
			get
			{
				if (CrossSettings.IsSupported)
					return CrossSettings.Current;

				return null; // or your custom implementation 
			}
		}

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
		public static string UserName
		{
			get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(UserName), value);
		}

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
		public static string Password
		{
			get => AppSettings.GetValueOrDefault(nameof(Password), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(Password), value);

		}

        /// <summary>
        /// Gets or sets the auth token.
        /// </summary>
        /// <value>The auth token.</value>
		public static string AuthToken
        {
            get => AppSettings.GetValueOrDefault(nameof(AuthToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AuthToken), value);
        }

        /// <summary>
        /// Gets or sets the auth token expiration date.
        /// </summary>
        /// <value>The auth token expiration date.</value>
		public static DateTime AuthTokenExpirationDate
		{
            get => AppSettings.GetValueOrDefault(nameof(AuthTokenExpirationDate), DateTime.UtcNow);
			set => AppSettings.AddOrUpdateValue(nameof(AuthTokenExpirationDate), value);
		}
    }
}
