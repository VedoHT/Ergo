using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using InterfaceErgonomico.Models;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace InterfaceErgonomico
{
    public partial class LoginForm : Form
    {

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "evhhQwpj8EFCvHEbHudc4DE8PHATnhDeBfQvytwL",
            BasePath = "https://ergo-5ec0f-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);

        public static string UsuarioValor = "";
        public static Stopwatch stopwatchValida;
        public bool logado;

        public LoginForm()
        {
            InitializeComponent();

            ReturnBtn.Visible = false;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;         
        }

        private void PasswordText_TextChanged(object sender, EventArgs e)
        {

        }

        private void PasswordCheck_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {

        }

        private void ShowPassword_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            PasswordBtn.PasswordChar = ShowPasswordBtn.Checked ? '\0' : '•';
        }

        //Demonstra se a conexão com o banco de dados está ok
        private void LoginForm_Load_1(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(ifc);

                if (client != null)
                    ConnectionStatus.Text = "Conectado";
                else
                    ConnectionStatus.Text = "Erro na conexão";

            }
            catch (Exception)
            {
                MessageBox.Show("Foi encontrado um erro na sua conexão. Verifique se possui acesso a internet");
            }

            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        //Botão de registro
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (Login(true))
                DesabilitaCampos(false);
        }

        //Botão de login
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            stopwatchValida = new Stopwatch();
            if (Login(false))
            {
                logado = true;
                this.Hide();
                stopwatchValida.Start();
            }
        }

        private void MostrarRegister_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            PasswordText.PasswordChar = MostrarRegister.Checked ? '\0' : '•';
        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            DesabilitaCampos(true);
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            DesabilitaCampos(false);
        }

        #region Funções Auxiliares

        public bool Login(bool register)
        {
            if (register)
            {
                if (string.IsNullOrEmpty(UsuarioBtn.Text) &&
                    string.IsNullOrEmpty(PasswordText.Text) &&
                    string.IsNullOrEmpty(NameText.Text) &&
                    string.IsNullOrEmpty(SobrenomeText.Text))
                {
                    MessageBox.Show("Preencha todos os campos");
                    return false;
                }

                if (string.IsNullOrEmpty(UsuarioBtn.Text) ||
                    string.IsNullOrEmpty(PasswordText.Text))
                {
                    MessageBox.Show("Usuário ou senha inválidos.");
                    return false;
                }

                if (string.IsNullOrEmpty(NameText.Text) ||
                    string.IsNullOrEmpty(SobrenomeText.Text))
                {
                    MessageBox.Show("Nome ou sobrenome não preenchidos.");
                    return false;
                }

                if (PasswordText.Text.Length < 6)
                {
                    MessageBox.Show("Senha deve possuir no mínimo 6 caracteres.");
                    return false;
                }

                if (DtaNascimento.Value.Date.Year > 2013)
                {
                    MessageBox.Show("Usuário deve possuir mais de 10 anos de idade.");
                    return false;
                }

                UsuarioRegister registerUser = new UsuarioRegister()
                {
                    Username = UsuarioBtn.Text,
                    Password = PasswordText.Text,
                    NameComplete = $"{NameText.Text} {SobrenomeText.Text}",
                    BornDate = DtaNascimento.Value.Date
                };

                PerfilUsuario perfilCompleto = new PerfilUsuario()
                {
                    Username = UsuarioBtn.Text,
                    NameComplete = $"{NameText.Text} {SobrenomeText.Text}",
                    Email = "",
                    BornDate = DtaNascimento.Value.Date,
                    Phone = "",
                    Height = null,
                    Weight = null,
                    NameOrg = "",
                    ProfilePic = null,
                };

                Config configUsuario = new Config()
                {
                    Username = UsuarioBtn.Text,
                    PermitirNotif = true,
                    Minimizar = false,
                    TempoAgua = 45,
                    TempoPe = 45,
                    Idioma = 1,
                };

                try
                {
                    SetResponse set = client.Set(@"Usuarios/" + UsuarioBtn.Text, registerUser);
                    SetResponse setComplete = client.Set(@"UsersComplete/" + UsuarioBtn.Text, perfilCompleto);
                    SetResponse setConfig = client.Set(@"UsersConfig/" + UsuarioBtn.Text, configUsuario);

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
                if (string.IsNullOrEmpty(UsernameTextBtn.Text) &&
                    string.IsNullOrEmpty(PasswordBtn.Text))
                {
                    MessageBox.Show("Preencha todos os campos");
                    return false;
                }

                try
                {
                    FirebaseResponse res = client.Get(@"Usuarios/" + UsernameTextBtn.Text);
                    UsuarioLogin resUser = res.ResultAs<UsuarioLogin>();
                    UsuarioLogin curUser = new UsuarioLogin()
                    {
                        Username = UsernameTextBtn.Text,
                        Password = PasswordBtn.Text,
                    };

                    switch(ValidaUsuario(resUser, curUser))
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
                            UsuarioValor = UsernameTextBtn.Text;
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

        #region Controle de campos que serão visualizados ou não

        public void DesabilitaCampos(bool register)
        {

            if (register)
            {
                #region Deixar invisivel botões login

                RegisterBtn.Visible = false;
                LoginBtn.Visible = false;
                ShowPasswordText.Visible = false;
                ShowPasswordBtn.Visible = false;
                MoreText.Visible = false;
                PasswordCheckBtn.Visible = false;
                PasswordBtn.Visible = false;
                UsernameTextBtn.Visible = false;
                #endregion

                #region Mostrar botões de registro
                NameRegisterTxt.Visible = true;
                NameText.Visible = true;
                SobrenomeRegisterTxt.Visible = true;
                SobrenomeText.Visible = true;
                DtaNascimentoRegisterTxt.Visible = true;
                DtaNascimento.Visible = true;
                UsuarioRegisterTxt.Visible = true;
                UsuarioBtn.Visible = true;
                SenhaRegisterTxt.Visible = true;
                PasswordText.Visible = true;
                ConfirmButton.Visible = true;
                ReturnBtn.Visible = true;
                MostrarRegister.Visible = true;
                MostrarRegisterText.Visible = true;
                #endregion
            }
            else
            {
                #region Deixar invisivel botões de registro

                RegisterBtn.Visible = true;
                LoginBtn.Visible = true;
                ShowPasswordText.Visible = true;
                ShowPasswordBtn.Visible = true;
                MoreText.Visible = true;
                PasswordCheckBtn.Visible = true;
                PasswordBtn.Visible = true;
                UsernameTextBtn.Visible = true;
                #endregion

                #region Mostrar botões de login
                NameRegisterTxt.Visible = false;
                NameText.Visible = false;
                SobrenomeRegisterTxt.Visible = false;
                SobrenomeText.Visible = false;
                DtaNascimentoRegisterTxt.Visible = false;
                DtaNascimento.Visible = false;
                UsuarioRegisterTxt.Visible = false;
                UsuarioBtn.Visible = false;
                SenhaRegisterTxt.Visible = false;
                PasswordText.Visible = false;
                ConfirmButton.Visible = false;
                ReturnBtn.Visible = false;
                MostrarRegister.Visible = false;
                MostrarRegisterText.Visible = false;
                #endregion
            }
        }
        #endregion

        #endregion

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void bunifuPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void NameText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void SobrenomeText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;        
        }

        private void UsernameTextBtn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }
    }
}
