using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Vector_Iveco
{
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;
        private Thread _threadLeitura;
        private bool _executando = true;

        public MainWindow()
        {
            InitializeComponent();

            // Configurar porta serial (ajuste COMx conforme o Arduino)
            _serialPort = new SerialPort("COM5", 9600);
            _serialPort.Open();

            // Thread para leitura contínua
            _threadLeitura = new Thread(AtualizarLeituraSensor);
            _threadLeitura.IsBackground = true;
            _threadLeitura.Start();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AtualizarLeituraSensor()
        {
            while (_executando)
            {
                try
                {
                    string? linha = _serialPort.ReadLine(); // lê linha do Arduino
                    if (double.TryParse(linha.Replace(",", "."), out double percentual))
                    {
                        bool possuiAgua = percentual > 20; // ex: considera água se >20%

                        Dispatcher.Invoke(() =>
                        {
                            if (double.TryParse(txtDensidade.Text, out double densidade) &&
                                double.TryParse(txtBiodiesel.Text, out double biodiesel))
                            {
                                string resultado = AnalisarCombustivel(densidade, possuiAgua, biodiesel, percentual);
                                AplicarEstiloResultado(resultado);
                            }
                            else
                            {
                                txtResultado.Text = "Aguardando valores válidos...";
                                txtResultado.Background = System.Windows.Media.Brushes.LightGray;
                                txtResultado.Foreground = System.Windows.Media.Brushes.Black;
                            }
                        });
                    }
                }
                catch
                {
                    // erros de leitura são ignorados
                }
            }
        }

        private string AnalisarCombustivel(double densidade, bool possuiAgua, double biodiesel, double percentualAgua)
        {
            string resultado = $"DENSIDADE: {densidade} kg/m³\n";
            resultado += $"ÁGUA NO SENSOR: {(percentualAgua/63):F1}%\n";
            resultado += $"PRESENÇA DE ÁGUA: {(possuiAgua ? "SIM" : "NÃO")}\n";
            resultado += $"BIODIESEL: {biodiesel}%\n\n";

            if (possuiAgua)
            {
                if (densidade >= 815 && densidade <= 850)
                {
                    resultado += "⚠️ COMBUSTÍVEL CONTÉM ÁGUA!\n";
                    resultado += "Recomenda-se análise mais detalhada.\n";
                }
                else
                {
                    resultado += "❌ COMBUSTÍVEL DEVE SER SUBSTITUÍDO IMEDIATAMENTE!\n";
                    resultado += "RISCO DE DANOS AO MOTOR!\n\n";
                    resultado += "NÃO UTILIZE ESTE COMBUSTÍVEL!\n\n";
                    resultado += "CONTATOS PARA DESCARTE:\n";
                    resultado += "• Central de Descarte: (11) 3333-4444\n";
                    resultado += "• EMAE: (11) 2222-5555\n";
                    resultado += "• CETESB: 0800-113560";
                }
            }
            else
            {
                if (densidade >= 815 && densidade <= 850)
                {
                    resultado += "✅ COMBUSTÍVEL DE BOA QUALIDADE\n";
                    resultado += "Pronto para uso.\n";
                    resultado += $"Mistura Diesel/Biodiesel: {100 - biodiesel}%/{biodiesel}%";
                }
                else
                {
                    resultado += "⚠️ COMBUSTÍVEL DE BAIXA QUALIDADE\n";
                    resultado += "Pode ser utilizado, mas recomenda-se aditivação.\n";
                }
            }

            return resultado;
        }

        private void AplicarEstiloResultado(string resultado)
        {
            if (resultado.Contains("❌"))
            {
                txtResultado.Text = resultado;
                txtResultado.Background = System.Windows.Media.Brushes.Crimson;
                txtResultado.Foreground = System.Windows.Media.Brushes.White;
            }
            else if (resultado.Contains("⚠️"))
            {
                txtResultado.Text = resultado;
                txtResultado.Background = System.Windows.Media.Brushes.Yellow;
                txtResultado.Foreground = System.Windows.Media.Brushes.Black;
            }
            else if (resultado.Contains("✅"))
            {
                txtResultado.Text = resultado;
                txtResultado.Background = System.Windows.Media.Brushes.Green;
                txtResultado.Foreground = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtResultado.Text = resultado;
                txtResultado.Background = System.Windows.Media.Brushes.LightGray;
                txtResultado.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _executando = false;
            if (_serialPort.IsOpen) _serialPort.Close();
            base.OnClosed(e);
        }
    }
}