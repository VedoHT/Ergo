using Bunifu.UI.WinForms;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceErgonomico.Models.Login
{
    public class LoginModel
    {
        public IFirebaseClient Client { get; set; }
        public bool Register { get; set; }
        public BunifuTextBox UsuarioBtn { get; set; }
        public BunifuTextBox PasswordText { get; set; }
        public BunifuTextBox NameText { get; set; }
        public BunifuTextBox SobrenomeText { get; set; }
        public BunifuDatePicker DtaNascimento { get; set; }
        public BunifuTextBox UsernameTextBtn { get; set; }
        public BunifuTextBox PasswordBtn { get; set; }
    }
}
