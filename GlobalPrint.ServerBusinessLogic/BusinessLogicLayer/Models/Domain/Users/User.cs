using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users
{
    [Table("user", Schema = "public")]
    public class User : IDomainModel
    {
        [Key]
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("name")]
        public string UserName { get; set; }
        [Column("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Column("phone")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Телефонный номер введен некорректно")]
        public string PhoneNumber { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        [Column("amount_of_money")]
        public decimal AmountOfMoney { get; set; }
        [Column("phone_confirmed")]
        public bool PhoneNumberConfirmed { get; set; }
        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.UserID; }
            set { this.UserID = value; }
        }
        #endregion
    }
}
