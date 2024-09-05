using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using InterfaceErgonomico.Models;
using InterfaceErgonomico.Models.Graficos;
using InterfaceErgonomico.Modulos.Grafico;
using InterfaceErgonomico.Modulos.Login;
using InterfaceErgonomico.Modulos.Principal;
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
        //Modulos
        private GraficosModulo graficosModulo = new GraficosModulo();
        private PrincipalModulo principalModulo = new PrincipalModulo();

        //Globais cronômetro
        private Stopwatch stopwatchSentado;
        private Stopwatch stopwatchPe;
        private Stopwatch stopwatchLogin;

        #region ConfigFirebase
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
        #endregion

        #region Globais
        private TimeSpan TempoSentadoAnterior;
        private TimeSpan TempoAguaAnterior;
        private TimeSpan TempoSentado;
        private TimeSpan TempoPe;
        private int TempoAgua;
        private int TempoPeValida;
        private bool FoiNotificadoAgua;
        private bool FoiNotificadoPe;
        private bool ValidaPrimeiraVoltaPe;
        private bool ValidaPrimeiraVoltaAgua;
        #endregion

        public Principal()
        {
            InitializeComponent();
        }

        #region ControleFormulario
        private void FechaPrograma(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizaPrograma(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ArrastaPaineis(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void CarregaFormularioPrincipal(object sender, EventArgs e)
        {
            stopwatchSentado = new Stopwatch();
            stopwatchPe = new Stopwatch();
            stopwatchLogin = new Stopwatch();
            Client = new FireSharp.FirebaseClient(Ifc);

            using (LoginForm form = new LoginForm())
            {
                form.ShowDialog();
                txtUsuario.Text = LoginModulo.UsuarioValor;
                pauseBtn.Image = Properties.Resources.app_OK;

                FirebaseResponse res = Client.Get(@"UsersComplete/" + txtUsuario.Text);
                PerfilUsuario resUser = res.ResultAs<PerfilUsuario>();

                if (resUser.ProfilePic != null)
                {
                    byte[] imageConvert = (byte[])Convert.ChangeType(resUser.ProfilePic, typeof(byte[]));
                    var bitmap = principalModulo.ConverteByteParaImagem(imageConvert);
                    UserLogged.Image = bitmap;
                }

                if (form.Logado)
                {
                    stopwatchLogin = LoginForm.StopwatchValida;
                    AcaoLogotipo(sender, e);
                }
            }
        }
        #endregion

        private void AcaoLogotipo(object sender, EventArgs e)
        {
            FirebaseResponse resConfig = Client.Get(@"UsersConfig/" + txtUsuario.Text);
            Config resUserConfig = resConfig.ResultAs<Config>();
            TempoPeValida = resUserConfig.TempoPe;
            ValidaPrimeiraVoltaAgua = false;
            ValidaPrimeiraVoltaPe = false;

            Title.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
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

        private void MiniUsuario(object sender, EventArgs e)
        {
            AbaPerfil(sender, e);
        }

        private void CronometroUm(object sender, EventArgs e)
        {
            TempoSentado = stopwatchSentado.Elapsed;
            this.CronometroSentado.Text = string.Format("{0:hh\\:mm\\:ss\\:ff}", TempoSentado);
        }

        private void CronometroDois(object sender, EventArgs e)
        {
            TempoPe = stopwatchPe.Elapsed;
            this.CronometroPe.Text = string.Format("{0:hh\\:mm\\:ss\\:ff}", TempoPe);
        }

        private void HabilitaBotaoDePe(object sender, EventArgs e)
        {
            SitBtn.Image = Properties.Resources.Sitguy;
            UpBtn.Image = Properties.Resources.UpPressed;
            pauseBtn.Image = Properties.Resources.Pause;
            stopwatchSentado.Stop();
            stopwatchPe.Start();      
        }

        private void HabilitaBotaoSentado(object sender, EventArgs e)
        {
            SitBtn.Image = Properties.Resources.Sitpressed;
            UpBtn.Image = Properties.Resources.Upguy;
            pauseBtn.Image = Properties.Resources.Pause;
            stopwatchSentado.Start();
            stopwatchPe.Stop();           
        }

        private void BotaoGerarGraficos(object sender, EventArgs e)
        {
            NotificaGeraGraficosRequest request = new NotificaGeraGraficosRequest()
            {
                Client = Client,
                TxtUsuario = txtUsuario,
                TempoSentado = TempoSentado,
                TempoPe = TempoPe,
                RadioPizza = radioPizza
            };
            
            graficosModulo.NotificaGeraGrafico(request);
        }

        private void ControlaCheckBoxes(object sender, EventArgs e)
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

        private void BotaoSelecionaImagemPerfil(object sender, EventArgs e)
        {
            OpenFileDialog opnfd = new OpenFileDialog();
            opnfd.Filter = "Arquivos de Imagem (*jpg;*.jpeg)|*.jpg;*.jpeg";

            if (opnfd.ShowDialog() == DialogResult.OK)
                ImageBox1.Image = new Bitmap(opnfd.FileName);
        }

        private void BotaoSalvarPerfil(object sender, EventArgs e)
        {
            byte[] novaImagem = null;

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
                Phone = !string.IsNullOrEmpty(telefoneTxt.Text) ? telefoneTxt.Text : "",
                ProfilePic = novaImagem,
            };

            try
            {
                Client.Set(@"UsersComplete/" + perfilCompleto.Username, perfilCompleto);
                MessageBox.Show("Salvo com sucesso!");

                if(ImageBox1.Image != null)
                    UserLogged.Image = ImageBox1.Image;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao registrar! \n Verifique o status do servidor, os campos e sua internet \n Caso o erro persista, entre em contato.");
            }

        }

        private void BotaoPausarCronometro(object sender, EventArgs e)
        {
            SitBtn.Image = Properties.Resources.Sitguy;
            UpBtn.Image = Properties.Resources.Upguy;
            pauseBtn.Image = Properties.Resources.app_OK;
            stopwatchPe.Stop();
            stopwatchSentado.Stop();
        }

        private void PainelListagemDeGraficos(object sender, EventArgs e)
        {
            var nomeGrafico = listGraficos.SelectedItem.ToString();

            string[] split = nomeGrafico.Split('-');
            var teste = "";
            for (int i = 0; i < split.Length; i++)
                teste = split[i];

            double? grafico = Convert.ToDouble(teste);

            GeraGraficosRequest requestGrafico = new GeraGraficosRequest()
            {
                Client = Client,
                TxtUsuario = txtUsuario,
                GraficoChart = graficoChart,
                Contagem = grafico
            };
            var graficoGerado = graficosModulo.GerarGrafico(requestGrafico);
            graficoChart = graficoGerado.GraficoChart;
            textBoxResultado.Text = graficoGerado.TextBoxResultado;
        }

        private void PreencheNumeros(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void PreencheTextos(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void BotaoVisualizarPdf(object sender, EventArgs e)
        {
            FirebaseResponse res = Client.Get(@"Pdfs/");
            Pdfs resUser = res.ResultAs<Pdfs>();
            byte[] bytes = Convert.FromBase64String(resUser.PdfErgo);
            var pasta = "";

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
                    MessageBox.Show("Arquivo se encontra na pasta designada.");
                }
            }
            catch (IOException te)
            {
                if (te.HResult == -2147024816 || te.HResult == -2147024713)
                {
                    File.Exists($@"{pasta}\DicasErgo.pdf");
                    File.Delete($@"{pasta}\DicasErgo.pdf");          
                }
            }
        }

        private void BotaoSalvaConfiguracao(object sender, EventArgs e)
        {
            Config ConfigUsuario = new Config()
            {
                Idioma = comboBoxIdioma.SelectedIndex,
                PermitirNotif = radioSimNotif.Checked,
                Minimizar = radioSimPlano.Checked,
                TempoPe = Convert.ToInt32(minutosLevantar.Text),
                Username = txtUsuario.Text
            };

            TempoPeValida = Convert.ToInt32(minutosLevantar.Text);

            Client.Set(@"UsersConfig/" + txtUsuario.Text, ConfigUsuario);
            MessageBox.Show("Salvo com sucesso!");
        }

        private void NotificacaoDesktop(object sender, EventArgs e)
        {
            var teste = stopwatchLogin.Elapsed;
            var validaTesteSentado = false;
            var validaTesteAgua = false;

            TimeSpan testeSentado = new TimeSpan(TempoSentadoAnterior.Days, TempoSentadoAnterior.Hours, (TempoSentadoAnterior.Minutes + TempoPeValida), TempoSentadoAnterior.Seconds, TempoSentadoAnterior.Milliseconds);
            TimeSpan resultadoSentado = TempoSentado.Subtract(testeSentado);

            TimeSpan testeAgua = new TimeSpan(TempoAguaAnterior.Days, TempoAguaAnterior.Hours, (TempoAguaAnterior.Minutes + TempoAgua), TempoAguaAnterior.Seconds, TempoAguaAnterior.Milliseconds);
            TimeSpan resultadoAgua = teste.Subtract(testeAgua);

            if(resultadoSentado.Minutes == TempoPeValida)
                validaTesteSentado = true;

            if (resultadoAgua.Minutes == TempoPeValida)
                validaTesteAgua = true;

            if (!ValidaPrimeiraVoltaPe && TempoSentado.Minutes == TempoPeValida && TempoSentado.Minutes != 0 && TempoSentado.Milliseconds > 1 && TempoSentado.Milliseconds <= 600 && !FoiNotificadoPe)
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("Icon.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Ficar de pé";
                notifyIcon1.BalloonTipText = "Está na hora de você ficar de pé.";
                notifyIcon1.ShowBalloonTip(100);
                FoiNotificadoPe = true;
                ValidaPrimeiraVoltaPe = true;
                TempoSentadoAnterior = TempoSentado;
            }
            else if((validaTesteSentado && TempoSentado.Minutes != 0 && TempoSentado.Milliseconds > 1 && TempoSentado.Milliseconds <= 600 && !FoiNotificadoPe))
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("Icon.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Ficar de pé";
                notifyIcon1.BalloonTipText = "Está na hora de você ficar de pé.";
                notifyIcon1.ShowBalloonTip(100);
                FoiNotificadoPe = true;
                TempoSentadoAnterior = TempoSentado;
            }

            if (!ValidaPrimeiraVoltaAgua && teste.Minutes == TempoAgua && teste.Minutes != 0 && teste.Milliseconds > 1 && teste.Milliseconds <= 600 && !FoiNotificadoAgua)
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("sitguyNOTIF.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Tomar água";
                notifyIcon1.BalloonTipText = "Está na hora de você tomar água";
                notifyIcon1.ShowBalloonTip(100);
                FoiNotificadoAgua = true;
                ValidaPrimeiraVoltaAgua = true;
                TempoAguaAnterior = teste;
            }
            else if (validaTesteAgua && teste.Minutes != 0 && teste.Milliseconds > 1 && teste.Milliseconds <= 600 && !FoiNotificadoAgua)
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("sitguyNOTIF.ico"));
                notifyIcon1.Text = "";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "ERGO - Tomar água";
                notifyIcon1.BalloonTipText = "Está na hora de você tomar água";
                notifyIcon1.ShowBalloonTip(100);
                FoiNotificadoAgua = true;
                TempoAguaAnterior = teste;
            }

            if(TempoSentado.Minutes != 0 && validaTesteSentado)
            {
                FoiNotificadoPe = false;
            }

            if (teste.Minutes != 0 && validaTesteAgua)
            {
                FoiNotificadoAgua = false;
            }
        }

        #region Abas
        private void AbaConfiguracoes(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Configurações";

            FirebaseResponse res = Client.Get(@"UsersConfig/" + txtUsuario.Text);
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

            minutosLevantar.Text = resUser.TempoPe.ToString();

            salvarConfig.Visible = true;
            tempoPeTitle.Visible = true;
            minutosLevantar.Visible = true;

            TitleIdioma.Visible = true;
            comboBoxIdioma.Visible = true;

            titleNotifWindows.Visible = true;
            pnlNotificacao.Visible = true;

            titleSegundoPlano.Visible = true;
            pnlSegundoPlano.Visible = true;

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Cronometro
            CronometroSentado.Visible = false;
            CronometroPe.Visible = false;

            titleNaoSentado.Visible = false;
            titleSentado.Visible = false;

            BtnGerarGraficos.Visible = false;
            pnlConfigs.Visible = false;
        }

        private void AbaPerfil(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Perfil";
            pnlPerfil.Visible = true;

            FirebaseResponse res = Client.Get(@"UsersComplete/" + txtUsuario.Text);
            PerfilUsuario resUser = res.ResultAs<PerfilUsuario>();

            nameTxt.Text = resUser.NameComplete;
            emailTxt.Text = resUser.Email;
            dtaNascimento.Value = resUser.BornDate.Date;


            telefoneTxt.Text = resUser.Phone.ToString();

            if (resUser.ProfilePic != null)
            {
                byte[] imageConvert = (byte[])Convert.ChangeType(resUser.ProfilePic, typeof(byte[]));
                var bitmap = principalModulo.ConverteByteParaImagem(imageConvert);
                ImageBox1.Image = bitmap;
            }
            else
            {
                ImageBox1.Image = null;
            }

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
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

        private void AbaCronometragem(object sender, EventArgs e)
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

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
            tempoPeTitle.Visible = false;
            minutosLevantar.Visible = false;
            TitleIdioma.Visible = false;
            comboBoxIdioma.Visible = false;
            titleNotifWindows.Visible = false;
            pnlNotificacao.Visible = false;
            titleSegundoPlano.Visible = false;
            pnlSegundoPlano.Visible = false;
        }

        private void AbaGraficos(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Gráficos";
            pnlGrafico.Visible = true;
            graficoChart.Visible = false;
            textBoxResultado.Text = "";
            listGraficos.Items.Clear();

            double j = 1;
            FirebaseResponse resGrafico = Client.Get($@"Graficos/{txtUsuario.Text}/{j}");
            GraficosModel resUser = resGrafico.ResultAs<GraficosModel>();

            if (resGrafico.Body != "null")
            {
                if (listGraficos.Items.Count == 0)
                    listGraficos.Items.Add(resUser.NomeGrafico);

                for (j = 2; resGrafico.Body != "null"; ++j)
                {
                    resGrafico = Client.Get($@"Graficos/{txtUsuario.Text}/{j}");
                    if (resGrafico.Body != "null")
                    {
                        resUser = resGrafico.ResultAs<GraficosModel>();
                        listGraficos.Items.Add(resUser.NomeGrafico);
                    }
                }
            }

            //Controle de visuais de Dicas
            pnlDicas.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
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

        private void AbaDicas(object sender, EventArgs e)
        {
            Title.Visible = true;
            Title.Text = "Dicas E-book";
            pnlDicas.Visible = true;

            //Controle de visuais de Gráfico:
            pnlGrafico.Visible = false;

            //Controle de visuais Perfil:
            pnlPerfil.Visible = false;

            //Controle de visuais Configurações
            salvarConfig.Visible = false;
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

        #endregion
    }
}
