using System;
using System.Collections.Generic;

using WaveletAnomalyDetection.Distributions;

namespace WaveletAnomalyDetection.AnomalyDetectionAlgorithms
{
    /// <summary>
    /// Статистический алгоритм обнаружения аномалий на основе критерия Кохрана-Кокса для средних значений.
    /// </summary>
    public class CochraneCoxCriterionForApproximations : IAnomalyDetectionAlgorithm
    {
        public string Name => "Критерий Кохрана-Кокса (среднее значение)";

        private const double _anomalySignificanceLevel = 0.001;
        private const double _probabilityOfAnomalySignificanceLevel = 0.05;

        /// <summary>
        /// Проверка на наличие аномалий для нормальных и исследуемых коэффициентов.
        /// </summary>
        /// <param name="firstApproximationCoefficients">Аппроксимирующие коэффициенты (средние значения) для нормальной модели.</param>
        /// <param name="firstDetailingCoefficients">Не используются. Может быть передан null.</param>
        /// <param name="secondApproximationCoefficients">Аппроксимирующие коэффициенты (средние значения) для исследуемых данных.</param>
        /// <param name="secondDetailingCoefficients">Не используются. Может быть передан null.</param>
        /// <param name="sensitivity">Чувствительность критерия. Число в полуинтервале (0, double.MaxValue]. Рекомендуемое значение: 1.</param>
        /// <returns>Результат применения алгоритма.</returns>
        public AnomalyDetectionResult CheckOnAnomaly(IList<double> firstApproximationCoefficients, IList<double> firstDetailingCoefficients,
            IList<double> secondApproximationCoefficients, IList<double> secondDetailingCoefficients, double sensitivity)
        {
            var firstApproximationApproximatedCoefficient = 0.0;
            foreach (var coefficient in firstApproximationCoefficients)
            {
                firstApproximationApproximatedCoefficient += coefficient;
            }
            firstApproximationApproximatedCoefficient /= firstApproximationCoefficients.Count;

            var secondApproximationApproximatedCoefficient = 0.0;
            foreach (var coefficient in secondApproximationCoefficients)
            {
                secondApproximationApproximatedCoefficient += coefficient;
            }
            secondApproximationApproximatedCoefficient /= secondApproximationCoefficients.Count;

            var firstDispersion = 0.0;
            foreach (var coefficient in firstApproximationCoefficients)
            {
                firstDispersion += Math.Pow(coefficient - firstApproximationApproximatedCoefficient, 2);
            }
            firstDispersion /= firstApproximationCoefficients.Count - 1;

            var secondDispersion = 0.0;
            foreach (var coefficient in secondApproximationCoefficients)
            {
                secondDispersion += Math.Pow(coefficient - secondApproximationApproximatedCoefficient, 2);
            }
            secondDispersion /= secondApproximationCoefficients.Count - 1;

            var firstWeightedDispersion = firstDispersion / firstApproximationCoefficients.Count;
            var secondWeightedDispersion = secondDispersion / secondApproximationCoefficients.Count;
            var summaryDispersion = firstWeightedDispersion + secondWeightedDispersion;

            var statisticsResult = (Math.Abs(secondApproximationApproximatedCoefficient - firstApproximationApproximatedCoefficient)) / Math.Sqrt(summaryDispersion);

            statisticsResult *= sensitivity;

            var probabilityOfAnomalyLimit = (firstWeightedDispersion * StudentDistributionTable.GetCriticalValue(firstApproximationCoefficients.Count - 1, _probabilityOfAnomalySignificanceLevel)
                + secondWeightedDispersion * StudentDistributionTable.GetCriticalValue(secondApproximationCoefficients.Count - 1, _probabilityOfAnomalySignificanceLevel))
                / (firstWeightedDispersion + secondWeightedDispersion);

            var anomalyLimit = (firstWeightedDispersion * StudentDistributionTable.GetCriticalValue(firstApproximationCoefficients.Count - 1, _anomalySignificanceLevel)
                + secondWeightedDispersion * StudentDistributionTable.GetCriticalValue(secondApproximationCoefficients.Count - 1, _anomalySignificanceLevel))
                / (firstWeightedDispersion + secondWeightedDispersion);

            var result = new AnomalyDetectionResult() { Source = Name, Type = AnomalyDetectionResultType.Normal, StatisticsValue = statisticsResult, StatisticsLimit = probabilityOfAnomalyLimit };

            if (statisticsResult > probabilityOfAnomalyLimit)
            {
                result.Type = AnomalyDetectionResultType.HighProbabilityOfAnomaly;
                result.StatisticsLimit = probabilityOfAnomalyLimit;
                result.Message = "Обнаружена высокая вероятность наличия долговременной низкочастотной аномалии.";
            }

            if (statisticsResult > anomalyLimit)
            {
                result.Type = AnomalyDetectionResultType.Anomaly;
                result.StatisticsLimit = anomalyLimit;
                result.Message = "Обнаружена долговременная низкочастотная аномалия.";
            }

            return result;
        }

        public override string ToString() => Name;
    }
}
