using Microsoft.CSharp.RuntimeBinder;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace GlobalPrint.Infrastructure.Notifications
{
    /// <summary>
    /// Класс для отправки push уведомлений через Firebase Cloud Messaging.
    /// </summary>
    public class FirebaseNotificator
    {
        private readonly string _sendNotificationService = @"https://fcm.googleapis.com/fcm/send";
        private readonly string _getDeviceInfoService = @"https://iid.googleapis.com/iid/info/{0}?details=true";
        private readonly string _addDeviceToGroupService = @"https://iid.googleapis.com/iid/v1/{0}/rel/topics/{1}";
        private readonly string _serverApiKey = WebConfigurationManager.AppSettings["FirebaseCloudMessagingToken"].ToString();
        private readonly string _senderId = WebConfigurationManager.AppSettings["FirebaseSenderID"].ToString();
        
        /// <summary>
        /// Отправить push уведомление.
        /// </summary>
        /// <param name="message">Сообщение для отправки.</param>
        /// <returns>Ответ на запрос.</returns>
        public string SendNotification(NotificationMessage message)
        {
            var webRequest = WebRequest.Create(_sendNotificationService);
            webRequest.Method = "post";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add($"Authorization: key={_serverApiKey}");
            webRequest.Headers.Add($"Sender: id={_senderId}");

            // Все данные нужно оставить в поле data, не заполнять notification, иначе не сработает backgroundHandler в js
            var data = new
            {
                to = $"/topics/{message.DestinationUserID}",
                data = new
                {
                    body = message.Body,
                    title = message.Title,
                    icon = message.Icon,
                    url = message.Action,
                    destinationUserID = message.DestinationUserID
                },
                priority = "high"
            };
            var json = new JavaScriptSerializer().Serialize(data);
            var byteArray = Encoding.UTF8.GetBytes(json);
            webRequest.ContentLength = byteArray.Length;

            using (var dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (var webResponse = webRequest.GetResponse())
                {
                    using (var dataStreamResponse = webResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStreamResponse))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Добавить токен браузера к группе пользователя.
        /// </summary>
        /// <param name="deviceID">Токен браузера.</param>
        /// <param name="groupID">Группа пользователя.</param>
        /// <returns>Ответ на запрос.</returns>
        public string AddDeviceToGroup(string deviceID, string groupID)
        {
            bool isDeviceAleradyInGroup = CheckDeviceIdInGroup(deviceID, groupID);
            if (isDeviceAleradyInGroup)
            {
                return null;
            }

            string serviceUrl = string.Format(
                _addDeviceToGroupService, deviceID, groupID
            );
            WebRequest webRequest = WebRequest.Create(serviceUrl);
            webRequest.Method = "post";
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = 0;
            webRequest.Headers.Add($"Authorization: key={_serverApiKey}");

            using (var webResponse = webRequest.GetResponse())
            {
                using (var dataStreamResponse = webResponse.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStreamResponse))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Проверить существование свойства/поля у динамического объекта.
        /// </summary>
        /// <param name="obj">Динамический объект.</param>
        /// <param name="property">Имя свойства.</param>
        /// <returns>Имеет ли объект данное свойство.</returns>
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
        /// Проверить, есть ли токен браузера в группе пользователя.
        /// </summary>
        /// <param name="deviceID">Токен браузера.</param>
        /// <param name="groupID">Группа пользователя.</param>
        /// <returns>Есть ли токен браузера в группе пользователя.</returns>
        public bool CheckDeviceIdInGroup(string deviceID, string groupID)
        {
            string serviceUrl = string.Format(
                _getDeviceInfoService,
                deviceID
            );
            WebRequest webRequest = WebRequest.Create(serviceUrl);
            webRequest.Method = "get";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add($"Authorization: key={_serverApiKey}");

            using (var webResponse = webRequest.GetResponse())
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
