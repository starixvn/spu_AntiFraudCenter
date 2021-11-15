namespace AntiFraudCenterService.Shares.Model
{
    using Newtonsoft.Json;
    using System.Collections;
    using static AntiFraudCenterService.Shares.Constants;

    public class DBResult
    {
        public HttpStatus ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        [JsonIgnore]
        public Hashtable OutParam { get; set; }
    }
}