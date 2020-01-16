using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using NeuralNetworkTools;
using NeuralNetworkTools.LearningAlgorythms;
using NeuralNetworkTools.NeuralNetworks;
using NeuralNetworkTools.NeuronBuilders;

namespace AnomalyDetectionApplication
{
    /// <summary>
    /// Интерфейс для генерации нейросетевых моделей библиотекой NeuralNetworkTools.
    /// </summary>
    internal class NeuralNetworkModelGenerator
    {
        /// <summary>
        /// Поддерживаемые типы искусственных нейроных сетей.
        /// </summary>
        public List<INeuralNetwork> SupportedNeuralNetworks { get; private set; }

        /// <summary>
        /// Выбранная нейросетевая модель.
        /// </summary>
        public INeuralNetwork NeuralNetwork
        {
            get => _neuralNetworkBuilder.NeuralNetwork;

            set
            {
                _neuralNetworkBuilder.NeuralNetwork = value;
            }
        }

        /// <summary>
        /// Поддерживаемые типы искусственных нейронов.
        /// </summary>
        public List<INeuronBuilder> SupportedNeuronBuilders { get; private set; }

        /// <summary>
        /// Выбранная модель искусственного нейрона.
        /// </summary>
        public INeuronBuilder NeuronBuilder
        {
            get => _neuralNetworkBuilder.NeuronBuilder;

            set
            {
                _neuralNetworkBuilder.NeuronBuilder = value;
            }
        }

        /// <summary>
        /// Поддерживаемые алгоритмы обучения нейросетевых моделей.
        /// </summary>
        public List<ILearningAlgorythm> SupportedLearningAlgorythms { get; private set; }

        /// <summary>
        /// Выбранный алгоритм обучения нейросетевой модели.
        /// </summary>
        public ILearningAlgorythm LearningAlgorythm { get; set; }

        private NeuralNetworkBuilder _neuralNetworkBuilder { get; set; }
        private List<Tuple<IList<double>, IList<double>>> _trainingSet { get; set; }
        private List<Tuple<IList<double>, IList<double>>> _validationSet { get; set; }

        public NeuralNetworkModelGenerator()
        {
            _neuralNetworkBuilder = new NeuralNetworkBuilder(null, null);

            SupportedNeuralNetworks = new List<INeuralNetwork>
            {
                new MultilayerPerceptron()
            };

            SupportedNeuronBuilders = new List<INeuronBuilder>
            {
                new SigmoidNeuronBuilder()
            };

            SupportedLearningAlgorythms = new List<ILearningAlgorythm>
            {
                new BackpropagationLearningAlgorythm()
            };
        }

        /// <summary>
        /// Добавление нового слоя в модель в качестве последнего слоя.
        /// </summary>
        /// <param name="neuronAmout">Количество нейронов в слое.</param>
        /// <param name="synapticConnectionAmount">Количество синаптических связей у каждого из нейронов.</param>
        public void AddLayer(int neuronAmout, int synapticConnectionAmount)
        {
            _neuralNetworkBuilder.AddLayer(neuronAmout, synapticConnectionAmount);
        }

        /// <summary>
        /// Генерация нейросетевой модели. Результат помещается в поле NeuralNetwork.
        /// </summary>
        public void GenerateneuralNetworkModel()
        {
            NeuralNetwork = _neuralNetworkBuilder.Build();
        }

        /// <summary>
        /// Загрузка обучающей выборки из файла.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void LoadTrainingSetFromFile(string filePath)
        {
            _trainingSet = new List<Tuple<IList<double>, IList<double>>>();
            using (var streamReader = new StreamReader(filePath))
            {
                var line = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    var splitedLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var inputSample = splitedLine[0].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(feature => double.Parse(feature)).ToList();
                    var desiredResult = splitedLine[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(feature => double.Parse(feature)).ToList();

                    _trainingSet.Add(new Tuple<IList<double>, IList<double>>(inputSample, desiredResult));
                }
            }
        }

        /// <summary>
        /// Загрузка проверочной выборки из файла.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void LoadValidationSetFromFile(string filePath)
        {
            _validationSet = new List<Tuple<IList<double>, IList<double>>>();
            using (var streamReader = new StreamReader(filePath))
            {
                var line = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    var splitedLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var inputSample = splitedLine[0].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(feature => double.Parse(feature)).ToList();
                    var desiredResult = splitedLine[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(feature => double.Parse(feature)).ToList();

                    _validationSet.Add(new Tuple<IList<double>, IList<double>>(inputSample, desiredResult));
                }
            }
        }

        /// <summary>
        /// Обучение нейросетевой модели с выбранными параметрами на основе загруженных обучающей и проверочной выборок.
        /// </summary>
        public void Learn()
        {
            LearningAlgorythm.Learn(NeuralNetwork, _trainingSet, _validationSet);
        }

        /// <summary>
        /// Сохранение обученной нейросетевой модели в файл.
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public void SaveNeuralNetworkModelToFile(string filePath)
        {
            var binaryFormatter = new BinaryFormatter();

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                binaryFormatter.Serialize(fileStream, _neuralNetworkBuilder.NeuralNetwork);
            }
        }
    }
}