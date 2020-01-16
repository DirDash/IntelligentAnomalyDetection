using System;

namespace WaveletAnomalyDetection.Wavelets
{
    /// <summary>
    /// Вейвлет Добеши 6
    /// </summary>
    [Serializable]
    public class D6Wavelet : IWavelet
    {
        public override string ToString() => "Вейвлет Добеши 6";

        public double[] Coefficients { get; } = new double[] { 0.0498175, 0.12083221, -0.19093442, -0.650365, 1.14111692, -0.47046721 };
        public double[] ScalingCoefficients { get; } = new double[] { 0.47046721, 1.14111692, 0.650365, -0.19093442, -0.12083221, 0.0498175 };
    }
}