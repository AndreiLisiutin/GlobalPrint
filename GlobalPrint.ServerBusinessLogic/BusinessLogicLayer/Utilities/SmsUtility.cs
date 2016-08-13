using AberrantSMPP;
using AberrantSMPP.Packet;
using AberrantSMPP.Packet.Request;
using AberrantSMPP.Packet.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities
{
    public class SmsUtility
    {
        public class Parameters
        {
            public Parameters()
            {

            }
            public string Login { get; set; }
            public string Password { get; set; }
            public string Host { get; set; }
            public string Port { get; set; }
            public string Sender { get; set; }
            public bool Enabled { get; set; }

        }
        public SmsUtility(Parameters parameters)
        {
            this._parameters = parameters;
        }
        Parameters _parameters;

        public object HttpUtility { get; private set; }

        private SMPPCommunicator _CreateSmppClient()
        {
            if (!this._parameters.Enabled)
            {
                return null;
            }
            try
            {
                string login = _parameters.Login;
                string password = _parameters.Password;
                string server = _parameters.Host;
                string port = _parameters.Port;
                SMPPCommunicator client = new SMPPCommunicator();
                client.Host = server;//"217.118.84.12";
                client.Port = Convert.ToUInt16(port);//3334;
                client.SystemId = login;// "8299";
                client.Password = password;// "WK|P\"W>a";
                client.EnquireLinkInterval = 30;
                client.ReBindInterval = 10;
                client.ResponseTimeout = 30000;
                client.BindType = AberrantSMPP.Packet.Request.SmppBind.BindingType.BindAsTransceiver;
                client.NpiType = AberrantSMPP.Packet.Pdu.NpiType.ISDN;
                client.TonType = AberrantSMPP.Packet.Pdu.TonType.International;
                client.Version = AberrantSMPP.Packet.Pdu.SmppVersionType.Version3_4;

                client.Bind();
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при чтении из конфигурации системы параметров отправки СМС.", ex);
            }
        }

        public void Send(string phone, string messageText)
        {
            if (!this._parameters.Enabled)
            {
                return;
            }
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    this._SendWebRequest(phone, messageText);
                    return;

                    using (var smppClient = this._CreateSmppClient())
                    {
                        phone = SmsUtility.ExtractValidPhone(phone);

                        var smppRequest = new SmppSubmitSm();
                        smppRequest.SourceAddressTon = Pdu.TonType.Alphanumeric;
                        smppRequest.SourceAddressNpi = Pdu.NpiType.ISDN;
                        smppRequest.DestinationAddressTon = Pdu.TonType.International;
                        smppRequest.DestinationAddressNpi = Pdu.NpiType.ISDN;


                        smppRequest.AlertOnMsgDelivery = 0x1;
                        smppRequest.DataCoding = DataCoding.UCS2;
                        smppRequest.SourceAddress = this._parameters.Sender;
                        smppRequest.DestinationAddress = phone;
                        smppRequest.ValidityPeriod = "000000235959000R"; //YYMMDDhhmmsstnnR
                        smppRequest.LanguageIndicator = LanguageIndicator.Unspecified;
                        smppRequest.ShortMessage = messageText;
                        smppRequest.PriorityFlag = Pdu.PriorityType.Highest;
                        //smppRequest.RegisteredDelivery = (Pdu.RegisteredDeliveryType)0x1e;
                        smppRequest.RegisteredDelivery = Pdu.RegisteredDeliveryType.OnSuccessOrFailure;

                        SmppSubmitSmResp smppResponse = smppClient.SendRequest(smppRequest) as SmppSubmitSmResp;

                        //if (smppResponse.CommandStatus != CommandStatus.ESME_ROK || smppResponse.MessageId == null)
                        //{
                        //}
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            });
        }

        public static string ExtractValidPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }

            string numbers = new string(phone.Replace("+7", "8").AsEnumerable().Where(ch => Char.IsDigit(ch)).ToArray());
            if (numbers.Length == 10)
            {
                numbers = "7" + numbers;
            }
            else if (numbers.Length > 1)
            {
                numbers = "7" + numbers.Substring(1);
            }
            return numbers.Length == 11 ? numbers : null;
        }

        public string GetneratePassword(int charsCount)
        {
            string pass = "";
            Random r = new Random();
            for (int i = 0; i < charsCount; i++)
            {
                pass += r.Next(0, 9);
            }
            return pass;
        }

        public void _SendWebRequest(string phone, string messageText)
        {
            try
            {
                this._parameters.Host = "https://gate.smsaero.ru/send";
                this._parameters.Login = "andreyzykine@mail.ru";
                this._parameters.Password = "123QWEasd";
                this._parameters.Sender = "news";

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("user", this._parameters.Login);
                parameters.Add("password", this.CreateMD5(this._parameters.Password));
                parameters.Add("to", SmsUtility.ExtractValidPhone(phone));
                parameters.Add("text", messageText);
                parameters.Add("from", this._parameters.Sender);

                string response = this._ExecuteWebRequest(this._parameters.Host, parameters);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        /// <summary> Выполнить запрос к сервису бее
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string _ExecuteWebRequest(string uri, Dictionary<string, string> parameters)
        {
            try
            {
                string postData = string.Join("&", parameters.Select(e => e.Key + "=" + (e.Value ?? "")));
                uri += "?" + postData;
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "GET";
                webRequest.KeepAlive = false;
                webRequest.PreAuthenticate = false;
                webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                
                //Encoding requestEncoding = Encoding.GetEncoding("windows-1251");
                //byte[] byteArray = requestEncoding.GetBytes(postData);

                //webRequest.ContentLength = byteArray.Length;
                //using (Stream stream = webRequest.GetRequestStream())
                //{
                //    stream.Write(byteArray, 0, byteArray.Length);
                //    stream.Close();
                //}

                WebResponse webResponse = webRequest.GetResponse();
                Encoding responseEncoding = System.Text.Encoding.UTF8;
                using (StreamReader loResponseStream = new StreamReader(webResponse.GetResponseStream(), responseEncoding))
                {
                    string response = loResponseStream.ReadToEnd();
                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
