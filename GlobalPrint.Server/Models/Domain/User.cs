using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    [Table("user", Schema = "public")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Column("phone")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Телефонный номер введен некорректно")]
        public string Phone { get; set; }
        [Column("login")]
        public string Login { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        [Column("amount_of_money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public decimal AmountOfMoney { get; set; }

    }
}
