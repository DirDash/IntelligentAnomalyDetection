using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using NeuralNetworkTools;
using NeuralNetworkTools.LearningAlgorythms;

namespace AnomalyDetectionApplication
{
    /// <summary>
    /// Interaction logic for NeuralNetworkModelGenerationWindow.xaml
    /// </summary>
    public partial class NeuralNetworkModelGenerationWindow : Window
    {
        private MainWindow _mainWindow { get; set; }

        private NeuralNetworkModelGenerator _neuralNetworkGenerator { get; set; }

        private string _trainingSetFileName { get; set; }
        private string _testingSetFileName { get; set; }

        public NeuralNetworkModelGenerationWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _neuralNetworkGenerator = new NeuralNetworkModelGenerator();

            InitializeComponent();

            foreach (var neuralNetwork in _neuralNetworkGenerator.SupportedNeuralNetworks)
            {
                NeuralNetworkComboBox.Items.Add(neuralNetwork);
            }
            NeuralNetworkComboBox.SelectedIndex = 0;

            foreach (var neuronBuilder in _neuralNetworkGenerator.SupportedNeuronBuilders)
            {
                NeuronComboBox.Items.Add(neuronBuilder);
            }
            NeuronComboBox.SelectedIndex = 0;

            foreach (var learningAlgorythm in _neuralNetworkGenerator.SupportedLearningAlgorythms)
            {
                LearningAlgorythmComboBox.Items.Add(learningAlgorythm);
            }
            LearningAlgorythmComboBox.SelectedIndex = 0;
        }

        private void UpdateInterface()
        {
            if (!string.IsNullOrEmpty(_trainingSetFileName))
            {
                TrainingSetLabel.Content = _trainingSetFileName;
            }

            if (!string.IsNullOrEmpty(_testingSetFileName))
            {
                TestingSetLabel.Content = _testingSetFileName;
            }

            if (_neuralNetworkGenerator.NeuralNetwork != null && !string.IsNullOrEmpty(_trainingSetFileName) && !string.IsNullOrEmpty(_testingSetFileName))
            {
                TrainNeuralNetworkButton.IsEnabled = true;
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

        private void SaveNeuralNetworkModel()
        {
            try
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.DefaultExt = ".nnm";
                fileDialog.Filter = "Neural network models (.nnm)|*.nnm";
                fileDialog.FileName = $"neural_network_model.nnm";

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Сохранение модели...");

                    _neuralNetworkGenerator.SaveNeuralNetworkModelToFile(fileDialog.FileName);

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

        private void UpdateMainWindow(string neuralNetworkModelFileName)
        {
            _mainWindow.DetectionEngine.NeuralNetworkModel = _neuralNetworkGenerator.NeuralNetwork;
            _mainWindow.NeuralNetworkModelFileName = neuralNetworkModelFileName;
            _mainWindow.UpdateInterface();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWindow.SetStatus("");
        }

        #region Events

        private void NeuralNetworkComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _neuralNetworkGenerator.NeuralNetwork = (INeuralNetwork)NeuralNetworkComboBox.SelectedItem;
        }

        private void NeuronComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _neuralNetworkGenerator.NeuronBuilder = (INeuronBuilder)NeuronComboBox.SelectedItem;
        }

        private void AddLayerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var neuronAmount = int.Parse(NeuronAmountTextBox.Text);
                var synapticConnectionAmount = int.Parse(SynapticConnectionAmountTextBox.Text);

                _neuralNetworkGenerator.AddLayer(neuronAmount, synapticConnectionAmount);

                NeuralNetworkLayersTextBox.Text += $"{_neuralNetworkGenerator.NeuralNetwork.Layers.Count()}. Количество нейронов: {neuronAmount}; количество синаптических связей: {synapticConnectionAmount}.";
                NeuralNetworkLayersTextBox.Text += Environment.NewLine;

                SynapticConnectionAmountTextBox.IsEnabled = false;
                SynapticConnectionAmountTextBox.Text = neuronAmount.ToString();

                GenerateNeuralNetworkButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при добавлении слоя искусственных нейронов.");
            }
        }

        private void GenerateNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _neuralNetworkGenerator.GenerateneuralNetworkModel();

                var modelName = "neural_network_model";
                foreach (var layer in _neuralNetworkGenerator.NeuralNetwork.Layers)
                {
                    modelName += $"_{layer.Count}";
                }
                ModelNameLabel.Content = modelName;

                LearningAlgorythmComboBox.IsEnabled = true;
                MomentumTextBox.IsEnabled = true;
                MaxEpochAmountTextBox.IsEnabled = true;
                LearningSpeedTextBox.IsEnabled = true;
                LoadTrainingSetButton.IsEnabled = true;
                LoadValidationSetButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                ModelNameLabel.Content = "";

                LearningAlgorythmComboBox.IsEnabled = false;
                MomentumTextBox.IsEnabled = false;
                MaxEpochAmountTextBox.IsEnabled = false;
                LearningSpeedTextBox.IsEnabled = false;
                LoadTrainingSetButton.IsEnabled = false;
                LoadValidationSetButton.IsEnabled = false;

                ShowException(exception, "Ошибка при генерации нейросетевой модели.");
            }
        }

        private void LearningAlgorythmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _neuralNetworkGenerator.LearningAlgorythm = (ILearningAlgorythm)LearningAlgorythmComboBox.SelectedItem;
        }

        private void LearningSpeedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(LearningSpeedTextBox.Text, out var value))
            {
                if (value < 0.000001)
                    value = 0.000001;

                if (_neuralNetworkGenerator?.NeuralNetwork != null)
                {
                    _neuralNetworkGenerator.NeuralNetwork.LearningSpeed = value;
                }
            }
        }

        private void MomentumTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(MomentumTextBox.Text, out var value))
            {
                if (value < 0)
                    value = 0;

                if (_neuralNetworkGenerator?.LearningAlgorythm != null && _neuralNetworkGenerator.LearningAlgorythm is BackpropagationLearningAlgorythm)
                {
                    (_neuralNetworkGenerator.LearningAlgorythm as BackpropagationLearningAlgorythm).Momentum = value;
                }
            }
        }

        private void MaxEpochAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(MaxEpochAmountTextBox.Text, out var value))
            {
                if (value < 1)
                    value = 1;

                if (_neuralNetworkGenerator?.LearningAlgorythm != null && _neuralNetworkGenerator.LearningAlgorythm is BackpropagationLearningAlgorythm)
                {
                    (_neuralNetworkGenerator.LearningAlgorythm as BackpropagationLearningAlgorythm).LearningEpochMaxAmount = value;
                }
            }
        }

        private void LoadTrainingSetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Загрузка выборки...");
                    _trainingSetFileName = Path.GetFileName(fileDialog.FileName);

                    _neuralNetworkGenerator.LoadTrainingSetFromFile(fileDialog.FileName);

                    UpdateInterface();
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при попытке загрузки файла.");
            }

            SetStatus("");
        }

        private void LoadValidationSetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();

                if (fileDialog.ShowDialog() == true)
                {
                    SetStatus("Загрузка выборки...");
                    _testingSetFileName = Path.GetFileName(fileDialog.FileName);

                    _neuralNetworkGenerator.LoadValidationSetFromFile(fileDialog.FileName);

                    UpdateInterface();
                }
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при попытке загрузки файла.");
            }

            SetStatus("");
        }

        private void TrainNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            SetStatus("Обучение...");

            try
            {
                _neuralNetworkGenerator.Learn();

                SaveNeuralNetworkModel();
            }
            catch (Exception exception)
            {
                ShowException(exception, "Ошибка при обучении нейросетевой модели");
            }
        }

        #endregion
    }
}