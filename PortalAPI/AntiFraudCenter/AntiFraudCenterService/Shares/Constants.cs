namespace AntiFraudCenterService.Shares
{
    public static class Constants
    {
        public const int Aes128 = 128;

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

        public const string InvalidInput = "Invalid data input.";

        public const string DataTooLong = "Invalid data input.";

        public const string EncryptedKey = "95595967e187d95b";

        public const string IVKey = "@/adhtsknccr&";

        public const string InternalPassword = "Welcome1";

        public const int SecretKey_NumberOfByte = 16;

        public const int MaxLenght = 80;
    }
}