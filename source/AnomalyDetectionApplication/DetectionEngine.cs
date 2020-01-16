using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using NeuralNetworkTools;
using WaveletAnomalyDetection;
using WaveletAnomalyDetection.AnomalyDetectionAlgorithms;
using WaveletAnomalyDetection.WaveletAnomalyDetectors;

namespace AnomalyDetectionApplication
{
    /// <summary>
    /// Интерфейс для взаимодействия с библиотеками WaveletAnomalyDetection и NeuralNetworkTools.
    /// </summary>
    public class DetectionEngine
    {
        /// <summary>
        /// Список названий параметров.
        /// </summary>
        public List<string> Features { get; private set; }

        /// <summary>
        /// Список объектов, заданных числовыми векторами.
        /// </summary>
        public List<List<double>> Samples { get; private set; }

        /// <summary>
        /// Детектор аномалий на основе дискретного вейвлет преобразования.
        /// </summary>
        public DiscreteWaveletTransformationAnomalyDetector WaveletTransformationAnomalyDetector { get; set; }

        /// <summary>
        /// Статистическая модель для обнаружения аномалий.
        /// </summary>
        public MultiFeatureStatisticsModel StatisticsModel { get; set; }

        /// <summary>
        /// Нейросетевая модель для обнаружения аномалий.
        /// </summary>
        public INeuralNetwork NeuralNetworkModel { get; set; }

        private List<string> _statisticsResults { get; set; }
        private List<double> _neuralNetworkResult { get; set; }

        public DetectionEngine()
        {
            Features = new List<string>();
            Samples = new List<List<double>>();
            _statisticsResults = new List<string>();
            _neuralNetworkResult = new List<double>();

            WaveletTransformationAnomalyDetector = new DiscreteWaveletTransformationAnomalyDetector(new List<IAnomalyDetectionAlgorithm>());
        }

        /// <summary>
        /// Загрузка данных из файла в списки Samples и Features.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void LoadDataFromFile(string filePath)
        {
            Features.Clear();
            Samples.Clear();

            using (var streamReader = new StreamReader(filePath))
            {
                var line = streamReader.ReadLine();
                foreach (var featureName in line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Features.Add(featureName);
                }

                while ((line = streamReader.ReadLine()) != null)
                {
                    var sample = new List<double>();

                    foreach (var feature in line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        sample.Add(double.Parse(feature));
                    }

                    Samples.Add(sample);
                }

                if (Features.Count != Samples.First().Count)
                {
                    Features.Clear();

                    for (var i = 1; i <= Samples.First().Count; i++)
                    {
                        Features.Add(i.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Загрузка статистической модели из файла в поле StatisticsModel.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void LoadStatisticsModelFromFile(string filePath)
        {
            var binaryFormatter = new BinaryFormatter();

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                StatisticsModel = (MultiFeatureStatisticsModel)binaryFormatter.Deserialize(fileStream);
            }
        }

        /// <summary>
        /// Загрузка нейросетевой модели из файла в поле NeuralNetworkModel.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void LoadNeuralNetworkModelFromFile(string filePath)
        {
            var binaryFormatter = new BinaryFormatter();

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                NeuralNetworkModel = (INeuralNetwork)binaryFormatter.Deserialize(fileStream);
            }
        }

        /// <summary>
        /// Выбор статистических алгоритмов обнаружения аномалий.
        /// </summary>
        /// <param name="cochraneCoxApproximations">Добавить критерий Кохрана-Кокса (средние значения).</param>
        /// <param name="fischerApproximations">Добавить критерий Фишера (средние значения).</param>
        /// <param name="fischerDispersions">Добавить критерий Фишера (дисперсия).</param>
        public void SelectStatisticsAlgorythms(bool cochraneCoxApproximations, bool fischerApproximations, bool fischerDispersions)
        {
            WaveletTransformationAnomalyDetector.AnomalyDetectionAlgorithms.Clear();
            if (cochraneCoxApproximations)
            {
                WaveletTransformationAnomalyDetector.AnomalyDetectionAlgorithms.Add(new CochraneCoxCriterionForApproximations());
            }

            if (fischerApproximations)
            {
                WaveletTransformationAnomalyDetector.AnomalyDetectionAlgorithms.Add(new FischerCriterionForApproximations());
            }

            if (fischerDispersions)
            {
                WaveletTransformationAnomalyDetector.AnomalyDetectionAlgorithms.Add(new FischerCriterionForDispersions());
            }
        }

        /// <summary>
        /// Поиск аномалий на основе статистической модели.
        /// </summary>
        /// <returns>Список (возможно, пустой) сообщений статистических алгоритмов обнаружения аномалий.</returns>
        public List<string> SearchAnomaliesByStatistics()
        {
            var featureSets = new List<List<double>>();
            foreach (var feature in Samples.First())
            {
                featureSets.Add(new List<double>());
            }

            foreach (var sample in Samples)
            {
                for (var i = 0; i < sample.Count; i++)
                {
                    featureSets[i].Add(sample[i]);
                }
            }

            _statisticsResults.Clear();

            for (var i = 0; i < featureSets.Count; i++)
            {
                var anomalyDetectionResultsForFeature = WaveletTransformationAnomalyDetector.CheckOnAnomaly(featureSets[i], StatisticsModel.FeatureStatisticsModels[i]);

                foreach (var result in  anomalyDetectionResultsForFeature)
                {
                    if (result.Type == AnomalyDetectionResultType.Anomaly || result.Type == AnomalyDetectionResultType.HighProbabilityOfAnomaly)
                    {
                        _statisticsResults.Add($"Параметр №{i + 1}: ({result.Source}) {result.Message} (Значение: {result.StatisticsValue}, порог: {result.StatisticsLimit})");
                    }
                }
            }

            return _statisticsResults;
        }

        /// <summary>
        /// Поиск аномалий на основе статистической модели.
        /// </summary>
        /// <returns>Список (возможно, пустой) сообщений об аномалиях.</returns>
        public List<string> SearchAnomaliesByNeuralNetwork()
        {
            var messages = new List<string>();

            _neuralNetworkResult = new List<double>();
            for (var i = 0; i < Samples.Count; i++)
            {
                var output = NeuralNetworkModel.Calculate(Samples[i])[0];
                _neuralNetworkResult.Add(output);
                if (output > 0.5)
                {
                    var probability = (output + 1) / 2;
                    messages.Add($"№{i + 1}: обнаружена аномалия с вероятностью {probability}.");
                }                
            }

            return messages;
        }

        /// <summary>
        /// Сохранение результатов статистического обнаружения аномалий в файл.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void SaveStatisticsResult(string filePath)
        {
            using (var streamWriter = new StreamWriter(filePath))
            {
                for (var i = 0; i < _statisticsResults.Count; i++)
                {
                    streamWriter.WriteLine(_statisticsResults[i]);
                }
            }
        }

        /// <summary>
        /// Сохранение результатов нейросетевого обнаружения аномалий в файл.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void SaveNeuralNetworkResult(string filePath)
        {
            using (var streamWriter = new StreamWriter(filePath))
            {
                for (var i = 0; i < Samples.Count; i++)
                {
                    var result = string.Join(";", Samples[i]);
                    result += " ";
                    result += _neuralNetworkResult[i].ToString();

                    streamWriter.WriteLine(result);
                }
            }
        }
    }
}