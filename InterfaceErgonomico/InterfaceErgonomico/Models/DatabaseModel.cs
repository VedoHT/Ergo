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
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public string NameOrg { get; set; }
        public byte[] ProfilePic { get; set; }
    }

    public class Config
    {
        public string Username { get; set; }
        public bool PermitirNotif { get; set; }
        public bool Minimizar { get; set; }
        public int TempoPe { get; set; }
        public int TempoAgua { get; set; }
        public int Idioma { get; set; }
    }

    public class Graficos
    {
        public string Username { get; set; }
        public int? Altura { get; set; }
        public int? Peso { get; set; }
        public bool Pizza { get; set; }
        public TimeSpan TempoSentado { get; set; }
        public TimeSpan TempoPe { get; set; }
        public double? Count { get; set; }
        public string NomeGrafico { get; set; }
    }

    public class Pdfs
    {
        public string PdfErgo { get; set; }
    }
}
