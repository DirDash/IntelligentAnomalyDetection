using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using WaveletAnomalyDetection;

namespace AnomalyDetectionApplication
{
    /// <summary>
    /// Interaction logic for StatisticsModelGenerationWindow.xaml
    /// </summary>
    public partial class StatisticsModelGenerationWindow : Window
    {
        private MainWindow _mainWindow { get; set; }

        private StatisticsModelGenerator _statisticsModelGenerator { get; set; }

        private string _normalSetFileName { get; set; }

        public StatisticsModelGenerationWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _statisticsModelGenerator = new StatisticsModelGenerator();

            InitializeComponent();

            foreach (var statisticsModel in _statisticsModelGenerator.SupportedWaveletTransformationStatisticsModels)
            {
                StatisticsModelComboBox.Items.Add(statisticsModel);
            }
            StatisticsModelComboBox.SelectedIndex = 0;

            foreach (var wavelet in _statisticsModelGenerator.SupportedWavelets)
            {
                WaveletComboBox.Items.Add(wavelet);
            }
            WaveletComboBox.SelectedIndex = 1;
        }

        private void UpdateInterface()
        {
            if (!string.IsNullOrEmpty(_normalSetFileName))
            {
                NormalSeLabel.Content = _normalSetFileName;
            }
        }

        private void SetStatus(string status)
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

        private void SaveStatisticsModel()
        {
            try
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.DefaultExt = ".sm";
                fileDialog.Filter = "Statistics model (.sm)|*.sm";
                fileDialog.FileName = $"statistics_model.sm";

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Сохранение модели...");

                    _statisticsModelGenerator.SaveStatisticsModelToFile(fileDialog.FileName);

                    UpdateMainWindow(Path.GetFileName(fileDialog.FileName));

                    Close();
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при попытке сохранения файла.");
            }

            SetStatus("");
        }

        private void UpdateMainWindow(string statisticskModelFileName)
        {
            _mainWindow.DetectionEngine.StatisticsModel = _statisticsModelGenerator.StatisticsModel;
            _mainWindow.StatisticsModelFileName = statisticskModelFileName;
            _mainWindow.UpdateInterface();
        }


        #region Events

        private void StatisticsModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _statisticsModelGenerator.WaveletTransformationStatisticsModel = (IWaveletTransformationStatisticsModel)StatisticsModelComboBox.SelectedItem;
        }

        private void WaveletComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _statisticsModelGenerator.Wavelet = (IWavelet)WaveletComboBox.SelectedItem;
        }

        private void LoadNormalSetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Загрузка выборки...");
                    _normalSetFileName = Path.GetFileName(fileDialog.FileName);

                    _statisticsModelGenerator.LoadNormalSetFromFile(fileDialog.FileName);

                    GenerateStatisticsModelButton.IsEnabled = true;

                    UpdateInterface();
                }
            }
            catch (Exception exception)
            {
                GenerateStatisticsModelButton.IsEnabled = false;

                ShowException(exception, "Ошибка при попытке загрузки файла.");
            }

            SetStatus("");
        }

        private void GenerateStatisticsModelButton_Click(object sender, RoutedEventArgs e)
        {
            SetStatus("Генерация модели...");

            try
            {
                _statisticsModelGenerator.GenerateStatisticsModel();

                SaveStatisticsModel();
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при генерации статистической модели.");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWindow.SetStatus("");
        }

        #endregion
    }
}