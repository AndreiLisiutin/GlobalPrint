﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users
{
    [Table("user_action_type", Schema = "public")]
    public class UserActionType : IDomainModel
    {
        [Key]
        [Column("user_action_type_id")]
        public int UserActionTypeID { get; set; }
        [Column("name")]
        public string Name { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.UserActionTypeID; }
            set { this.UserActionTypeID = value; }
        }
        #endregion
    }
}