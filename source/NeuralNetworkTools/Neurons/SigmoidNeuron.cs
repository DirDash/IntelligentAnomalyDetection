using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetworkTools.Neurons
{
    /// <summary>
    /// Модель искусственного нейрона с сигмоидальной функцией активацией (гиперболический тангенс), принимающей значения в интервале [-1; 1].
    /// </summary>
    [Serializable]
    public class SigmoidNeuron : INeuron
    {
        public override string ToString() => "Нейронов с сигмоидальной функцией активации";

        /// <summary>
        /// Коллекция весов синаптических связей.
        /// </summary>
        public IList<double> SynapticWeights { get; private set; }

        /// <summary>
        /// Порог, увеличивающий выходное значение функции активации.
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// Коэффициент скорости обучения. Число в полуинтервале (0; double.MaxValue]. Рекомендуемое значение: 1.
        /// </summary>
        public double LearningSpeed { get; set; }

        /// <summary>
        /// Коэффициент сжатия потенциала активации.
        /// </summary>
        private double _compressionRatio = 2.0/3.0;

        public SigmoidNeuron(int synapticConnectionAmount, double threshold, double learningSpeed, double compressionRatio = 2.0 / 3.0)
        {
            SynapticWeights = new double[synapticConnectionAmount];
            Threshold = threshold;
            LearningSpeed = learningSpeed;
            _compressionRatio = compressionRatio;
        }

        /// <param name="inputSample">Объект в виде числового вектора.</param>
        /// <returns>Значение потенциала активации.</returns>
        public double CalculateActivationPotential(IList<double> inputValues)
        {
            ValidateInput(inputValues);

            var weightedSum = 0.0;
            for (var i = 0; i < inputValues.Count(); i++)
            {
                weightedSum += SynapticWeights[i] * inputValues[i];
            }

            return weightedSum + Threshold;
        }

        /// <param name="inputSample">Объект в виде числового вектора.</param>
        /// <returns>Выходное значение.</returns>
        public double CalculateOutputValue(IList<double> inputValues)
        {
            return CalculateActivationFunction(CalculateActivationPotential(inputValues));
        }

        /// <param name="activationPotential">Значение потенциала активации.</param>
        /// <returns>Выходное значение.</returns>
        public double CalculateOutputValue(double activationPotential)
        {
            return CalculateActivationFunction(activationPotential);
        }

        /// <summary>
        /// Активационная функция.
        /// </summary>
        public double CalculateActivationFunction(double x)
        {
            var res  = Math.Tanh(_compressionRatio * x);
            return res;
        }

        /// <summary>
        /// Производная активационной функции.
        /// </summary>
        public double CalculateActivationFunctionDerivative(double x)
        {
            return _compressionRatio / Math.Pow(Math.Cosh(_compressionRatio * x), 2);
        }

        private void ValidateInput(IList<double> inputSample)
        {
            if (inputSample.Count() != SynapticWeights.Count())
            {
                throw new ArgumentException($"Размерность вектора входных образца не совпадает с количеством синаптических связей. Ожидалось: {SynapticWeights.Count()}, получено: {inputSample.Count()}.");
            }
        }
    }
}