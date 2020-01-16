using System;

namespace WaveletAnomalyDetection.Wavelets
{
    /// <summary>
    /// Вейвлет Хаара
    /// </summary>
    [Serializable]
    public class HaarWavelet : IWavelet
    {
        public override string ToString() => "Вейвлет Хаара";

        public double[] Coefficients { get; } = new double[] { 1, -1 };
        public double[] ScalingCoefficients { get; } = new double[] { 1, 1 };
    }
}