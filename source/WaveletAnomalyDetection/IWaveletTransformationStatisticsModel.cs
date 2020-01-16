using System.Collections.Generic;

namespace WaveletAnomalyDetection
{
    /// <summary>
    /// Интерфейс для статистической модели на основе вейвлет-преобразования.
    /// </summary>
    public interface IWaveletTransformationStatisticsModel
    {
        /// <summary>
        /// Вейвлет для создания статистичих моделей.
        /// </summary>
        IWavelet Wavelet { get; }

        /// <summary>
        /// Коллекция статистических моделей, созданных на основе выбранного вейвлета.
        /// </summary>
        IList<WaveletStatistics> WaveletStatistics { get; set; }
    }
}