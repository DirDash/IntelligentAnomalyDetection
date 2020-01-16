using System.Collections.Generic;

namespace NeuralNetworkTools
{
    /// <summary>
    /// Модель искусственного нейрона
    /// </summary>
    public interface INeuron
    {
        /// <summary>
        /// Коллекция весов синаптических связей.
        /// </summary>
        IList<double> SynapticWeights { get; }

        /// <summary>
        /// Порог, увеличивающий выходное значение функции активации.
        /// </summary>
        double Threshold { get; set; }

        /// <summary>
        /// Коэффициент скорости обучения. Число в полуинтервале (0; double.MaxValue]. Рекомендуемое значение: 1.
        /// </summary>
        double LearningSpeed { get; set; }

        /// <param name="inputSample">Объект в виде числового вектора.</param>
        /// <returns>Значение потенциала активации.</returns>
        double CalculateActivationPotential(IList<double> inputSample);

        /// <param name="inputSample">Объект в виде числового вектора.</param>
        /// <returns>Выходное значение.</returns>
        double CalculateOutputValue(IList<double> inputSample);

        /// <param name="activationPotential">Значение потенциала активации.</param>
        /// <returns>Выходное значение.</returns>
        double CalculateOutputValue(double activationPotential);

        /// <summary>
        /// Активационная функция.
        /// </summary>
        double CalculateActivationFunction(double x);

        /// <summary>
        /// Производная активационной функции.
        /// </summary>
        double CalculateActivationFunctionDerivative(double x);
    }
}