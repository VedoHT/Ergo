using Bunifu.UI.WinForms;
using FireSharp.Interfaces;
using FireSharp.Response;
using InterfaceErgonomico.Models;
using InterfaceErgonomico.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfaceErgonomico.Modulos.Login
{
    public class LoginModulo
    {
        public static string UsuarioValor { get; set; }

        public bool Login(LoginModel request)
        {
            if (request.Register)
            {
                if (string.IsNullOrEmpty(request.UsuarioBtn.Text) &&
                    string.IsNullOrEmpty(request.PasswordText.Text) &&
                    string.IsNullOrEmpty(request.NameText.Text) &&
                    string.IsNullOrEmpty(request.SobrenomeText.Text))
                {
                    MessageBox.Show("Preencha todos os campos");
                    return false;
                }

                if (string.IsNullOrEmpty(request.UsuarioBtn.Text) ||
                    string.IsNullOrEmpty(request.PasswordText.Text))
                {
                    MessageBox.Show("Usuário ou senha inválidos.");
                    return false;
                }

                if (string.IsNullOrEmpty(request.NameText.Text) ||
                    string.IsNullOrEmpty(request.SobrenomeText.Text))
                {
                    MessageBox.Show("Nome ou sobrenome não preenchidos.");
                    return false;
                }

                if (request.PasswordText.Text.Length < 6)
                {
                    MessageBox.Show("Senha deve possuir no mínimo 6 caracteres.");
                    return false;
                }

                if (request.DtaNascimento.Value.Date.Year > 2013)
                {
                    MessageBox.Show("Usuário deve possuir mais de 10 anos de idade.");
                    return false;
                }

                UsuarioRegister registerUser = new UsuarioRegister()
                {
                    Username = request.UsuarioBtn.Text,
                    Password = request.PasswordText.Text,
                    NameComplete = $"{request.NameText.Text} {request.SobrenomeText.Text}",
                    BornDate = request.DtaNascimento.Value.Date
                };

                PerfilUsuario perfilCompleto = new PerfilUsuario()
                {
                    Username = request.UsuarioBtn.Text,
                    NameComplete = $"{request.NameText.Text} {request.SobrenomeText.Text}",
                    Email = "",
                    BornDate = request.DtaNascimento.Value.Date,
                    Phone = "",
                    ProfilePic = null,
                };

                Config configUsuario = new Config()
                {
                    Username = request.UsuarioBtn.Text,
                    PermitirNotif = true,
                    Minimizar = false,
                    TempoPe = 45,
                    Idioma = 1,
                };

                try
                {
                    request.Client.Set(@"Usuarios/" + request.UsuarioBtn.Text, registerUser);
                    request.Client.Set(@"UsersComplete/" + request.UsuarioBtn.Text, perfilCompleto);
                    request.Client.Set(@"UsersConfig/" + request.UsuarioBtn.Text, configUsuario);

                    MessageBox.Show("Registrado com Sucesso!");
                    return true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao registrar! \n Verifique o status do servidor, os campos e sua internet \n Caso o erro persista, entre em contato.");
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.UsernameTextBtn.Text) &&
                    string.IsNullOrEmpty(request.PasswordBtn.Text))
                {
                    MessageBox.Show("Preencha todos os campos");
                    return false;
                }

                try
                {
                    FirebaseResponse res = request.Client.Get(@"Usuarios/" + request.UsernameTextBtn.Text);
                    UsuarioLogin resUser = res.ResultAs<UsuarioLogin>();
                    UsuarioLogin curUser = new UsuarioLogin()
                    {
                        Username = request.UsernameTextBtn.Text,
                        Password = request.PasswordBtn.Text,
                    };

                    switch (ValidaUsuario(resUser, curUser))
                    {
                        case 'E':
                            MessageBox.Show("Usuario não encontrado.");
                            return false;
                        case 'U':
                            MessageBox.Show("Usuario está incorreto");
                            return false;
                        case 'S':
                            MessageBox.Show("Senha está incorreta");
                            return false;
                        case 'O':
                            UsuarioValor = request.UsernameTextBtn.Text;
                            return true;
                        default:
                            return false;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao registrar! \n " +
                        "Verifique o status do servidor, os campos e sua internet \n " +
                        "Caso o erro persista, entre em contato.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Legenda: E = Erro
        ///          U = Usuario diferente
        ///          S = Senha diferente
        ///          O = Ok
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        /// <returns></returns>
        public char ValidaUsuario(UsuarioLogin user1, UsuarioLogin user2)
        {
            if (user1 == null || user2 == null)
                return 'E';

            if (user1.Username != user2.Username)
                return 'U';
            else if (user1.Password != user2.Password)
                return 'S';

            return 'O';
        }

        public char MostraSenha(BunifuCheckBox request)
        {
            return request.Checked ? '\0' : '•';
        }
    }
}
