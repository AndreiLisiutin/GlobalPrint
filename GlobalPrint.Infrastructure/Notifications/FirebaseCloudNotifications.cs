using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace GlobalPrint.Infrastructure.Notifications
{
    /// <summary>
    /// Class for sending messages via firebase cloud.
    /// </summary>
    public class FirebaseCloudNotifications
    {
        private readonly string _webService = @"https://fcm.googleapis.com/fcm/send";
        private readonly string _serverApiKey = WebConfigurationManager.AppSettings["FirebaseCloudMessagingToken"].ToString();
        private readonly string _senderId = WebConfigurationManager.AppSettings["FirebaseSenderID"].ToString();

        /// <summary>
        /// Send notification via firebase cloud.
        /// </summary>
        /// <param name="deviceId">Devise identifier of recieved user.</param>
        /// <param name="message">Message to send.</param>
        /// <returns>Wrapper with notification result info.</returns>
        public FirebaseNotificationStatus SendNotification(NotificationMessage message)
        {
            FirebaseNotificationStatus result = new FirebaseNotificationStatus();

            try
            {
                WebRequest webRequest = WebRequest.Create(_webService);
                webRequest.Method = "post";
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add(string.Format("Authorization: key={0}", _serverApiKey));
                webRequest.Headers.Add(string.Format("Sender: id={0}", _senderId));
                
                var data = new
                {
                    to = message.Destination,
                    notification = new
                    {
                        body = message.Body,
                        title = message.Title,
                        icon = message.Icon
                    },
                    priority = "high"
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                webRequest.ContentLength = byteArray.Length;

                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (var dataStreamResponse = webResponse.GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStreamResponse))
                            {
                                String responseFromServer = reader.ReadToEnd();
                                result.Response = responseFromServer;
                                result.IsSuccessful = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Response = null;
                result.Error = ex;
            }

            return result;
        }

        /// <summary>
        /// Status of sent firebase notification.
        /// </summary>
        public class FirebaseNotificationStatus
        {
            public bool IsSuccessful { get; set; }
            public string Response { get; set; }
            public Exception Error { get; set; }
        }
    }
}
