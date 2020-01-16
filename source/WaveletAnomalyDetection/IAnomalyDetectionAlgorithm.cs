using System.Collections.Generic;

namespace WaveletAnomalyDetection
{
    /// <summary>
    /// Интерфейс алгоритма поиска аномалий на основе статистических критериев.
    /// </summary>
    public interface IAnomalyDetectionAlgorithm
    {
        /// <summary>
        /// Название алгоритма. Используется при выводе результатов поиска аномалий.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Проверка на наличие аномалий для нормальных и исследуемых коэффициентов.
        /// </summary>
        /// <param name="firstApproximationCoefficients">Аппроксимирующие коэффициенты (средние значения) для нормальной модели.</param>
        /// <param name="firstDetailingCoefficients">Детализирующие коэффициенты (дисперсия) для нормальной модели.</param>
        /// <param name="secondApproximationCoefficients">Аппроксимирующие коэффициенты (средние значения) для исследуемых данных.</param>
        /// <param name="secondDetailingCoefficients">Детализирующие коэффициенты (дисперсия) для исследуемых данных.</param>
        /// <param name="sensitivity">Чувствительность критерия. Число в полуинтервале (0, double.MaxValue]. Рекомендуемое значение: 1.</param>
        /// <returns>Результат применения алгоритма.</returns>
        AnomalyDetectionResult CheckOnAnomaly(IList<double> firstApproximationCoefficients, IList<double> firstDetailingCoefficients, IList<double> secondApproximationCoefficients, IList<double> secondDetailingCoefficients, double sensitivity);
    }
}
