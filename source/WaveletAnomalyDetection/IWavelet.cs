namespace WaveletAnomalyDetection
{
    /// <summary>
    /// Интерфейс вейвлета, заданный своими коэффициентами и коэффициентами своей масштабирующей функции.
    /// </summary>
    public interface IWavelet
    {
        /// <summary>
        /// Собственные коэффициенты вейвлета.
        /// </summary>
        double[] Coefficients { get; }

        /// <summary>
        /// Коэффициенты масштабирующей функции вейвлета.
        /// </summary>
        double[] ScalingCoefficients { get; }
    }
}