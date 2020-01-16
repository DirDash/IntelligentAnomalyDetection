namespace WaveletAnomalyDetection
{
    /// <summary>
    /// Результат применения детектора аномалий для выборки данных.
    /// </summary>
    public class AnomalyDetectionResult
    {
        /// <summary>
        /// Тип результата: нормальный, аномальный с высокой вероятностью, аномальный.
        /// </summary>
        public AnomalyDetectionResultType Type { get; set; }

        /// <summary>
        /// Результат применения статистического критерия.
        /// </summary>
        public double StatisticsValue { get; set; }
        /// <summary>
        /// Предел статистического критерия.
        /// </summary>
        public double StatisticsLimit { get; set; }

        /// <summary>
        /// Название применённого статистического критерия.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Сообщение применённого статистического критерия.
        /// </summary>
        public string Message { get; set; }
    }
}