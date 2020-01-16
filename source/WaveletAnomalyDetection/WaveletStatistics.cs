using System;
using System.Collections.Generic;

namespace WaveletAnomalyDetection
{
    /// <summary>
    /// Статистические данные, содержащие последовательности коэффициентов аппроксимации и детализации (дисперсии) на основе выбранного вейвлета.
    /// </summary>
    [Serializable]
    public struct WaveletStatistics
    {
        public IWavelet Wavelet { get; set; }

        public IList<double> ApproximationCoefficients { get; set; }
        public IList<double> DetailingCoefficients { get; set; }
    }
}