using System.Collections.Generic;

using WaveletAnomalyDetection.StatisticsModels;

namespace WaveletAnomalyDetection
{
    /// <summary>
    /// WaveletTransformator реализует генерацию статистических моделей на основе быстрого дискретного вейвлет-преобразования.
    /// </summary>
    public static class WaveletTransformator
    {
        /// <summary>
        /// Возращает статистическую модель, содержащую коэффициенты аппроксимации и дисперсии на основе выбранного вейвлета для списка данных.
        /// </summary>
        /// <param name="data">Список данных для статистического анализа в виде вектора над полем рациональных чисел.</param>
        /// <param name="wavelet">Вейвлет, на основе которого вычисляются коэффициенты аппроксимации и дисперсии.</param>
        /// <returns>Cтатистическая модель для выбранных набора данных и вейвлета.</returns>
        public static IWaveletTransformationStatisticsModel GenerateStatisticsModel(IList<double> data, IWavelet wavelet)
        {
            var waveletStatistics = new WaveletStatistics
            {
                Wavelet = wavelet,
                ApproximationCoefficients = data,
                DetailingCoefficients = data
            };

            var statisticsModel = new WaveletTransformationStatisticsModel();
            statisticsModel.WaveletStatistics.Add(waveletStatistics);

            while (true)
            {
                waveletStatistics = Decompose(waveletStatistics);
                if (waveletStatistics.ApproximationCoefficients.Count == 0)
                {
                    break;
                }

                statisticsModel.WaveletStatistics.Add(waveletStatistics);
            }

            return statisticsModel;
        }

        /// <summary>
        /// Возвращает результат разложения посредством вычисления последующих коэффициентов аппроксимации и дисперсии на основе предыдущих. При каждом разложении количество коэффициентов сокращается вдвое.
        /// </summary>
        private static WaveletStatistics Decompose(WaveletStatistics waveletStatistics)
        {
            var nextLevelApproximationCoefficients = Filter(waveletStatistics.ApproximationCoefficients, waveletStatistics.Wavelet.ScalingCoefficients);
            var nextLevelDetailingCoefficients = Filter(waveletStatistics.ApproximationCoefficients, waveletStatistics.Wavelet.Coefficients);

            return new WaveletStatistics
            {
                Wavelet = waveletStatistics.Wavelet,
                ApproximationCoefficients = nextLevelApproximationCoefficients,
                DetailingCoefficients = nextLevelDetailingCoefficients
            };
        }

        /// <summary>
        /// Применение вейвлет-преобразования (фильтрации)
        /// </summary>
        private static List<double> Filter(IList<double> coefficientsToFilter, double[] filterCoefficients)
        {
            var filteredCoefficients = new List<double>();

            for (var i = 0; i + filterCoefficients.Length - 1 < coefficientsToFilter.Count; i += 2)
            {
                var newCoefficient = 0.0;
                for (int j = 0; j < filterCoefficients.Length; j++)
                {
                    newCoefficient += filterCoefficients[j] * coefficientsToFilter[i + j];
                }
                filteredCoefficients.Add(newCoefficient);
            }

            return filteredCoefficients;
        }
    }
}