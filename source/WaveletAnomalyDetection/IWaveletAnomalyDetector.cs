using System.Collections.Generic;

namespace WaveletAnomalyDetection
{
    /// <summary>
    /// Интерфейс детектора аномалий на основе вейвлет-преобразования. 
    /// </summary>
    public interface IWaveletAnomalyDetector
    {
        /// <summary>
        /// Список используемых статистических алгоритмов обнаружения аномалий.
        /// </summary>
        List<IAnomalyDetectionAlgorithm> AnomalyDetectionAlgorithms { get; set; }

        /// <summary>
        /// Коэффициент чувствительности детектора аномалий. Число в полуинтервале (0, double.MaxValue]. Рекомендуемое значение: 1.
        /// </summary>
        double Sensitivity { get; set; }

        /// <summary>
        /// Осуществляет применение статистических алгоритмов обнаружения аномалий к переданным данным.
        /// </summary>
        /// <param name="data">Исследумеые данные, представленные в виде числовых векторов.</param>
        /// <param name="normalModel">Стастистическая модель нормальных данных.</param>
        /// <returns>Коллекцию результатов обнаружения аномалий, по одному для каждого из выбранных алгоритмов обнаружения.</returns>
        IEnumerable<AnomalyDetectionResult> CheckOnAnomaly(IList<double> data, IWaveletTransformationStatisticsModel normalModel);
    }
}