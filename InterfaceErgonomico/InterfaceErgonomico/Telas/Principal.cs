using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfaceErgonomico
{
    public partial class Principal : Form
    {
        private Stopwatch stopwatch;
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);

        public Principal()
        {
            InitializeComponent();
        }

        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Principal_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void Principal_Load(object sender, EventArgs e)
        {
            stopwatch = new Stopwatch();
            using (LoginForm form = new LoginForm())
            {
                form.ShowDialog();
            }
        }

        private void ConfigBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Configurações";



            //Controle de visuais
            Cronometro.Visible = false;
        }

        private void ErgoBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Perfil";

            //Controle de visuais
            Cronometro.Visible = false;
        }

        private void LiquidBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Hidratação";

            //Controle de visuais
            Cronometro.Visible = false;
        }

        private void TimeBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Cronometragem";

            //Controle de visuais
            Cronometro.Visible = true;
        }

        private void DashboardBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Gráficos";

            //Controle de visuais
            Cronometro.Visible = false;
        }

        private void AboutBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Sobre";

            //Controle de visuais
            Cronometro.Visible = false;
        }

        private void BookBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Dicas E-book";

            //Controle de visuais
            Cronometro.Visible = false;
        }

        private void Logo_Click(object sender, EventArgs e)
        {
            Title.Visible = false;

            //ConfigBtn.FocusState = Bunifu.UI.WinForms.BunifuButton.BunifuButton.ButtonStates.Idle;
        }

        private void UserLogged_Click(object sender, EventArgs e)
        {
            ErgoBtn_Click(sender, e);
        }

        public int flagImage = 1;
        private void StartLogoutBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Cronometragem";
            Cronometro.Visible = true;
            flagImage *= -1;
            if (flagImage == -1)
            {
                StartLogoutBtn.Image = Properties.Resources.app_OK;
                stopwatch.Start();
            }
            else
            {
                StartLogoutBtn.Image = Properties.Resources.toggl_app_512px;
                stopwatch.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Cronometro.Text = string.Format("{0:hh\\:mm\\:ss\\:ff}", stopwatch.Elapsed);
        }
    }
}
