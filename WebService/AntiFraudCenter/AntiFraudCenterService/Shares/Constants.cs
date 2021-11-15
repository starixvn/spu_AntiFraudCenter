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
            OK= 200
        };
    }
}
