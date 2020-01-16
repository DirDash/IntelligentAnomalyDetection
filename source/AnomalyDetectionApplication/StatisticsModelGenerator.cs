using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using WaveletAnomalyDetection;
using WaveletAnomalyDetection.StatisticsModels;
using WaveletAnomalyDetection.Wavelets;

namespace AnomalyDetectionApplication
{
    /// <summary>
    /// Интерфейс для генерации статистических моделей библиотекой WaveletAnomalyDetection.
    /// </summary>
    internal class StatisticsModelGenerator
    {
        /// <summary>
        /// Поддерживаемые типы статистических моделей.
        /// </summary>
        public List<IWaveletTransformationStatisticsModel> SupportedWaveletTransformationStatisticsModels { get; private set; }

        /// <summary>
        /// Тип статистической модели.
        /// </summary>
        public IWaveletTransformationStatisticsModel WaveletTransformationStatisticsModel { get; set; }

        /// <summary>
        /// Поддерживаемые типы вейвлетов.
        /// </summary>
        public List<IWavelet> SupportedWavelets { get; private set; }

        /// <summary>
        /// Вейвлет для генерации статистической модели.
        /// </summary>
        public IWavelet Wavelet { get; set; }

        /// <summary>
        /// Сгенерированная статистическая модель.
        /// </summary>
        public MultiFeatureStatisticsModel StatisticsModel { get; set; }

        private List<List<double>> _normalSet { get; set; }

        public StatisticsModelGenerator()
        {
            SupportedWaveletTransformationStatisticsModels = new List<IWaveletTransformationStatisticsModel>()
            {
                new WaveletTransformationStatisticsModel()
            };

            SupportedWavelets = new List<IWavelet>()
            {
                new HaarWavelet(),
                new D4Wavelet(),
                new D6Wavelet(),
                new D8Wavelet(),
            };

            _normalSet = new List<List<double>>();
        }

        /// <summary>
        /// Загрузка нормальной выборки из файла.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void LoadNormalSetFromFile(string filePath)
        {
            _normalSet.Clear();

            using (var streamReader = new StreamReader(filePath))
            {
                var line = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    _normalSet.Add(line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(feature => double.Parse(feature)).ToList());
                }
            }
        }

        /// <summary>
        /// Генерация статистической модели на основе выбранных параметров и загруженной нормальной выборки.
        /// </summary>
        public void GenerateStatisticsModel()
        {
            var featureSets = new List<List<double>>();
            foreach (var feature in _normalSet.First())
            {
                featureSets.Add(new List<double>());
            }

            foreach (var normalSample in _normalSet)
            {
                for (var i = 0; i < normalSample.Count; i++)
                {
                    featureSets[i].Add(normalSample[i]);
                }
            }

            StatisticsModel = new MultiFeatureStatisticsModel();

            foreach (var featureSet in featureSets)
            {
                StatisticsModel.FeatureStatisticsModels.Add(WaveletTransformator.GenerateStatisticsModel(featureSet, Wavelet));
            }
        }

        /// <summary>
        /// Сохранение статистической модели в файл.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void SaveStatisticsModelToFile(string filePath)
        {
            var binaryFormatter = new BinaryFormatter();

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                binaryFormatter.Serialize(fileStream, StatisticsModel);
            }
        }
    }
}