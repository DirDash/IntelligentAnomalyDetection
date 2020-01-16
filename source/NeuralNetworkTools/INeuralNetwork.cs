using System;
using System.Collections.Generic;

namespace NeuralNetworkTools
{
    /// <summary>
    /// Модель искусственной нейронной сети.
    /// </summary>
    public interface INeuralNetwork : ICloneable
    {
        /// <summary>
        /// Коллекция слоёв искусственных нейронов.
        /// </summary>
        IList<IList<INeuron>> Layers { get; }

        /// <summary>
        /// Коэффициенты, использующиеся для нормализации входных значений.
        /// </summary>
        IList<Tuple<double, double>> NormalizationValues { get; set; }

        /// <summary>
        /// Размерность вектора входных данных.
        /// </summary>
        int InputValueDimension { get; }

        /// <summary>
        /// Размерность вектора выходных данных.
        /// </summary>
        int OutputValueDimension { get; }

        /// <summary>
        /// Коэффициент скорости обучения. Число в полуинтервале (0; double.MaxValue]. Рекомендуемое значение: 1.
        /// </summary>
        double LearningSpeed { get; set; }

        /// <summary>
        /// Вычисление на основе искусственной сети.
        /// </summary>
        /// <param name="sample">Входной объект в виде числового вектора.</param>
        /// <returns>Выходной объект в виде числового вектора.</returns>
        IList<double> Calculate(IList<double> sample);
    }
}