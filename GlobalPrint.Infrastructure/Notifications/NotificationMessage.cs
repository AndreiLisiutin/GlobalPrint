using System;

namespace GlobalPrint.Infrastructure.Notifications
{
    /// <summary>
    /// Class for notification messages, sent by Firebase Cloud Service.
    /// </summary>
    public class NotificationMessage
    {
        /// <summary>
        /// Default GlobalPrint logo.
        /// </summary>
        private readonly string _defaultIcon = @"/Resources/Documents/Logo/logo_09_2016.png";
        private readonly string _defaultSound = @"/Resources/Sounds/notification.mp3";

        public NotificationMessage()
        {
            Icon = _defaultIcon;
            Sound = _defaultSound;
        }

        /// <summary>
        /// Message title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Message body text.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Icon for notification.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Sound for notification. Does not work for web.
        /// </summary>
        [Obsolete("Not allowed in web")]
        public string Sound { get; set; }

        /// <summary>
        /// Action to perform on notification click.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Recieved user identifier.
        /// </summary>
        public int DestinationUserID { get; set; }
    }
}
