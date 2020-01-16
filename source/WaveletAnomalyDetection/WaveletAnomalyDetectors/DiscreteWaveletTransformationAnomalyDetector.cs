using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaveletAnomalyDetection.WaveletAnomalyDetectors
{
    /// <summary>
    /// Детектор аномалий на основе быстрого дискретного вейвлет-преобразования.
    /// </summary>
    public class DiscreteWaveletTransformationAnomalyDetector : IWaveletAnomalyDetector
    {
        /// <summary>
        /// Список используемых статистических алгоритмов обнаружения аномалий.
        /// </summary>
        public List<IAnomalyDetectionAlgorithm> AnomalyDetectionAlgorithms { get; set; }

        /// <summary>
        /// Коэффициент чувствительности детектора аномалий. Число в полуинтервале (0, double.MaxValue]. Рекомендуемое значение: 1.
        /// </summary>
        public double Sensitivity { get; set; }

        public DiscreteWaveletTransformationAnomalyDetector(List<IAnomalyDetectionAlgorithm> anomalyDetectionAlgorithms)
        {
            AnomalyDetectionAlgorithms = anomalyDetectionAlgorithms;
        }

        /// <summary>
        /// Осуществляет применение статистических алгоритмов обнаружения аномалий к переданным данным.
        /// </summary>
        /// <param name="data">Исследумеые данные, представленные в виде числовых векторов.</param>
        /// <param name="normalModel">Стастистическая модель нормальных данных.</param>
        /// <returns>Коллекцию результатов обнаружения аномалий, по одному для каждого из выбранных алгоритмов обнаружения.</returns>
        public IEnumerable<AnomalyDetectionResult> CheckOnAnomaly(IList<double> data, IWaveletTransformationStatisticsModel normalModel)
        {
            var testingModel = WaveletTransformator.GenerateStatisticsModel(data, normalModel.Wavelet);

            return RunAnomalyDetectionAlgorithms(testingModel, normalModel);
        }

        /// <summary>
        /// Применение статистических алгоритмов обнаружения аномалий.
        /// </summary>
        private IEnumerable<AnomalyDetectionResult> RunAnomalyDetectionAlgorithms(IWaveletTransformationStatisticsModel testingModel, IWaveletTransformationStatisticsModel normalModel)
        {
            var anomalyDetectionAlgorithmResults = new Dictionary<string, AnomalyDetectionResult>();
            foreach (var anomalyDetectionAlgorythm in AnomalyDetectionAlgorithms)
            {
                anomalyDetectionAlgorithmResults.Add(anomalyDetectionAlgorythm.Name, null);
            }

            var anomalyDetectionResults = new List<AnomalyDetectionResult>();

            for (var i = 0; i < normalModel.WaveletStatistics.Count && i < testingModel.WaveletStatistics.Count; i++)
            {
                var normalStatistics = normalModel.WaveletStatistics[i];
                var testingStatistics = testingModel.WaveletStatistics[i];

                Parallel.ForEach(AnomalyDetectionAlgorithms, (IAnomalyDetectionAlgorithm algorythm) =>
                {
                    anomalyDetectionResults.Add(algorythm.CheckOnAnomaly(normalStatistics.ApproximationCoefficients, normalStatistics.DetailingCoefficients, testingStatistics.ApproximationCoefficients, testingStatistics.DetailingCoefficients, Sensitivity));
                });

                foreach (var result in anomalyDetectionResults)
                {
                    // Записываются новые результаты и обновляются старые, если новые содержат информацию об обнаруженной аномалии
                    if (anomalyDetectionAlgorithmResults[result.Source] == null || result.Type > anomalyDetectionAlgorithmResults[result.Source].Type)
                    {
                        anomalyDetectionAlgorithmResults[result.Source] = result;
                    }
                }

                // Если аномалии не обнаружены, но вероятность их наличия очень высока, производится дальнейшее разложение с повторным применением статистических алгоритмов
                if (!anomalyDetectionAlgorithmResults.Values.Any(result => result.Type == AnomalyDetectionResultType.HighProbabilityOfAnomaly))
                {
                    break;
                }
            }

            return anomalyDetectionAlgorithmResults.Values;
        }
    }
}