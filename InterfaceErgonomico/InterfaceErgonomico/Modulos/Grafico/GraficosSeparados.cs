using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceErgonomico.Models
{
    public class GraficosSeparadosPizza
    {
        public string DataGrafico { get; set; }
        public string TituloColuna { get; set; }
        public TimeSpan ValorTempo { get; set; }
        public string TempoFormatado { get; set; }
        public GraficosSeparadosPizza() { }

        public GraficosSeparadosPizza(string dataGrafico, string tituloColuna, TimeSpan valorTempo, string tempoFormatado)
        {
            DataGrafico = dataGrafico;
            TituloColuna = tituloColuna;
            ValorTempo = valorTempo;
            TempoFormatado = tempoFormatado;
        }

        public List<GraficosSeparadosPizza> Pizza(string dataGrafico, TimeSpan valorTempoSentado, TimeSpan valorTempoPe)
        {
            var colunas = new List<GraficosSeparadosPizza>();
            var tempoFormatadoSentado = string.Format("{0:hh\\:mm\\:ss\\:ff}", valorTempoSentado);
            var tempoFormatadoPe = string.Format("{0:hh\\:mm\\:ss\\:ff}", valorTempoPe);

            colunas.Add(new GraficosSeparadosPizza(dataGrafico, "Tempo Sentado", valorTempoSentado, tempoFormatadoSentado));
            colunas.Add(new GraficosSeparadosPizza(dataGrafico, "Tempo de Pé", valorTempoPe, tempoFormatadoPe));

            return colunas;
        }

        public string[] GetNomeColunas(List<GraficosSeparadosPizza> pizza)
        {
            string[] colunas = new string[pizza.Count];

            for (int i = 0; i < pizza.Count; i++)
                colunas[i] = $@"{pizza[i].TituloColuna} = {pizza[i].TempoFormatado}";

            return colunas;
        }

        public double[] GetValoresColunas(List<GraficosSeparadosPizza> pizza)
        {
            double[] valores = new double[pizza.Count];

            for (int i = 0; i < pizza.Count; i++)
                valores[i] = (pizza[i].ValorTempo.Hours + 
                              pizza[i].ValorTempo.Minutes / 
                              60.0 + pizza[i].ValorTempo.Seconds / 
                              3600.0 + pizza[i].ValorTempo.Milliseconds / 
                              3.6e+6) * (pizza[i].ValorTempo > TimeSpan.Zero ? 1 : -1);

            return valores;
        }
    }
}
