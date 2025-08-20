using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vector_Iveco;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9.,]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void BtnAnalisar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Validar e converter entradas
            if (!double.TryParse(txtDensidade.Text, out double densidade))
            {
                MessageBox.Show("Por favor, insira um valor válido para densidade.", "Erro", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(txtBiodiesel.Text, out double biodiesel))
            {
                MessageBox.Show("Por favor, insira um valor válido para percentual de biodiesel.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool possuiAgua = cmbAgua.SelectedIndex == 1;

            // Realizar análise
            string resultado = AnalisarCombustivel(densidade, possuiAgua, biodiesel);
            txtResultado.Text = resultado;

            // Aplicar estilo baseado no resultado
            AplicarEstiloResultado(resultado);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao analisar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string AnalisarCombustivel(double densidade, bool possuiAgua, double biodiesel)
    {
        string resultado = $"DENSIDADE: {densidade} kg/m³\n";
        resultado += $"PRESENÇA DE ÁGUA: {(possuiAgua ? "SIM" : "NÃO")}\n";
        resultado += $"BIODIESEL: {biodiesel}%\n\n";

        if (possuiAgua)
        {
            if (densidade >= 815 && densidade <= 850)
            {
                resultado += "⚠️ COMBUSTÍVEL CONTÉM ÁGUA!\n";
                resultado += "Recomenda-se análise mais detalhada. O combustível apresenta água " +
                             "mas está dentro do padrão de densidade.\n";
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
                resultado += "Pronto para uso. Atende todos os parâmetros de qualidade.\n";
                resultado += $"Mistura Diesel/Biodiesel: {100 - biodiesel}%/{biodiesel}%";
            }
            else
            {
                resultado += "⚠️ COMBUSTÍVEL DE BAIXA QUALIDADE\n";
                resultado += "Pode ser utilizado, mas recomenda-se aditivação para melhorar a qualidade.\n";
                resultado += "Considere aditivos para:\n";
                resultado += "• Melhorar a lubrificação\n";
                resultado += "• Aumentar o poder calorífico\n";
                resultado += "• Prevenir formação de borra";
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
    }
}