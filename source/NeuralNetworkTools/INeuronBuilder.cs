namespace NeuralNetworkTools
{
    /// <summary>
    /// Построитель моделей искусственного нейрона.
    /// </summary>
    public interface INeuronBuilder
    {
        /// <summary>
        /// Строит модель искусственного нейрона с заданными параметрами.
        /// </summary>
        /// <param name="synapticConnectionAmount">Количество синаптических связей (размерность вектора входных данных)ю</param>
        /// <param name="threshold">Порог, увеличивающий выходное значение функции активации.</param>
        /// <param name="learningSpeed">Коэффициент скорости обучения. Число в полуинтервале (0; double.MaxValue]. Рекомендуемое значение: 1.</param>
        /// <returns>Модель искусственного нейрона.</returns>
        INeuron Build(int synapticConnectionAmount, double threshold, double learningSpeed);
    }
}