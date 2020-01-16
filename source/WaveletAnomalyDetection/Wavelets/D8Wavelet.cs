using System;

namespace WaveletAnomalyDetection.Wavelets
{
    /// <summary>
    /// Вейвлет Добеши 8
    /// </summary>
    [Serializable]
    public class D8Wavelet : IWavelet
    {
        public override string ToString() => "Вейвлет Добеши 8";

        public double[] Coefficients { get; } = new double[] { -0.01498699, -0.0465036, 0.0436163, 0.26450717, -0.03957503, -0.89220014, 1.01094572, -0.32580343 };
        public double[] ScalingCoefficients { get; } = new double[] { 0.32580343, 1.01094572, 0.89220014, -0.03957503, -0.26450717, 0.0436163, 0.0465036, -0.01498699 };
    }
}