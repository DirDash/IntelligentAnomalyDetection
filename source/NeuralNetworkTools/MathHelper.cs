using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetworkTools
{
    /// <summary>
    /// Набор вспомогательных утилит.
    /// </summary>
    internal static class MathHelper
    {
        private static Random _random = new Random();

        /// <summary>
        /// Получения случайного числа в заданном диапазоне.
        /// </summary>
        /// <param name="mathematicalExpectation">Математическое ожидание.</param>
        /// <param name="dispersion">Дисперсия.</param>
        /// <returns>Рациональное число в диапазоне [mathematicalExpectation - dispersion; mathematicalExpectation + dispersion].</returns>
        public static double GetRandomValue(double mathematicalExpectation, double dispersion)
        {
            return mathematicalExpectation + dispersion * (2 * _random.NextDouble() - 1);
        }

        /// <summary>
        /// Перемешивает элементы коллекции случайным образом.
        /// </summary>
        /// <param name="array">Перемешиваемая коллекция.</param>
        /// <returns>Перемешанная коллекция.</returns>
        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> array)
        {
            return array.OrderBy(element => _random.Next());
        }

        /// <summary>
        /// Нормализует вектор.
        /// </summary>
        public static IList<double> Normalize(IList<double> list)
        {
            var medianAndNormalizer = CalculateMedianAndNormalizer(list);

            var median = medianAndNormalizer.Item1;
            var normalizer = medianAndNormalizer.Item2;

            var normalizedList = new List<double>();
            for (var i = 0; i < list.Count(); i++)
            {
                var normalizedValue = normalizer != 0 ? (list[i] - median) / normalizer : 0;

                normalizedList.Add(normalizedValue);
            }

            return normalizedList;
        }

        /// <summary>
        /// Вычисление медианы и нормализатора для нормализации вектора.
        /// </summary>
        public static Tuple<double, double> CalculateMedianAndNormalizer(IList<double> list)
        {
            var min = list.First();
            var max = list.First();
            foreach (var element in list)
            {
                if (element < min)
                {
                    min = element;
                }

                if (element > max)
                {
                    max = element;
                }
            }

            var median = (min + max) / 2;
            var normalizer = max - median;

            return new Tuple<double, double>(median, normalizer);
        }
    }
}