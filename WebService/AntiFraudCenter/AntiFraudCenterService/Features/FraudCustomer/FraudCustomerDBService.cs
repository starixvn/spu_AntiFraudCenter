namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerDBService
    {
        IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string encryptedData);

        ParticipantModel GetParticipant(int? participantID);

        string GetFraudCustomerDecryption(int? participantID);

        DBResult AddFraudCustomers(string decryptedData, int? participantID);

        DBResult UpdateFraudCustomers(string decryptedData, int? participantID);

        DBResult DeleteFraudCustomers(string decryptedData, int? participantID);
    }

    public class FraudCustomerDBService : IFraudCustomerDBService
    {
        private readonly IFraudCustomerDataAccess fraudCustomerDataAccess;

        public FraudCustomerDBService(IFraudCustomerDataAccess fraudCustomerDataAccess)
        {
            this.fraudCustomerDataAccess = fraudCustomerDataAccess;
        }

        public IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string decryptedData)
        {
            if (!string.IsNullOrEmpty(decryptedData))
            {
                return fraudCustomerDataAccess.GetFraudCustomers(decryptedData) ?? new List<GetFraudCustomerModel>();
            }

            return new List<GetFraudCustomerModel>();
        }

        public ParticipantModel GetParticipant(int? participantID)
        {
            var participants = fraudCustomerDataAccess.GetParticipant();

            if (participants != null && participants.Any())
            {
                return participants.SingleOrDefault(c => c.ParticipantID == participantID);
            }

            return null;
        }

        public string GetFraudCustomerDecryption(int? participantID)
        {
            if (participantID != null)
            {
                var participant = GetParticipant(participantID);

                if (participant != null)
                {
                    var dummy = new GetFraudCustomerModel
                    {
                        AccountID = "AccountID_Vaule_Test",
                        FullName = "FullName_Vaule_Test",
                        Phone = "Phone_Vaule_Test",
                        CountryCode = "CountryCode_Vaule_Test",
                        Email = "Email_Vaule_Test",
                        DocumentID = "DocumentID_Vaule_Test",
                        DocumentTypeID = 1,
                        BankName = "BankName_Vaule_Test",
                        BankAccountID = "BankAccountID_Vaule_Test",
                        FraudID = 2
                    };

                    var dummy2 = new List<GetFraudCustomerModel>
                    {
                        new GetFraudCustomerModel
                        {
                            AccountID = "AccountID_Vaule_Test",
                            FullName = "FullName_Vaule_Test",
                            Phone = "Phone_Vaule_Test",
                            CountryCode = "CountryCode_Vaule_Test",
                            Email = "Email_Vaule_Test",
                            DocumentID = "DocumentID_Vaule_Test",
                            DocumentTypeID = 1,
                            BankName = "BankName_Vaule_Test",
                            BankAccountID = "BankAccountID_Vaule_Test",
                            FraudID = 2
                        },
                        dummy
                    };

                    string json = JsonConvert.SerializeObject(dummy2);

                    return CryptographicHelper.AESEncrypt(json, participant.SecretKey, participant.ParticipantName);
                }
            }

            return string.Empty;
        }

        public DBResult AddFraudCustomers(string decryptedData, int? participantID)
        {
            if (!string.IsNullOrEmpty(decryptedData) && participantID != null)
            {
                var rs = fraudCustomerDataAccess.AddFraudCustomers(participantID, decryptedData);
                return new DBResult
                {
                    ErrorCode = string.IsNullOrEmpty(rs.ErrorMsg) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                    ErrorMsg = rs.ErrorMsg
                };
            }

            return new DBResult
            {
                ErrorCode = HttpStatus.BadRequest,
                ErrorMsg = "Data input invalid"
            };
        }

        public DBResult UpdateFraudCustomers(string decryptedData, int? participantID)
        {
            if (!string.IsNullOrEmpty(decryptedData) && participantID != null)
            {
                var rs = fraudCustomerDataAccess.UpdateFraudCustomers(participantID, decryptedData);
                return new DBResult
                {
                    ErrorCode = string.IsNullOrEmpty(rs.ErrorMsg) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                    ErrorMsg = rs.ErrorMsg
                };
            }

            return new DBResult
            {
                ErrorCode = HttpStatus.BadRequest,
                ErrorMsg = "Data input invalid"
            };
        }

        public DBResult DeleteFraudCustomers(string decryptedData, int? participantID)
        {
            if (!string.IsNullOrEmpty(decryptedData) && participantID != null)
            {
                var rs = fraudCustomerDataAccess.DeleteFraudCustomers(participantID, decryptedData);
                return new DBResult
                {
                    ErrorCode = string.IsNullOrEmpty(rs.ErrorMsg) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                    ErrorMsg = rs.ErrorMsg
                };
            }

            return new DBResult
            {
                ErrorCode = HttpStatus.BadRequest,
                ErrorMsg = "Data input invalid"
            };
        }
    }
}