using System;

using NeuralNetworkTools.Neurons;

namespace NeuralNetworkTools.NeuronBuilders
{
    /// <summary>
    /// Построитель моделей искусственного нейрона с сигмоидальной функцией активацией (гиперболический тангенс).
    /// </summary>
    public class SigmoidNeuronBuilder : INeuronBuilder
    {
        public override string ToString() => "Генератор нейронов с сигмоидальной функцией активации";

        /// <summary>
        /// Строит модель искусственного нейрона с заданными параметрами.
        /// </summary>
        /// <param name="synapticConnectionAmount">Количество синаптических связей (размерность вектора входных данных)ю</param>
        /// <param name="threshold">Порог, увеличивающий выходное значение функции активации.</param>
        /// <param name="learningSpeed">Коэффициент скорости обучения. Число в полуинтервале (0; double.MaxValue]. Рекомендуемое значение: 1.</param>
        /// <returns>Модель искусственного нейрона с сигмоидальной функцией активацией (гиперболический тангенс)</returns>
        public INeuron Build(int synapticConnectionAmount, double threshold, double learningSpeed)
        {
            var neuron = new SigmoidNeuron(synapticConnectionAmount, threshold, learningSpeed);

            var dispersion = 1.0 / Math.Sqrt(synapticConnectionAmount);

            for (var i = 0; i < neuron.SynapticWeights.Count; i++)
            {
                neuron.SynapticWeights[i] = MathHelper.GetRandomValue(0, dispersion);
            }

            return neuron;
        }
    }
}