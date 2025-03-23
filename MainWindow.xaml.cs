using AI_Lawyer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AI_Lawyer.Services;

namespace AI_Lawyer
{
    public class CaseData : INotifyPropertyChanged
    {
        private string _caseDescription;
        public string CaseDescription
        {
            get { return _caseDescription; }
            set { _caseDescription = value; OnPropertyChanged("CaseDescription"); }
        }

        private ObservableCollection<string> _analysisResults = new ObservableCollection<string>();
        public ObservableCollection<string> AnalysisResults
        {
            get { return _analysisResults; }
            set { _analysisResults = value; OnPropertyChanged("AnalysisResults"); }
        }

        public void AnalyzeCase()
        {
            //Логика ИИ
            AnalysisResults.Add("Найдена лазейка: Доказательства добыты незаконно.");
            AnalysisResults.Add("Рекомендация: Оспорить допустимость доказательств.");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OpenAIService _openAIService = new OpenAIService();
        public CaseData ViewModel { get; set; } = new CaseData();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            LoadLaws();
        }

        private void LoadLaws()
        {
            var laws = LawDatabase.GetLaws();
            LawsListBox.ItemsSource = laws;
        }

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnalyzeCase();

            string caseText = ViewModel.CaseDescription;
            if (!string.IsNullOrWhiteSpace(caseText))
            {
                string advice = await _openAIService.GetLegalAdviceAsync(caseText);
                ViewModel.AnalysisResults.Add($"Консультация AI: {advice}");
            }
            else
            {
                MessageBox.Show("Введите описание дела перед запросом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
