using System;
using System.Collections.Generic;

namespace NeuralNetworkTools
{
    /// <summary>
    /// Алгоритм обучения искусственной нейронной сети.
    /// </summary>
    public interface ILearningAlgorythm
    {
        /// <summary>
        /// Обучает переданную модель искусственной нейронной сети.
        /// </summary>
        /// <param name="neuralNerwork">Модель обучаемой искусственной нейронной сети.</param>
        /// <param name="trainingSet">Тренировочная выборка вида (входной вектор, желаемый выходной вектор).</param>
        /// <param name="validaitonSet">Проверочная выборка вида (входной вектор, желаемый выходной вектор).</param>
        /// <returns>Обученная модель искусственной нейронной сети.</returns>
        INeuralNetwork Learn(INeuralNetwork neuralNerwork, IEnumerable<Tuple<IList<double>, IList<double>>> trainingSet, IEnumerable<Tuple<IList<double>, IList<double>>> validaitonSet);
    }
}