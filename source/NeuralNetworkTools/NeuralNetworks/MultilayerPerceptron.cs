using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetworkTools.NeuralNetworks
{
    /// <summary>
    /// Модель полносвязного многослойного персептрона.
    /// </summary>
    [Serializable]
    public class MultilayerPerceptron : INeuralNetwork
    {
        public override string ToString() => "Многослойный персептрон";

        // <summary>
        /// Коллекция слоёв искусственных нейронов.
        /// </summary>
        public IList<IList<INeuron>> Layers { get; private set; }

        /// <summary>
        /// Коэффициенты, использующиеся для нормализации входных значений.
        /// </summary>
        public IList<Tuple<double, double>> NormalizationValues { get; set; }

        /// <summary>
        /// Размерность вектора входных данных.
        /// </summary>
        public int InputValueDimension
        {
            get => Layers.First().First().SynapticWeights.Count();
        }

        /// <summary>
        /// Размерность вектора выходных данных.
        /// </summary>
        public int OutputValueDimension
        {
            get => Layers.Last().Count();
        }

        private double _learningSpeed = 1.0;
        /// <summary>
        /// Коэффициент скорости обучения. Число в полуинтервале (0; double.MaxValue]. Рекомендуемое значение: 1.
        /// </summary>
        public double LearningSpeed
        {
            get => _learningSpeed;

            set
            {
                _learningSpeed = value;

                if (Layers.Count > 0)
                {
                    foreach (var layer in Layers)
                    {
                        foreach (var neuron in layer)
                        {
                            neuron.LearningSpeed = value;
                        }
                    }
                }
            }
        }

        public MultilayerPerceptron()
        {
            Layers = new List<IList<INeuron>>();
            NormalizationValues = new List<Tuple<double, double>>();
        }

        /// <summary>
        /// Вычисление на основе искусственной сети.
        /// </summary>
        /// <param name="sample">Входной объект в виде числового вектора.</param>
        /// <returns>Выходной объект в виде числового вектора.</returns>
        public IList<double> Calculate(IList<double> inputSample)
        {
            var currentValues = NormalizeSample(inputSample);

            foreach (var layer in Layers)
            {
                var newValues = new List<double>();

                foreach (var neuron in layer)
                {
                    newValues.Add(neuron.CalculateOutputValue(currentValues));
                }

                currentValues = newValues;
            }

            return currentValues;
        }

        /// <summary>
        /// Нормализует значения числового вектора, приводя их к интервалу [-1; 1].
        /// </summary>
        /// <param name="sample">Числовой вектор.</param>
        /// <returns>Нормализованный числовой вектор.</returns>
        public List<double> NormalizeSample(IList<double> sample)
        {
            var normalizedSample = new List<double>();

            for (var i = 0; i < sample.Count(); i++)
            {
                var featureMedian = NormalizationValues[i].Item1;
                var featureNormalizer = NormalizationValues[i].Item2;

                var normalizedFeature = featureNormalizer != 0 ? (sample[i] - featureMedian) / featureNormalizer : 0;
                normalizedSample.Add(normalizedFeature);
            }

            return normalizedSample;
        }

        public object Clone()
        {
            var layersCopy = new List<IList<INeuron>>();
            foreach (var layer in Layers)
            {
                var layerCopy = new List<INeuron>();
                foreach (var neuron in layer)
                {
                    layerCopy.Add(neuron);
                }

                layersCopy.Add(layerCopy);
            }

            var normalizationValuesCopy = new List<Tuple<double, double>>();
            foreach (var value in NormalizationValues)
            {
                normalizationValuesCopy.Add(new Tuple<double, double>(value.Item1, value.Item2));
            }

            return new MultilayerPerceptron()
            {
                Layers = layersCopy,
                NormalizationValues = normalizationValuesCopy
            };
        }
    }
}