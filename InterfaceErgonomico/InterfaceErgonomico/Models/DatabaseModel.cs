using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceErgonomico.Models
{
    public class UsuarioLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UsuarioRegister : UsuarioLogin
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
        public char Gender { get; set; }
        public DateTime BornDate { get; set; }
    }
}
