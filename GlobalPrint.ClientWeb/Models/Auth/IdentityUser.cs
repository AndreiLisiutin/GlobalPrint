﻿using GlobalPrint.Server;
using GlobalPrint.Server.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ClientWeb
{
    public class IdentityUser : User, IUser<int>
    {
        [NotMapped]
        [Obsolete("Поле используется в механизме ASP.NET Identity, оставлено тут для совместимости с ним. Используйте UserID вместо него.")]
        public int Id { get { return base.UserID; } set { base.UserID = value; } }
    }
}
