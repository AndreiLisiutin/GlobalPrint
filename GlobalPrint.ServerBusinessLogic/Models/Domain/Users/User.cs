﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Users
{
    [Table("user", Schema = "public")]
    public class User : IDomainModel, IUserProfile
    {
        [DebuggerStepThrough]
        public User()
        {
        }
        
        [Column("name")]
        public string UserName { get; set; }
        [Column("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Column("amount_of_money")]
        public decimal AmountOfMoney { get; set; }
        [Column("bic")]
        public string Bic { get; set; }
        [Column("phone")]
        public string PhoneNumber { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        [Column("security_stamp")]
        public string SecurityStamp { get; set; }
        [Column("phone_confirmed")]
        public bool PhoneNumberConfirmed { get; set; }
        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }
        [Column("last_activity_date")]
        public DateTime LastActivityDate { get; set; }

        #region IDomainModel
        [Key]
        [Column("user_id")]
        public int ID { get; set; } 
        #endregion
    }
}
