using Bunifu.UI.WinForms;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace InterfaceErgonomico.Models.Graficos
{
    public class GraficosModel
    {
        public string Username { get; set; }
        public bool Pizza { get; set; }
        public TimeSpan TempoSentado { get; set; }
        public TimeSpan TempoPe { get; set; }
        public double? Count { get; set; }
        public string NomeGrafico { get; set; }
    }

    public class GeraGraficosRequest
    {
        public IFirebaseClient Client { get; set; }
        public BunifuLabel TxtUsuario { get; set; }
        public Chart GraficoChart { get; set; }
        public double? Contagem { get; set; }
    }

    public class NotificaGeraGraficosRequest
    {
        public IFirebaseClient Client { get; set; }
        public BunifuLabel TxtUsuario { get; set; }
        public TimeSpan TempoSentado { get; set; } 
        public TimeSpan TempoPe { get; set; }
        public RadioButton RadioPizza { get; set; }
    }

    public class GeraGraficosResult
    {
        public string TextBoxResultado { get; set; }
        public Chart GraficoChart { get; set; }
    }
}
