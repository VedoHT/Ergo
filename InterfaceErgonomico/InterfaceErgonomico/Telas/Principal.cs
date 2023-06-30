using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using InterfaceErgonomico.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;

namespace InterfaceErgonomico
{
    public partial class Principal : Form
    {
        private Stopwatch stopwatchSentado;
        private Stopwatch stopwatchPe;
        private Stopwatch stopwatchLogin;
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
        TimeSpan tempoSentadoAnterior;
        TimeSpan tempoAguaAnterior;
        public TimeSpan tempoSentado;
        public TimeSpan tempoPe;

        double quantidadeFalta;

        int tempoAgua;
        int tempoPeValida;

        bool foiNotificadoAgua;
        bool foiNotificadoPe;
        bool validaPrimeiraVoltaPe;
        bool validaPrimeiraVoltaAgua;

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
            stopwatchSentado = new Stopwatch();
            stopwatchPe = new Stopwatch();
            stopwatchLogin = new Stopwatch();
            client = new FireSharp.FirebaseClient(ifc);

            using (LoginForm form = new LoginForm())
            {
                form.ShowDialog();
                txtUsuario.Text = LoginForm.UsuarioValor;
                pauseBtn.Image = Properties.Resources.app_OK;

                FirebaseResponse res = client.Get(@"UsersComplete/" + txtUsuario.Text);
                PerfilUsuario resUser = res.ResultAs<PerfilUsuario>();

                if (resUser.ProfilePic != null)
                {
                    byte[] imageConvert = (byte[])Convert.ChangeType(resUser.ProfilePic, typeof(byte[]));
                    var bitmap = ByteToImage(imageConvert);
                    UserLogged.Image = bitmap;
                }

                if (form.logado)
                {
                    stopwatchLogin = LoginForm.stopwatchValida;
                    Logo_Click(sender, e);
                }
            }
        }

        private void ConfigBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Configurações";

            FirebaseResponse res = client.Get(@"UsersConfig/" + txtUsuario.Text);
            Config resUser = res.ResultAs<Config>();

            switch (resUser.Idioma)
            {
                case 0:
                    comboBoxIdioma.SelectedIndex = 0;
                    break;
                case 1:
                    comboBoxIdioma.SelectedIndex = 1;
                    break;
                case 2:
                    comboBoxIdioma.SelectedIndex = 2;
                    break;
                default:
                    comboBoxIdioma.SelectedIndex = 0;
                    break;
            }

            switch (resUser.PermitirNotif)
            {
                case true:
                    radioSimNotif.Checked = true;
                    break;
                case false:
                    radioNaoNotif.Checked = true;
                    break;
                default:
                    radioSimNotif.Checked = true;
                    break;
            }

            switch (resUser.Minimizar)
            {
                case true:
                    radioSimPlano.Checked = true;
                    break;
                case false:
                    radioSimPlano.Checked = true;
                    break;
                default:
                    radioSimPlano.Checked = true;
                    break;
            }

            lembrarTomarAgua.Text = resUser.TempoAgua.ToString();
            minutosLevantar.Text = resUser.TempoPe.ToString();
            
            salvarConfig.Visible = true;

            tempoAguatitle.Visible = true;
            lembrarTomarAgua.Visible = true;

            tempoPeTitle.Visible = true;
            minutosLevantar.Visible = true;

            TitleIdioma.Visible = true;
            comboBoxIdioma.Visible = true;

            titleNotifWindows.Visible = true;
            pnlNotificacao.Visible = true;

            titleSegundoPlano.Visible = true;
            pnlSegundoPlano.Visible = true;

            //Controle de visuais de Sobre
            pnlSobre.Visible = false;

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Hidratacao
            pnlHidratacao.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void ErgoBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Perfil";
            pnlPerfil.Visible = true;

            FirebaseResponse res = client.Get(@"UsersComplete/" + txtUsuario.Text);
            PerfilUsuario resUser = res.ResultAs<PerfilUsuario>();

            nameTxt.Text = resUser.NameComplete;
            emailTxt.Text = resUser.Email;
            dtaNascimento.Value = resUser.BornDate.Date;

            if (string.IsNullOrEmpty(resUser.NameOrg))
                naoRadio.Checked = true;
            else
            {
                empresaTxt.Enabled = true;
                simRadio.Checked = true;
            }

            telefoneTxt.Text = resUser.Phone.ToString();
            alturaTxt.Text = resUser.Height.ToString();
            pesoTxt.Text = resUser.Weight.ToString();
            empresaTxt.Text = resUser.NameOrg;

            if (resUser.ProfilePic != null)
            {
                byte[] imageConvert = (byte[])Convert.ChangeType(resUser.ProfilePic, typeof(byte[]));
                var bitmap = ByteToImage(imageConvert);
                ImageBox1.Image = bitmap;
            }
            else
            {
                ImageBox1.Image = null;
            }

            //Controle de visuais de Sobre
            pnlSobre.Visible = false;

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Hidratacao
            pnlHidratacao.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoAguatitle.Visible = false;
            lembrarTomarAgua.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void LiquidBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Hidratação";
            pnlHidratacao.Visible = true;

            FirebaseResponse res = client.Get(@"UsersComplete/" + txtUsuario.Text);
            PerfilUsuario resUser = res.ResultAs<PerfilUsuario>();

            if (resUser.Weight != null)
            {
                var metaAguaDiaria = Convert.ToDouble(resUser.Weight);
                quantidadeFalta = Convert.ToDouble(metaAguaDiaria * 35);

                if(qtdFaltaText.Text == "0")
                    qtdFaltaText.Text = quantidadeFalta.ToString();

                metaDiariaResult.Text = $@"{metaAguaDiaria * 35}";
                qtdFaltaText.Text = $@"{qtdFaltaText.Text}";
                jaTomeiText.Text = $@"{jaTomeiText.Text}";
            }
            else
            {
                metaDiariaResult.Text = "Falta cadastrar seu peso na aba 'Perfil'";
                qtdFaltaText.Text = $@"";
                jaTomeiText.Text = $@"";
            }

            //Controle de visuais de Sobre
            pnlSobre.Visible = false;

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoAguatitle.Visible = false;
            lembrarTomarAgua.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void TimeBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Cronometragem";

            CronometroSentado.Visible = true;
            CronometroPe.Visible = true;

            titleNaoSentado.Visible = true;
            titleSentado.Visible = true;

            BtnGerarGraficos.Visible = true;
            pnlConfigs.Visible = true;
            radioPizza.Checked = true;

            //Controle de visuais de Sobre
            pnlSobre.Visible = false;

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Hidratacao
            pnlHidratacao.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoAguatitle.Visible = false;
            lembrarTomarAgua.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;
        }

        private void DashboardBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Gráficos";
            pnlGrafico.Visible = true;
            graficoChart.Visible = false;
            textBoxResultado.Text = "";
            listGraficos.Items.Clear();

            double j = 1;
            FirebaseResponse resGrafico = client.Get($@"Graficos/{txtUsuario.Text}/{j}");
            Graficos resUser = resGrafico.ResultAs<Graficos>();

            if (resGrafico.Body != "null")
            {
                if (listGraficos.Items.Count == 0)
                    listGraficos.Items.Add(resUser.NomeGrafico);

                for (j = 2; resGrafico.Body != "null"; ++j)
                {
                    resGrafico = client.Get($@"Graficos/{txtUsuario.Text}/{j}");
                    if (resGrafico.Body != "null")
                    {
                        resUser = resGrafico.ResultAs<Graficos>();
                        listGraficos.Items.Add(resUser.NomeGrafico);
                    }
                }              
            }

            //Controle de visuais de Sobre
            pnlSobre.Visible = false;

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Hidratacao
            pnlHidratacao.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoAguatitle.Visible = false;
            lembrarTomarAgua.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void AboutBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Sobre";
            pnlSobre.Visible = true;

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Hidratacao
            pnlHidratacao.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoAguatitle.Visible = false;
            lembrarTomarAgua.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void BookBtn_Click(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Dicas E-book";
            pnlDicas.Visible = true;

            //Controle de visuais de Sobre
            pnlSobre.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Hidratacao
            pnlHidratacao.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoAguatitle.Visible = false;
            lembrarTomarAgua.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void Logo_Click(object sender, EventArgs e)
        {
            FirebaseResponse resConfig = client.Get(@"UsersConfig/" + txtUsuario.Text);
            Config resUserConfig = resConfig.ResultAs<Config>();
            tempoAgua = resUserConfig.TempoAgua;
            tempoPeValida = resUserConfig.TempoPe;
            validaPrimeiraVoltaAgua = false;
            validaPrimeiraVoltaPe = false;

            Title.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Hidratacao
            pnlHidratacao.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoAguatitle.Visible = false;
            lembrarTomarAgua.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void UserLogged_Click(object sender, EventArgs e)
        {
            ErgoBtn_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tempoSentado = stopwatchSentado.Elapsed;
            this.CronometroSentado.Text = string.Format("{0:hh\\:mm\\:ss\\:ff}", tempoSentado);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            tempoPe = stopwatchPe.Elapsed;
            this.CronometroPe.Text = string.Format("{0:hh\\:mm\\:ss\\:ff}", tempoPe);
        }

        public int flagImage = 1;

        //BtnUP
        private void btnUpClick(object sender, EventArgs e)
        {
            SitBtn.Image = Properties.Resources.Sitguy;
            UpBtn.Image = Properties.Resources.UpPressed;
            pauseBtn.Image = Properties.Resources.Pause;
            stopwatchSentado.Stop();
            stopwatchPe.Start();      
        }

        //BtnSit
        private void btnSitClick(object sender, EventArgs e)
        {
            SitBtn.Image = Properties.Resources.Sitpressed;
            UpBtn.Image = Properties.Resources.Upguy;
            pauseBtn.Image = Properties.Resources.Pause;
            stopwatchSentado.Start();
            stopwatchPe.Stop();           
        }

        private void BtnGerarGraficos_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = client.Get(@"UsersComplete/" + txtUsuario.Text);
            PerfilUsuario resUser = res.ResultAs<PerfilUsuario>();

            double j = 1;
            FirebaseResponse resGrafico = client.Get($@"Graficos/{txtUsuario.Text}/{j}");

            if (resGrafico.Body != "null")
            {
                for(j = 2; resGrafico.Body != "null"; j++)
                    resGrafico = client.Get($@"Graficos/{txtUsuario.Text}/{j}");

                j -= 1;
            }

            var nomeGrafico = $"{j}";

            Graficos grafico = new Graficos()
            {
                Username = resUser.Username,
                Altura = resUser.Height,
                Peso = resUser.Weight,
                TempoSentado = tempoSentado,
                TempoPe = tempoPe,
                Pizza = radioPizza.Checked == true ? true : false,
                Count = j,
                NomeGrafico = $@"{DateTime.Now.ToString("dd/MM/yyyy")}-{j}"
            };

            SetResponse set = client.Set($@"Graficos/{resUser.Username}/{nomeGrafico}", grafico);
            MessageBox.Show("Gráfico gerado com sucesso.");
        }


        private void checkBoxAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAuto.Checked)
            {
                radioDiario.Enabled = true;
                radioSemanal.Enabled = true;
                radioMensal.Enabled = true;
            }
            else
            {
                radioDiario.Enabled = false;
                radioSemanal.Enabled = false;
                radioMensal.Enabled = false;
            }
        }

        private void radioNaoPlano_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioSimPlano_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioSimNotif_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioNaoNotif_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioPizza_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioColunas_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioDiario_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioSemanal_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioMensal_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void selecionarFotoBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnfd = new OpenFileDialog();
            opnfd.Filter = "Arquivos de Imagem (*jpg;*.jpeg)|*.jpg;*.jpeg";

            if (opnfd.ShowDialog() == DialogResult.OK)
                ImageBox1.Image = new Bitmap(opnfd.FileName);
        }

        private void simRadio_CheckedChanged(object sender, EventArgs e)
        {
            empresaTxt.Enabled = true;
        }

        private void naoRadio_CheckedChanged(object sender, EventArgs e)
        {
            empresaTxt.Enabled = false;
        }

        private void btnlSalvarPerfil_Click(object sender, EventArgs e)
        {
            byte[] novaImagem = null;

            string org = "";


            if (simRadio.Checked && string.IsNullOrEmpty(empresaTxt.Text))
                MessageBox.Show("Preencha com a empresa que você trabalha.");
            else if (simRadio.Checked)
                org = empresaTxt.Text;
            else
                org = "";

            int? nulo = null;

            if (ImageBox1.Image != null)
            {
                ImageConverter converter = new ImageConverter();
                novaImagem = (byte[])converter.ConvertTo(ImageBox1.Image, typeof(byte[]));
            }

            PerfilUsuario perfilCompleto = new PerfilUsuario()
            {
                Username = txtUsuario.Text,
                NameComplete = nameTxt.Text,
                Email = emailTxt.Text,
                BornDate = dtaNascimento.Value,
                Phone = string.IsNullOrEmpty(telefoneTxt.Text) == false ? telefoneTxt.Text : "",
                Height = string.IsNullOrEmpty(alturaTxt.Text) == false ? Convert.ToInt32(alturaTxt.Text) : nulo,
                Weight = string.IsNullOrEmpty(pesoTxt.Text) == false ? Convert.ToInt32(pesoTxt.Text) : nulo,
                NameOrg = org,
                ProfilePic = novaImagem,
            };

            try
            {
                SetResponse set = client.Set(@"UsersComplete/" + perfilCompleto.Username, perfilCompleto);
                MessageBox.Show("Salvo com sucesso!");

                if(ImageBox1.Image != null)
                    UserLogged.Image = ImageBox1.Image;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao registrar! \n Verifique o status do servidor, os campos e sua internet \n Caso o erro persista, entre em contato.");
            }

        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            SitBtn.Image = Properties.Resources.Sitguy;
            UpBtn.Image = Properties.Resources.Upguy;
            pauseBtn.Image = Properties.Resources.app_OK;
            stopwatchPe.Stop();
            stopwatchSentado.Stop();
        }

        private void simRadio_CheckedChanged_1(object sender, EventArgs e)
        {
            empresaTxt.Enabled = true;
        }

        private void naoRadio_CheckedChanged_1(object sender, EventArgs e)
        {
            empresaTxt.Enabled = false;
        }

        private void listGraficos_SelectedIndexChanged(object sender, EventArgs e)
        {
            var nomeGrafico = listGraficos.SelectedItem.ToString();

            string[] split = nomeGrafico.Split('-');
            var teste = "";
            for (int i = 0; i < split.Length; i++)
                teste = split[i];

            double? grafico = Convert.ToDouble(teste);

            GerarGrafico(grafico);
        }

       
        private void GerarGrafico(double? count)
        {
            FirebaseResponse res = client.Get($@"Graficos/{txtUsuario.Text}/{count}");
            Graficos resUser = res.ResultAs<Graficos>();
            string sentadoPe = "";

            //Grafico Pizza
            if (resUser.Pizza)
            {
                if (graficoChart.Series.Count != 0)
                {
                    graficoChart.Series.Clear();
                    graficoChart.Titles.Clear();
                    graficoChart.Legends.Clear();
                }

                graficoChart.Visible = true;

                GraficosSeparadosPizza graficoPizza = new GraficosSeparadosPizza();
                var lista = graficoPizza.Pizza(resUser.NomeGrafico, resUser.TempoSentado, resUser.TempoPe);

                var colunas = graficoPizza.GetNomeColunas(lista);
                var valores = graficoPizza.GetValoresColunas(lista);

                //TituloPrincipal
                var titulo = new Title();
                titulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                titulo.ForeColor = Color.Gray;
                titulo.Text = $@"Gráfico: {resUser.NomeGrafico}";
                graficoChart.Titles.Add(titulo);

                //Legendas
                /*graficoChart.Legends.Add("Legenda");
                graficoChart.Legends[0].Title = "Legendas";
                */
                graficoChart.Series.Add("Tempos");
                graficoChart.Series[0].ChartType = SeriesChartType.Pie;

                //Série de dados com os valores do gráfico
                graficoChart.Series[0].Points.DataBindXY(colunas, valores);

                if (TimeSpan.Compare(resUser.TempoPe, resUser.TempoSentado) == 1)
                {
                    sentadoPe = "em pé, indicando que você está cuidando de seu tempo em frente ao computador, continue assim!\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                else
                {
                    sentadoPe = "sentado, indicando que você necessita de pausas no tempo de uso do computador, para uma rotina mais saudável.\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                textBoxResultado.Text = $@"Com base no gráfico gerado, você está passando mais tempo {sentadoPe}";

            }
            else  //Grafico Colunas
            {
                if (graficoChart.Series.Count != 0)
                {
                    graficoChart.Series.Clear();
                    graficoChart.Titles.Clear();
                    graficoChart.Legends.Clear();
                }

                graficoChart.Visible = true;

                GraficosSeparadosPizza graficoPizza = new GraficosSeparadosPizza();
                var lista = graficoPizza.Pizza(resUser.NomeGrafico, resUser.TempoSentado, resUser.TempoPe);

                var colunas = graficoPizza.GetNomeColunas(lista);
                var valores = graficoPizza.GetValoresColunas(lista);

                //TituloPrincipal
                var titulo = new Title();
                titulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                titulo.ForeColor = Color.Gray;
                titulo.Text = $@"Gráfico: {resUser.NomeGrafico}";
                graficoChart.Titles.Add(titulo);

                //Legendas
                /*graficoChart.Legends.Add("Legenda");
                graficoChart.Legends[0].Title = "Legendas";
                */
                graficoChart.Series.Add("Tempos");
                graficoChart.Series[0].ChartType = SeriesChartType.Column;

                graficoChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

                //Série de dados com os valores do gráfico
                graficoChart.Series[0].Points.DataBindXY(colunas, valores);

                if (TimeSpan.Compare(resUser.TempoPe, resUser.TempoSentado) == 1)
                {
                    sentadoPe = "em pé, indicando que você está cuidando de seu tempo em frente ao computador, continue assim!\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                else
                {
                    sentadoPe = "sentado, indicando que você necessita de pausas no tempo de uso do computador, para uma rotina mais saúdavel.\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                textBoxResultado.Text = $@"Com base no gráfico gerado, você está passando mais tempo {sentadoPe}";
            }
            
        }

        private void quantidadeKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var falta = Convert.ToInt32(quantidadeKey.Text) + 1;
            quantidadeKey.Text = falta.ToString();

            if (Convert.ToInt32(quantidadeKey.Text) == 0)
            {
                quantidadeKey.Text = "0";
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var falta = Convert.ToInt32(quantidadeKey.Text) - 1;
            quantidadeKey.Text = falta.ToString();

            if (Convert.ToInt32(quantidadeKey.Text) == 0)
            {
                quantidadeKey.Text = "0";
            }
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(quantidadeKey.Text) > 0)
            {
                var falta = Convert.ToInt32(qtdFaltaText.Text) - Convert.ToInt32(quantidadeKey.Text);
                if (falta < 0)
                {
                    parabensTxt.Visible = true;
                    jaTomeiText.Text = (Convert.ToInt32(jaTomeiText.Text) + Convert.ToInt32(quantidadeKey.Text)).ToString();
                }
                else
                {
                    qtdFaltaText.Text = $@"{falta}";
                    quantidadeFalta = Convert.ToDouble(qtdFaltaText.Text);
                    jaTomeiText.Text = (Convert.ToInt32(jaTomeiText.Text) + Convert.ToInt32(quantidadeKey.Text)).ToString();
                }
            }
            else
            {
                var falta = Convert.ToInt32(qtdFaltaText.Text) + Convert.ToInt32(quantidadeKey.Text);
                if (falta < 0)
                {
                    parabensTxt.Visible = true;
                    jaTomeiText.Text = (Convert.ToInt32(jaTomeiText.Text) + Convert.ToInt32(quantidadeKey.Text)).ToString();
                }
                else
                {
                    qtdFaltaText.Text = $@"{falta}";
                    quantidadeFalta = Convert.ToDouble(qtdFaltaText.Text);
                    jaTomeiText.Text = (Convert.ToInt32(jaTomeiText.Text) + Convert.ToInt32(quantidadeKey.Text)).ToString();
                }
            }

            quantidadeKey.Text = "0";
        }

        private void nameTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void telefoneTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = client.Get(@"Pdfs/");
            Pdfs resUser = res.ResultAs<Pdfs>();
            byte[] bytes = Convert.FromBase64String(resUser.PdfErgo);
            var pasta = "";
            bool exists = false;

            if (browsePdf.ShowDialog() == DialogResult.OK)
            {
                pasta = browsePdf.SelectedPath;
            }

            try
            {
                using (var stream = new FileStream($@"{pasta}\DicasErgo.pdf", FileMode.CreateNew))
                using (var writer = new StreamWriter(stream))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    //stream.Close();
                    MessageBox.Show("Arquivo se encontra na pasta designada.");
                }
            }
            catch (IOException te)
            {
                if (te.HResult == -2147024816 || te.HResult == -2147024713)
                {
                    exists = File.Exists($@"{pasta}\DicasErgo.pdf");
                    File.Delete($@"{pasta}\DicasErgo.pdf");          
                }
            }
        }

        private void minutosLevantar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void lembrarTomarAgua_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void salvarConfig_Click(object sender, EventArgs e)
        {
            Config ConfigUsuario = new Config()
            {
                Idioma = comboBoxIdioma.SelectedIndex,
                PermitirNotif = radioSimNotif.Checked == true ? true : false,
                Minimizar = radioSimPlano.Checked == true ? true : false,
                TempoAgua = Convert.ToInt32(lembrarTomarAgua.Text),
                TempoPe = Convert.ToInt32(minutosLevantar.Text),
                Username = txtUsuario.Text
            };

            tempoPeValida = Convert.ToInt32(minutosLevantar.Text);
            tempoAgua = Convert.ToInt32(lembrarTomarAgua.Text);

            SetResponse setConfig = client.Set(@"UsersConfig/" + txtUsuario.Text, ConfigUsuario);
            MessageBox.Show("Salvo com sucesso!");
        }


        private void timer3_Tick(object sender, EventArgs e)
        {
            var teste = stopwatchLogin.Elapsed;
            var validaTesteSentado = false;
            var validaTesteAgua = false;

            TimeSpan testeSentado = new TimeSpan(tempoSentadoAnterior.Days, tempoSentadoAnterior.Hours, (tempoSentadoAnterior.Minutes + tempoPeValida), tempoSentadoAnterior.Seconds, tempoSentadoAnterior.Milliseconds);
            TimeSpan resultadoSentado = tempoSentado.Subtract(testeSentado);

            TimeSpan testeAgua = new TimeSpan(tempoAguaAnterior.Days, tempoAguaAnterior.Hours, (tempoAguaAnterior.Minutes + tempoAgua), tempoAguaAnterior.Seconds, tempoAguaAnterior.Milliseconds);
            TimeSpan resultadoAgua = teste.Subtract(testeAgua);

            if(resultadoSentado.Minutes == tempoPeValida)
                validaTesteSentado = true;

            if (resultadoAgua.Minutes == tempoPeValida)
                validaTesteAgua = true;

            if (validaPrimeiraVoltaPe == false && tempoSentado.Minutes == tempoPeValida && tempoSentado.Minutes != 0 && tempoSentado.Milliseconds > 1 && tempoSentado.Milliseconds <= 600 && foiNotificadoPe == false)
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("Icon.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Ficar de pé";
                notifyIcon1.BalloonTipText = "Está na hora de você ficar de pé.";
                notifyIcon1.ShowBalloonTip(100);
                foiNotificadoPe = true;
                validaPrimeiraVoltaPe = true;
                tempoSentadoAnterior = tempoSentado;
            }
            else if((validaTesteSentado && tempoSentado.Minutes != 0 && tempoSentado.Milliseconds > 1 && tempoSentado.Milliseconds <= 600 && foiNotificadoPe == false))
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("Icon.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Ficar de pé";
                notifyIcon1.BalloonTipText = "Está na hora de você ficar de pé.";
                notifyIcon1.ShowBalloonTip(100);
                foiNotificadoPe = true;
                tempoSentadoAnterior = tempoSentado;
            }

            if (validaPrimeiraVoltaAgua == false && teste.Minutes == tempoAgua && teste.Minutes != 0 && teste.Milliseconds > 1 && teste.Milliseconds <= 600 && foiNotificadoAgua == false)
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("sitguyNOTIF.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Tomar água";
                notifyIcon1.BalloonTipText = "Está na hora de você tomar água";
                notifyIcon1.ShowBalloonTip(100);
                foiNotificadoAgua = true;
                validaPrimeiraVoltaAgua = true;
                tempoAguaAnterior = teste;
            }
            else if (validaTesteAgua && teste.Minutes != 0 && teste.Milliseconds > 1 && teste.Milliseconds <= 600 && foiNotificadoAgua == false)
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("sitguyNOTIF.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Tomar água";
                notifyIcon1.BalloonTipText = "Está na hora de você tomar água";
                notifyIcon1.ShowBalloonTip(100);
                foiNotificadoAgua = true;
                tempoAguaAnterior = teste;
            }

            if(tempoSentado.Minutes != 0 && validaTesteSentado)
            {
                foiNotificadoPe = false;
            }

            if (teste.Minutes != 0 && validaTesteAgua)
            {
                foiNotificadoAgua = false;
            }
        }

        private void pesoTxt_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void alturaTxt_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }
    }
}
