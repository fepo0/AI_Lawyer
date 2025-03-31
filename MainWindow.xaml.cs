using AI_Lawyer.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;


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

        private ObservableCollection<string> _matchedLaws = new ObservableCollection<string>();
        public ObservableCollection<string> MatchedLaws
        {
            get { return _matchedLaws; }
            set { _matchedLaws = value; OnPropertyChanged("MatchedLaws"); }
        }

        public void AnalyzeCase()
        {
            AnalysisResults.Clear();
            AnalysisResults.Add("Найдена лазейка: Доказательства добыты незаконно.");
            AnalysisResults.Add("Рекомендация: Оспорить допустимость доказательств.");

            MatchedLaws.Clear();
            var laws = Data.LawDatabase.SearchLaws(CaseDescription);
            foreach (var law in laws)
            {
                MatchedLaws.Add(law);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CaseData ViewModel { get; set; } = new CaseData();
        private readonly AIService aIService = new AIService();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            LoadLaws();
        }

        private void LoadLaws()
        {
            var laws = LawDatabase.GetLaws();
            ViewModel.MatchedLaws.Clear();
            foreach (var law in laws)
            {
                ViewModel.MatchedLaws.Add(law);
            }
        }
        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnalyzeCase();

            try
            {
                string recommendations = await aIService.GetRecommendationsAsync(ViewModel.CaseDescription);
                ViewModel.AnalysisResults.Clear();
                ViewModel.AnalysisResults.Add(recommendations);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
