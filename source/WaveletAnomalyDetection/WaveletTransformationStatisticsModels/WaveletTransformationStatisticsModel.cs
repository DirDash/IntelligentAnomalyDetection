using System;
using System.Collections.Generic;
using System.Linq;

namespace WaveletAnomalyDetection.StatisticsModels
{
    /// <summary>
    /// Статистическая модель на основе вейвлет-преобразования.
    /// </summary>
    [Serializable]
    public class WaveletTransformationStatisticsModel : IWaveletTransformationStatisticsModel
    {
        public override string ToString() => "Статистическая модель вейвлет-преобразований";

        /// <summary>
        /// Вейвлет для создания статистичих моделей.
        /// </summary>
        public IWavelet Wavelet
        {
            get
            {
                if (WaveletStatistics.Count() == 0)
                {
                    return null;
                }

                return WaveletStatistics.First().Wavelet;
            }
        }

        /// <summary>
        /// Коллекция статистических моделей, созданных на основе выбранного вейвлета.
        /// </summary>
        public IList<WaveletStatistics> WaveletStatistics { get; set; }

        public WaveletTransformationStatisticsModel()
        {
            WaveletStatistics = new List<WaveletStatistics>();
        }
    }
}