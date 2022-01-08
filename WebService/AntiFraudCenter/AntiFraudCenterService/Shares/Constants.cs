namespace AntiFraudCenterService.Shares
{
    public static class Constants
    {
        public enum HttpStatus
        {
            Unauthorized = 401,
            BadRequest = 400,
            NotFound = 404,
            MethodNotAllowed = 405,
            OK = 200,
            Created = 201,
            ServerUnavailable = 503,
            InternalServerError = 500
        };

        #region Error Messages

        public const string InvalidInput = "Invalid data input.";

        public const string InvalidKey = "Invalid key.";

        public const string UMMessage = "System is under maintaince.";

        public const string NotExist = "Not Exist";

        public const string NoPermissionAccount = "No Permission Account";

        public const string CountryCodeInvalid = "CountryCode is invalid";

        #endregion Error Messages

        public const int MaxLenght = 80;

        public enum RequestAction
        {
            Insert = 1,
            Update = 2,
            Delete = 3
        };
    }
}