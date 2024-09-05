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
using InterfaceErgonomico.Modulos.Login;
using Bunifu.UI.WinForms;
using InterfaceErgonomico.Models.Login;

namespace InterfaceErgonomico
{
    public partial class LoginForm : Form
    {
        private readonly LoginModulo moduloLogin = new LoginModulo();

        private IFirebaseConfig Ifc = new FirebaseConfig()
        {
            AuthSecret = "evhhQwpj8EFCvHEbHudc4DE8PHATnhDeBfQvytwL",
            BasePath = "https://ergo-5ec0f-default-rtdb.firebaseio.com/"
        };

        public IFirebaseClient Client { get; set; }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);

        public static Stopwatch StopwatchValida { get; set; }
        public bool Logado { get; set; }

        public LoginForm()
        {
            InitializeComponent();

            ReturnBtn.Visible = false;
        }

        private void FechaPrograma(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizaPrograma(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;         
        }

        private void MostrarSenhaLogin(object sender, BunifuCheckBox.CheckedChangedEventArgs e)
        {
            PasswordBtn.PasswordChar = moduloLogin.MostraSenha(ShowPasswordBtn);
        }

        private void AnalisaConexao(object sender, EventArgs e)
        {
            try
            {
                Client = new FireSharp.FirebaseClient(Ifc);
                ConnectionStatus.Text = "Conectado";
            }
            catch (Exception)
            {
                ConnectionStatus.Text = "Erro na conexão";
                MessageBox.Show("Foi encontrado um erro na sua conexão. Verifique se possui acesso a internet");
            }

            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }
  
        private void BotaoRegistrar(object sender, EventArgs e)
        {
            LoginModel requestLogin = new LoginModel()
            {
                Client = new FireSharp.FirebaseClient(Ifc),
                Register = true,
                UsuarioBtn = UsuarioBtn,
                PasswordText = PasswordText,
                NameText = NameText,
                SobrenomeText = SobrenomeText,
                DtaNascimento = DtaNascimento,
                UsernameTextBtn = UsernameTextBtn,
                PasswordBtn = PasswordText
            };

            if (moduloLogin.Login(requestLogin))
                DesabilitaCampos(false);
        }

        private void BotaoLogin(object sender, EventArgs e)
        {
            StopwatchValida = new Stopwatch();

            LoginModel requestLogin = new LoginModel()
            {
                Client = new FireSharp.FirebaseClient(Ifc),
                Register = false,
                UsuarioBtn = UsuarioBtn,
                PasswordText = PasswordText,
                NameText = NameText,
                SobrenomeText = SobrenomeText,
                DtaNascimento = DtaNascimento,
                UsernameTextBtn = UsernameTextBtn,
                PasswordBtn = PasswordBtn
            };

            if (moduloLogin.Login(requestLogin))
            {
                Logado = true;
                this.Hide();
                StopwatchValida.Start();
            }
        }

        private void MostrarSenhaRegistro(object sender, BunifuCheckBox.CheckedChangedEventArgs e)
        {
            PasswordText.PasswordChar = moduloLogin.MostraSenha(MostrarRegister);
        }

        private void BotaoRegistrarUsuario(object sender, EventArgs e)
        {
            DesabilitaCampos(true);
        }

        private void BotaoVoltar(object sender, EventArgs e)
        {
            DesabilitaCampos(false);
        }

        private void ArrastaPaineis(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void PreencheTextos(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
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

    }
}
