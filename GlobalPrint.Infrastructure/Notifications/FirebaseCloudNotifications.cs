using Microsoft.CSharp.RuntimeBinder;
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
        private readonly string _sendNotificationService = @"https://fcm.googleapis.com/fcm/send";
        private readonly string _getDeviceInfoService = @"https://iid.googleapis.com/iid/info/{0}?details=true";
        private readonly string _addDeviceToGroupService = @"https://iid.googleapis.com/iid/v1/{0}/rel/topics/{1}";
        private readonly string _serverApiKey = WebConfigurationManager.AppSettings["FirebaseCloudMessagingToken"].ToString();
        private readonly string _senderId = WebConfigurationManager.AppSettings["FirebaseSenderID"].ToString();

        /// <summary>
        /// Send notification via firebase cloud.
        /// </summary>
        /// <param name="deviceId">Devise identifier of recieved user.</param>
        /// <param name="message">Message to send.</param>
        /// <returns>Wrapper with notification result info.</returns>
        public void SendNotification(NotificationMessage message)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(_sendNotificationService);
                webRequest.Method = "post";
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add(string.Format("Authorization: key={0}", _serverApiKey));
                webRequest.Headers.Add(string.Format("Sender: id={0}", _senderId));

                var data = new
                {
                    to = "/topics/" + message.DestinationUserID,
                    notification = new
                    {
                        body = message.Body,
                        title = message.Title,
                        icon = message.Icon,
                        click_action = message.Action
                    },
                    destinationUserID = message.DestinationUserID,
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
                                //result.Response = responseFromServer;
                                //result.IsSuccessful = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var IsSuccessful = false;
                //var Response = null;
                //var Error = ex;
            }
        }

        /// <summary>
        /// Add device to user devices group.
        /// </summary>
        /// <param name="deviceID">Device/browser identifier.</param>
        /// <param name="groupID">Devices group identifier</param>
        /// <returns>Wrapper with response info.</returns>
        public void AddDeviceToGroup(string deviceID, string groupID)
        {
            try
            {
                bool isDeviceAleradyInGroup = CheckDeviceIdInGroup(deviceID, groupID);
                if (isDeviceAleradyInGroup)
                {
                    return;
                }

                string serviceUrl = string.Format(
                    _addDeviceToGroupService, deviceID, groupID
                );
                WebRequest webRequest = WebRequest.Create(serviceUrl);
                webRequest.Method = "post";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = 0;
                webRequest.Headers.Add(string.Format("Authorization: key={0}", _serverApiKey));

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (var dataStreamResponse = webResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStreamResponse))
                        {
                            String responseFromServer = reader.ReadToEnd();
                            //result.Response = responseFromServer;
                            //result.IsSuccessful = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var IsSuccessful = false;
                //result.Response = null;
                //result.Error = ex;
            }
        }

        /// <summary>
        /// Check dynamic object has property with special name.
        /// </summary>
        /// <param name="obj">Dynamic object.</param>
        /// <param name="property">Property name.</param>
        /// <returns>Whether dynamic object has property.</returns>
        private bool CheckDynamicHasProperty(dynamic obj, string property)
        {
            try
            {
                var x = obj.property;
                return true;
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
        }

        /// <summary>
        /// Check, if current device is in group.
        /// </summary>
        /// <param name="deviceID">Device/browser identifier.</param>
        /// <param name="groupID">Devices group identifier</param>
        /// <returns>Whether device is in group or not.</returns>
        public bool CheckDeviceIdInGroup(string deviceID, string groupID)
        {
            string serviceUrl = string.Format(
                _getDeviceInfoService, deviceID
            );
            WebRequest webRequest = WebRequest.Create(serviceUrl);
            webRequest.Method = "get";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add(string.Format("Authorization: key={0}", _serverApiKey));

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (var dataStreamResponse = webResponse.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStreamResponse))
                    {
                        var response = reader.ReadToEnd();
                        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                        if (json != null)
                        {
                            if (CheckDynamicHasProperty(json, "rel") && json.rel != null)
                            {
                                if (CheckDynamicHasProperty(json.rel, "topics") && json.rel.topics != null)
                                {
                                    if (json.rel.topics[groupID] != null)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        
    }
}
