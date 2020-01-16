using System;

namespace WaveletAnomalyDetection.Wavelets
{
    /// <summary>
    /// Вейвлет Добеши 4
    /// </summary>
    [Serializable]
    public class D4Wavelet : IWavelet
    {
        public override string ToString() => "Вейвлет Добеши 4";

        public double[] Coefficients { get; } = new double[] { -0.1830127, -0.3169873, 1.1830127, -0.6830127 };
        public double[] ScalingCoefficients { get; } = new double[] { 0.6830127, 1.1830127, 0.3169873, -0.1830127 };
    }
}