namespace AntiFraudCenterService.Shares.Model
{
    using Newtonsoft.Json;
    using System.Collections;
    using static AntiFraudCenterService.Shares.Constants;

    public class FraudCustDBResult
    {
        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}