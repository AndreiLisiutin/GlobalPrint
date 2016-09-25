﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.BankUtility
{
    /// <summary>
    /// Bank info from web service.
    /// </summary>
    /// <remarks>
    /// Taken from http://www.cbr.ru/scripts/CO_XSD/CreditInfoByIntCode.xsd
    /// </remarks>
    public class BankInfo
    {
        /// <summary>
        /// Регистрационный номер КО.
        /// </summary>
        public short? RegNumber { get; set; }
        /// <summary>
        /// БИК.
        /// </summary>
        public string Bic { get; set; }
        /// <summary>
        /// Название организации.
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// Полное название организации.
        /// </summary>
        public string OrgFullName { get; set; }
        /// <summary>
        /// Список телефонов.
        /// </summary>
        public string Phones { get; set; }
        /// <summary>
        /// Дата внесения в КГР.
        /// </summary>
        public DateTime? DateKGRRegistration { get; set; }
        /// <summary>
        /// Основной государственный регистрационный номер.
        /// </summary>
        public string MainRegNumber { get; set; }
        /// <summary>
        /// Дата присвоения государственного регистрационного номера.
        /// </summary>
        public DateTime? MainDateReg { get; set; }
        /// <summary>
        /// Адрес из устава.
        /// </summary>
        public string UstavAdr { get; set; }
        /// <summary>
        /// Адрес фактический.
        /// </summary>
        public string FactAdr { get; set; }
        /// <summary>
        /// Removed Оо.
        /// </summary>
        public string Director { get; set; }
        /// <summary>
        /// Уставный капитал, руб.
        /// </summary>
        public decimal? UstMoney { get; set; }
        /// <summary>
        /// Статус кредитной орг.
        /// </summary>
        public string OrgStatus { get; set; }
        /// <summary>
        /// Вн. код региона.
        /// </summary>
        public short? RegCode { get; set; }
        /// <summary>
        /// Дата вынесения заключения (признак внесения КО в Систему страхования вкладов).
        /// </summary>
        public DateTime? SSV_Date { get; set; }
    }
}
