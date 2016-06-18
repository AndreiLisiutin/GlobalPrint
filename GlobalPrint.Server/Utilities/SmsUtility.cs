using AberrantSMPP;
using AberrantSMPP.Packet;
using AberrantSMPP.Packet.Request;
using AberrantSMPP.Packet.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class SmsUtility
    {
        public class Parameters
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Host { get; set; }
            public string Port { get; set; }

        }
        public SmsUtility(Parameters param)
        {
            this._param = param;
        }
        Parameters _param;
        private SMPPCommunicator _CreateSmppClient()
        {
            try
            {
                string login = _param.Login;
                string password = _param.Password;
                string server = _param.Host;
                string port = _param.Port;
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

        public string Send(string phone, string messageText)
        {
            try
            {
                using (var smppClient = this._CreateSmppClient())
                {
                    phone = this.ExtractValidPhone(phone);

                    var smppRequest = new SmppSubmitSm();
                    smppRequest.SourceAddressTon = Pdu.TonType.Alphanumeric;
                    smppRequest.SourceAddressNpi = Pdu.NpiType.ISDN;
                    smppRequest.DestinationAddressTon = Pdu.TonType.International;
                    smppRequest.DestinationAddressNpi = Pdu.NpiType.ISDN;


                    smppRequest.AlertOnMsgDelivery = 0x1;
                    smppRequest.DataCoding = DataCoding.UCS2;
                    smppRequest.SourceAddress = "Soft_3784";
                    smppRequest.DestinationAddress = phone;
                    smppRequest.ValidityPeriod = "000000235959000R"; //YYMMDDhhmmsstnnR
                    smppRequest.LanguageIndicator = LanguageIndicator.Unspecified;
                    smppRequest.ShortMessage = messageText;
                    smppRequest.PriorityFlag = Pdu.PriorityType.Highest;
                    //smppRequest.RegisteredDelivery = (Pdu.RegisteredDeliveryType)0x1e;
                    smppRequest.RegisteredDelivery = Pdu.RegisteredDeliveryType.OnSuccessOrFailure;

                    SmppSubmitSmResp smppResponse = smppClient.SendRequest(smppRequest) as SmppSubmitSmResp;

                    if (smppResponse.CommandStatus != CommandStatus.ESME_ROK || smppResponse.MessageId == null)
                    {
                        return null;
                    }
                    return smppResponse.MessageId;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string ExtractValidPhone(string phone)
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

    }
}
