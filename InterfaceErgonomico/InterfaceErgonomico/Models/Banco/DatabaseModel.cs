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
        public string NameComplete { get; set; }
        public DateTime BornDate { get; set; }
    }

    public class PerfilUsuario
    {
        public string Username { get; set; }
        public string NameComplete { get; set; }
        public string Email { get; set; }
        public DateTime BornDate { get; set; }
        public string Phone { get; set; }
        public byte[] ProfilePic { get; set; }
    }

    public class Config
    {
        public string Username { get; set; }
        public bool PermitirNotif { get; set; }
        public bool Minimizar { get; set; }
        public int TempoPe { get; set; }
        public int Idioma { get; set; }
    }

    public class Pdfs
    {
        public string PdfErgo { get; set; }
    }
}
