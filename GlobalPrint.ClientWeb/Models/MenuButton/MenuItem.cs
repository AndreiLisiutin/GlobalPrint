using System.Collections.Generic;

namespace GlobalPrint.ClientWeb.Models.MenuButton
{
    /// <summary>
    /// Пункт меню - кнопка.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Ссылка.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Отображаемое название кнопки.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Является ли действие POST операцией.
        /// </summary>
        public bool IsPostOperation { get; set; }

        /// <summary>
        /// Список css классов кнопки.
        /// </summary>
        public List<string> Classes { get; set; }

        /// <summary>
        /// Список html атрибутов кнопки.
        /// </summary>
        public object HtmlAttributes { get; set; }
    }
}