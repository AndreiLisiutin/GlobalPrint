using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Auth
{
    public class IdentityRole : IRole<int>
    {
        public IdentityRole(Role role)
        {
            this.Role = role;
        }
        public IdentityRole(int ID)
            : this(new Role())
        {
            this.Id = ID;
        }
        public IdentityRole(string name)
            : this(new Role())
        {
            this.Name = name;
        }
        public IdentityRole(int ID, string name)
            : this(new Role())
        {
            this.Id = ID;
            this.Name = name;
        }
        public Role Role { get; set; }

        public int Id
        {
            get
            {
                return this.Role.ID;
            }
            set
            {
                this.Role.ID = value;
            }
        }
        public string Name
        {
            get
            {
                return this.Role.Name;
            }
            set
            {
                this.Role.Name = value;
            }
        }
    }
}