using System;
using System.Collections.Generic;

using WaveletAnomalyDetection;

namespace AnomalyDetectionApplication
{
    /// <summary>
    /// Статистическая модель для объектов размерности больше единицы.
    /// </summary>
    [Serializable]
    public class MultiFeatureStatisticsModel
    {
        /// <summary>
        /// Одномерные статистические модели.
        /// </summary>
        public List<IWaveletTransformationStatisticsModel> FeatureStatisticsModels { get; set; }

        public MultiFeatureStatisticsModel()
        {
            FeatureStatisticsModels = new List<IWaveletTransformationStatisticsModel>();
        }
    }
}