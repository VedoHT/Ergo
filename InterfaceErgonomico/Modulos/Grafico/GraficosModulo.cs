using Bunifu.UI.WinForms;
using FireSharp.Interfaces;
using FireSharp.Response;
using InterfaceErgonomico.Models;
using InterfaceErgonomico.Models.Graficos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace InterfaceErgonomico.Modulos.Grafico
{
    public class GraficosModulo
    {
        public void NotificaGeraGrafico(NotificaGeraGraficosRequest request)
        {
            FirebaseResponse res = request.Client.Get(@"UsersComplete/" + request.TxtUsuario.Text);
            PerfilUsuario resUser = res.ResultAs<PerfilUsuario>();

            double j = 1;
            FirebaseResponse resGrafico = request.Client.Get($@"Graficos/{request.TxtUsuario.Text}/{j}");

            if (resGrafico.Body != "null")
            {
                for (j = 2; resGrafico.Body != "null"; j++)
                    resGrafico = request.Client.Get($@"Graficos/{request.TxtUsuario.Text}/{j}");

                j -= 1;
            }

            var nomeGrafico = $"{j}";

            GraficosModel grafico = new GraficosModel()
            {
                Username = resUser.Username,
                TempoSentado = request.TempoSentado,
                TempoPe = request.TempoPe,
                Pizza = request.RadioPizza.Checked,
                Count = j,
                NomeGrafico = $@"{DateTime.Now.ToString("dd/MM/yyyy")}-{j}"
            };

            request.Client.Set($@"Graficos/{resUser.Username}/{nomeGrafico}", grafico);
            MessageBox.Show("Gráfico gerado com sucesso.");
        }

        public GeraGraficosResult GerarGrafico(GeraGraficosRequest request)
        {
            FirebaseResponse res = request.Client.Get($@"Graficos/{request.TxtUsuario.Text}/{request.Contagem}");
            GraficosModel resUser = res.ResultAs<GraficosModel>();
            GeraGraficosResult result = new GeraGraficosResult();
            string sentadoPe = "";
            result.GraficoChart = request.GraficoChart;

            //Grafico Pizza
            if (resUser.Pizza)
            {
                if (request.GraficoChart.Series.Count != 0)
                {
                    result.GraficoChart.Series.Clear();
                    result.GraficoChart.Titles.Clear();
                    result.GraficoChart.Legends.Clear();
                }

                result.GraficoChart.Visible = true;

                GraficosSeparadosPizza graficoPizza = new GraficosSeparadosPizza();
                var lista = graficoPizza.Pizza(resUser.NomeGrafico, resUser.TempoSentado, resUser.TempoPe);

                var colunas = graficoPizza.GetNomeColunas(lista);
                var valores = graficoPizza.GetValoresColunas(lista);

                //TituloPrincipal
                var titulo = new Title();
                titulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                titulo.ForeColor = Color.Gray;
                titulo.Text = $@"Gráfico: {resUser.NomeGrafico}";
                result.GraficoChart.Titles.Add(titulo);

                result.GraficoChart.Series.Add("Tempos");
                result.GraficoChart.Series[0].ChartType = SeriesChartType.Pie;

                //Série de dados com os valores do gráfico
                result.GraficoChart.Series[0].Points.DataBindXY(colunas, valores);

                if (TimeSpan.Compare(resUser.TempoPe, resUser.TempoSentado) == 1)
                {
                    sentadoPe = "em pé, indicando que você está cuidando de seu tempo em frente ao computador, continue assim!\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                else
                {
                    sentadoPe = "sentado, indicando que você necessita de pausas no tempo de uso do computador, para uma rotina mais saudável.\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                result.TextBoxResultado = $@"Com base no gráfico gerado, você está passando mais tempo {sentadoPe}";

            }
            else  //Grafico Colunas
            {
                if (request.GraficoChart.Series.Count != 0)
                {
                    result.GraficoChart.Series.Clear();
                    result.GraficoChart.Titles.Clear();
                    result.GraficoChart.Legends.Clear();
                }

                result.GraficoChart.Visible = true;

                GraficosSeparadosPizza graficoPizza = new GraficosSeparadosPizza();
                var lista = graficoPizza.Pizza(resUser.NomeGrafico, resUser.TempoSentado, resUser.TempoPe);

                var colunas = graficoPizza.GetNomeColunas(lista);
                var valores = graficoPizza.GetValoresColunas(lista);

                //TituloPrincipal
                var titulo = new Title();
                titulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                titulo.ForeColor = Color.Gray;
                titulo.Text = $@"Gráfico: {resUser.NomeGrafico}";
                result.GraficoChart.Titles.Add(titulo);

                result.GraficoChart.Series.Add("Tempos");
                result.GraficoChart.Series[0].ChartType = SeriesChartType.Column;

                result.GraficoChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

                //Série de dados com os valores do gráfico
                result.GraficoChart.Series[0].Points.DataBindXY(colunas, valores);

                if (TimeSpan.Compare(resUser.TempoPe, resUser.TempoSentado) == 1)
                {
                    sentadoPe = "em pé, indicando que você está cuidando de seu tempo em frente ao computador, continue assim!\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                else
                {
                    sentadoPe = "sentado, indicando que você necessita de pausas no tempo de uso do computador, para uma rotina mais saúdavel.\n    Para visualizar mais dicas sobre saúde, entre no menu 'Dicas E-book'";
                }
                result.TextBoxResultado = $@"Com base no gráfico gerado, você está passando mais tempo {sentadoPe}";
            }

            return result;
        }
    }
}
