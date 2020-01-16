using System;
using System.Collections.Generic;

using WaveletAnomalyDetection.Distributions;

namespace WaveletAnomalyDetection.AnomalyDetectionAlgorithms
{
    /// <summary>
    /// Статистический алгоритм обнаружения аномалий на основе критерия Фишера для дисперсии.
    /// </summary>
    public class FischerCriterionForDispersions : IAnomalyDetectionAlgorithm
    {
        public string Name => "Критерий Фишера (дисперсия)";

        private const double _anomalySignificanceLevel = 0.001;
        private const double _probabilityOfAnomalySignificanceLevel = 0.05;

        /// <summary>
        /// Проверка на наличие аномалий для нормальных и исследуемых коэффициентов.
        /// </summary>
        /// <param name="firstApproximationCoefficients">Не используются. Может быть передан null.</param>
        /// <param name="firstDetailingCoefficients">Детализирующие коэффициенты (дисперсия) для нормальной модели.</param>
        /// <param name="secondApproximationCoefficients">Не используются. Может быть передан null.</param>
        /// <param name="secondDetailingCoefficients">Детализирующие коэффициенты (дисперсия) для исследуемых данных.</param>
        /// <param name="sensitivity">Чувствительность критерия. Число в полуинтервале (0, double.MaxValue]. Рекомендуемое значение: 1.</param>
        /// <returns>Результат применения алгоритма.</returns>
        public AnomalyDetectionResult CheckOnAnomaly(IList<double> firstApproximationCoefficients, IList<double> firstDetailingCoefficients,
            IList<double> secondApproximationCoefficients, IList<double> secondDetailingCoefficients, double sensitivity)
        {
            var firstDetailingApproximatedCoefficient = 0.0;
            foreach (var coefficient in firstDetailingCoefficients)
            {
                firstDetailingApproximatedCoefficient += coefficient;
            }
            firstDetailingApproximatedCoefficient /= firstDetailingCoefficients.Count;

            var secondDetailingApproximatedCoefficient = 0.0;
            foreach (var coefficient in secondDetailingCoefficients)
            {
                secondDetailingApproximatedCoefficient += coefficient;
            }
            secondDetailingApproximatedCoefficient /= secondDetailingCoefficients.Count;

            var firstDispersion = 0.0;
            foreach (var coefficient in firstDetailingCoefficients)
            {
                firstDispersion += Math.Pow(coefficient - firstDetailingApproximatedCoefficient, 2);
            }
            firstDispersion /= firstDetailingCoefficients.Count - 1;

            var secondDispersion = 0.0;
            foreach (var coefficient in secondDetailingCoefficients)
            {
                secondDispersion += Math.Pow(coefficient - secondDetailingApproximatedCoefficient, 2);
            }
            secondDispersion /= secondDetailingCoefficients.Count - 1;

            var statisticsResult = secondDispersion / firstDispersion;

            statisticsResult *= sensitivity;

            var probabilityOfAnomalyLimit = FischerDistributionTable.GetCriticalValue(firstDetailingCoefficients.Count - 1, secondDetailingCoefficients.Count - 1, _probabilityOfAnomalySignificanceLevel);
            var anomalyLimit = FischerDistributionTable.GetCriticalValue(firstDetailingCoefficients.Count - 1, secondDetailingCoefficients.Count - 1, _anomalySignificanceLevel);

            var result = new AnomalyDetectionResult() { Source = Name, Type = AnomalyDetectionResultType.Normal, StatisticsValue = statisticsResult, StatisticsLimit = probabilityOfAnomalyLimit };

            if (statisticsResult > probabilityOfAnomalyLimit)
            {
                result.Type = AnomalyDetectionResultType.HighProbabilityOfAnomaly;
                result.Message = "Обнаружена высокая вероятность наличия кратковременной высокочастотной аномалии.";
            }

            if (statisticsResult > anomalyLimit)
            {
                result.Type = AnomalyDetectionResultType.Anomaly;
                result.StatisticsLimit = anomalyLimit;
                result.Message = "Обнаружена кратковременная высокочастотная аномалия.";
            }

            return result;
        }

        public override string ToString() => "Критерий Фишера (дисперсии)";
    }
}
