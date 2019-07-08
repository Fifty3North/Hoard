namespace F3N.Hoard.Utils
{
    public static class Constants
    {
        #region "Device Storage Constants Used In Headers and LocalStorage"

        // TODO: First Five of these are covered under F3N.Mobile.Models.Constants.HttpHeaders - normalize for usage
        public const string DeviceId = "device-Id";
        public const string UserId = "user-Id";
        public const string AuthToken = "auth-token";
        public const string Authorization = "Authorization";
        public const string Roles = "Roles";
        public const string Version = "Version";
        public const string AppId = "app-id";

        public const string LastChange = "LastChange";
        public const string AccessTokenResponse = "AccessTokenResponse";
        public const string LoginMethod = "LoginMethod";
        public const string LoginMethodPin = "PIN";
        public const string LoginMethodPassword = "Password";
        public const string LoggedUserDetails = "LoggedUserDetails";
        public const string ExperimentalFeatures = "ExperimentalFeatures";
        public const string DiagnosticFeatures = "DiagnosticFeatures";

        // TODO: Then this is not a constant? and not used by Practitioner app.
        public static double Padding { get; set; }

        public static string PushNotificationToken = "PushNotificationToken";

        #endregion
    }
}
