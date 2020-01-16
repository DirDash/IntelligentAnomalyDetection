using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AnomalyDetectionApplication
{
    public partial class MainWindow : Window
    {
        public DetectionEngine DetectionEngine { get; }

        public string StatisticsModelFileName { get; set; }
        public string NeuralNetworkModelFileName { get; set; }

        private string _testingSetFileName { get; set; }
        private List<string> _statisticsSearchResults { get; set; }
        private List<string> _neuralNetworkSearchResults { get; set; }

        private bool _isInitialized = false;        

        public MainWindow()
        {
            DetectionEngine = new DetectionEngine();
            _statisticsSearchResults = new List<string>();
            _neuralNetworkSearchResults = new List<string>();

            InitializeComponent();

            DetectionEngine.SelectStatisticsAlgorythms(CochraneCoxApproximationsCheckBox.IsChecked == true, FischerApproximationsCheckBox.IsChecked == true, FischerDispersionCheckBox.IsChecked == true);

            _isInitialized = true;

            UpdateInterface();
        }

        public void UpdateInterface()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_testingSetFileName))
            {
                FilenameLabel.Content = _testingSetFileName;
                ObjectAmountLabel.Content = DetectionEngine.Samples.Count;
                FeatureAmountLabel.Content = DetectionEngine.Features.Count;
            }

            if (!string.IsNullOrEmpty(StatisticsModelFileName))
            {
                StatisticsModelFileNameLabel.Content = StatisticsModelFileName;
            }

            if (!string.IsNullOrEmpty(NeuralNetworkModelFileName))
            {
                NeuralNetworkModelNameLabel.Content = NeuralNetworkModelFileName;
            }

            if (!string.IsNullOrEmpty(_testingSetFileName))
            {
                SamplesTextBox.IsEnabled = true;


                if (!string.IsNullOrEmpty(StatisticsModelFileName))
                {
                    CochraneCoxApproximationsCheckBox.IsEnabled = true;
                    FischerApproximationsCheckBox.IsEnabled = true;
                    FischerDispersionCheckBox.IsEnabled = true;

                    SensivityTextBox.IsEnabled = true;

                    SeachAnomaliesByStatisticsButton.IsEnabled = true;

                    DisplayStatisticsResultsBox.IsEnabled = true;
                }

                if (!string.IsNullOrEmpty(NeuralNetworkModelFileName))
                {
                    SeachAnomaliesByNeuralNetworkButton.IsEnabled = true;

                    DisplayNeuralNetworkResultsBox.IsEnabled = true;
                }
            }
        }

        public void SetStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                StatusLbl.Content = "";
                StatusLbl.Visibility = Visibility.Hidden;
            }
            else
            {
                StatusLbl.Content = status.ToUpper();
                StatusLbl.Visibility = Visibility.Visible;
                AllowInterfaceToUpdate();
            }
        }       

        private void AllowInterfaceToUpdate()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }
        
        private void ShowException(Exception exception, string message = "")
        {
            MessageBox.Show($"{message}{Environment.NewLine}{exception.Message}{Environment.NewLine}Подробнее: https://github.com/DirDash/IntelligentAnomalyDetection");
        }

        private void UpdateSampleInfo()
        {
            SamplesTextBox.Clear();

            if (DisplaySamplesCheckBox.IsChecked != true || DetectionEngine.Samples.Count == 0)
            {
                return;
            }

            SetStatus("Вывод результатов...");

            for (var i = 0; i < DetectionEngine.Samples.Count; i++)
            {
                var sampleToString = "";
                if (FeatureComboBox.SelectedIndex > 0)
                {
                    sampleToString += $"{DetectionEngine.Samples[i][FeatureComboBox.SelectedIndex - 1].ToString()}; ";
                }
                else
                {
                    foreach (var feature in DetectionEngine.Samples[i])
                    {
                        sampleToString += $"{feature.ToString()}; ";
                    }
                }

                SamplesTextBox.Text += $"№{i + 1}: {sampleToString}";
                SamplesTextBox.Text += Environment.NewLine;
                SamplesTextBox.Text += Environment.NewLine;
            }

            SetStatus("");
        }

        private void UpdateStatisticsResults()
        {
            StatisticsSearchResultTextBox.Clear();

            if (DisplayStatisticsResultsBox.IsChecked != true)
            {
                return;
            }

            if (StatisticsSearchResultTextBox.IsEnabled && _statisticsSearchResults.Count == 0)
            {
                StatisticsSearchResultTextBox.Text = "Аномалии не обнаружены.";
                return;
            }

            SetStatus("Вывод результатов...");

            foreach (var message in _statisticsSearchResults)
            {
                StatisticsSearchResultTextBox.Text += message;
                StatisticsSearchResultTextBox.Text += Environment.NewLine;
                StatisticsSearchResultTextBox.Text += Environment.NewLine;
            }
            
            SetStatus("");
        }

        private void UpdateNeuralNetworkResults()
        {
            NeuralNetworkSearchResultTextBox.Clear();

            if (DisplayNeuralNetworkResultsBox.IsChecked != true)
            {
                return;
            }

            SetStatus("Вывод результатов...");

            foreach (var result in _neuralNetworkSearchResults)
            {
                NeuralNetworkSearchResultTextBox.Text += result;
                NeuralNetworkSearchResultTextBox.Text += Environment.NewLine;
                NeuralNetworkSearchResultTextBox.Text += Environment.NewLine;
            }

            if (_neuralNetworkSearchResults.Count == 0)
            {
                NeuralNetworkSearchResultTextBox.Text = "Аномалии не обнаружены.";
            }

            SetStatus("");
        }


        #region Events

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Загрузка данных...");
                    _testingSetFileName = Path.GetFileName(fileDialog.FileName);

                    DetectionEngine.LoadDataFromFile(fileDialog.FileName);

                    FeatureComboBox.Items.Clear();
                    FeatureComboBox.Items.Add("Все параметры");
                    foreach (var featureName in DetectionEngine.Features)
                    {
                        FeatureComboBox.Items.Add(featureName);
                    }
                    FeatureComboBox.SelectedIndex = 0;

                    UpdateSampleInfo();
                    UpdateInterface();
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, $"Ошибка при попытке загрузки файла. Убедитесь, что файл имеет корректный формат.");
            }

            SetStatus("");
        }

        private void GenerateStatisticsModelButton_Click(object sender, RoutedEventArgs e)
        {
            var statisticsModelWindow = new StatisticsModelGenerationWindow(this);
            statisticsModelWindow.Show();

            SetStatus("Создание статистической модели...");
        }

        private void GenerateNeuralNetworkModelButton_Click(object sender, RoutedEventArgs e)
        {
            var neuralNetworkGenerationWindow = new NeuralNetworkModelGenerationWindow(this);
            neuralNetworkGenerationWindow.Show();

            SetStatus("Создание нейросетевой модели...");
        }

        private void LoadStatisticsModelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.DefaultExt = ".sm";
                fileDialog.Filter = "Statistics model (.sm)|*.sm";

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Загрузка статистической модели...");
                    StatisticsModelFileName = Path.GetFileName(fileDialog.FileName);

                    DetectionEngine.LoadStatisticsModelFromFile(fileDialog.FileName);

                    UpdateInterface();
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, $"Ошибка при попытке загрузки статистической модели. Убедитесь, что файл имеет корректный формат.");
            }

            SetStatus("");
        }

        private void LoadNeuralNetworkModelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.DefaultExt = ".nnm";
                fileDialog.Filter = "Neural network model (.nnm)|*.nnm";

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Загрузка нейросетевой модели...");
                    NeuralNetworkModelFileName = Path.GetFileName(fileDialog.FileName);

                    DetectionEngine.LoadNeuralNetworkModelFromFile(fileDialog.FileName);

                    UpdateInterface();
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, $"Ошибка при попытке загрузки нейросетевой модели. Убедитесь, что файл имеет корректный формат.");
            }

            SetStatus("");
        }

        private void SearchAnomaliesByStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            SetStatus("Поиск аномалий...");

            try
            {
                _statisticsSearchResults = DetectionEngine.SearchAnomaliesByStatistics();

                StatisticsSearchResultTextBox.IsEnabled = true;
                SaveStatisticsResultButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                StatisticsSearchResultTextBox.IsEnabled = false;
                SaveStatisticsResultButton.IsEnabled = false;

                ShowException(exception, "Ошибка при поиске аномалий. Проверьте корректность загруженной модели и исследуемых данных.");
            }

            UpdateStatisticsResults();

            SetStatus("");
        }

        private void SeachAnomaliesByNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            SetStatus("Поиск аномалий...");

            try
            {
                _neuralNetworkSearchResults = DetectionEngine.SearchAnomaliesByNeuralNetwork();

                NeuralNetworkSearchResultTextBox.IsEnabled = true;
                SaveNeuralNetworkResultButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                NeuralNetworkSearchResultTextBox.IsEnabled = false;
                SaveNeuralNetworkResultButton.IsEnabled = false;

                ShowException(exception, "Ошибка при поиске аномалий. Проверьте корректность загруженной модели и исследуемых данных.");
            }

            UpdateNeuralNetworkResults();

            SetStatus("");
        }

        private void SaveStatisticsResultButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.DefaultExt = ".txt";
                fileDialog.FileName = $"{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}_statistics_anomaly_detection_result.txt";

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Сохранение результатов...");

                    DetectionEngine.SaveStatisticsResult(fileDialog.FileName);
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при попытке сохранения файла.");
            }

            SetStatus("");
        }

        private void SaveNeuralNetworkResultButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.DefaultExt = ".txt";
                fileDialog.FileName = $"{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}_neural_network_anomaly_detection_result.txt";

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Сохранение результатов...");

                    DetectionEngine.SaveNeuralNetworkResult(fileDialog.FileName);
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при попытке сохранения файла.");
            }

            SetStatus("");
        }

        private void DisplaySamplesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DisplaySamplesCheckBox.IsChecked = true;
            FeatureComboBox.IsEnabled = true;

            UpdateSampleInfo();
        }

        private void DisplaySamplesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisplaySamplesCheckBox.IsChecked = false;
            FeatureComboBox.IsEnabled = false;

            UpdateSampleInfo();
        }

        private void DisplayStatisticsResultsBox_Checked(object sender, RoutedEventArgs e)
        {
            DisplayStatisticsResultsBox.IsChecked = true;

            UpdateStatisticsResults();
        }

        private void DisplayStatisticsResultsBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisplayStatisticsResultsBox.IsChecked = false;

            UpdateStatisticsResults();
        }

        private void DisplayNeuralNetworkResultsBox_Checked(object sender, RoutedEventArgs e)
        {
            DisplayNeuralNetworkResultsBox.IsChecked = true;

            UpdateNeuralNetworkResults();
        }

        private void DisplayNeuralNetworkResultsBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisplayNeuralNetworkResultsBox.IsChecked = false;

            UpdateNeuralNetworkResults();
        }

        private void FeatureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSampleInfo();
        }

        private void CochraneCoxApproximationsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }
            
            DetectionEngine.SelectStatisticsAlgorythms(true, FischerApproximationsCheckBox.IsChecked == true, FischerDispersionCheckBox.IsChecked == true);
        }

        private void CochraneCoxApproximationsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            DetectionEngine.SelectStatisticsAlgorythms(false, FischerApproximationsCheckBox.IsChecked == true, FischerDispersionCheckBox.IsChecked == true);
        }

        private void FischerApproximationsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            DetectionEngine.SelectStatisticsAlgorythms(CochraneCoxApproximationsCheckBox.IsChecked == true, true, FischerDispersionCheckBox.IsChecked == true);
        }

        private void FischerApproximationsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            DetectionEngine.SelectStatisticsAlgorythms(CochraneCoxApproximationsCheckBox.IsChecked == true, false, FischerDispersionCheckBox.IsChecked == true);
        }

        private void FischerDispersionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            DetectionEngine.SelectStatisticsAlgorythms(CochraneCoxApproximationsCheckBox.IsChecked == true, FischerApproximationsCheckBox.IsChecked == true, true);
        }

        private void FischerDispersionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            DetectionEngine.SelectStatisticsAlgorythms(CochraneCoxApproximationsCheckBox.IsChecked == true, FischerApproximationsCheckBox.IsChecked == true, false);
        }

        private void SensivityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(SensivityTextBox.Text, out var value))
            {
                if (value < 0.01)
                {
                    value = 0.01;
                    SensivityTextBox.Text = value.ToString();
                }
                DetectionEngine.WaveletTransformationAnomalyDetector.Sensitivity = value;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}