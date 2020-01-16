using System.Collections.Generic;

namespace NeuralNetworkTools
{
    /// <summary>
    /// Построитель моделей искусственных нейронных сетей.
    /// </summary>
    public class NeuralNetworkBuilder
    {
        /// <summary>
        /// Построенная модель искусственной нейронной сети.
        /// </summary>
        public INeuralNetwork NeuralNetwork { get; set; }

        /// <summary>
        /// Построитель моделей икусственных нейронов.
        /// </summary>
        public INeuronBuilder NeuronBuilder { get; set; }

        /// <param name="neuralNetwork">Построенная модель искусственной нейронной сети.</param>
        /// <param name="neuronBuilder">Построитель моделей икусственных нейронов.</param>
        public NeuralNetworkBuilder(INeuralNetwork neuralNetwork, INeuronBuilder neuronBuilder)
        {
            NeuralNetwork = neuralNetwork;
            NeuronBuilder = neuronBuilder;
        }

        /// <summary>
        /// Добавление слоя искусственных нейроннов в качестве последнего слоя.
        /// </summary>
        /// <param name="neuronAmount">Количество искусственных нейронов в создаваемом слое.</param>
        /// <param name="synapticConnectionAmount">Количество синаптических весов у каждого из искусственных нейронов создаваемого слоя.</param>
        public void AddLayer(int neuronAmount, int synapticConnectionAmount)
        {
            var newLayer = new List<INeuron>();

            for (var i = 0; i < neuronAmount; i++)
            {
                newLayer.Add(NeuronBuilder.Build(synapticConnectionAmount, 0, 1));
            }

            NeuralNetwork.Layers.Add(newLayer);
        }

        /// <summary>
        /// Создает модель искусственной сети с выбранной структурой.
        /// </summary>
        /// <returns>Возвращает созданную модель искусственной нейронной сети.</returns>
        public INeuralNetwork Build()
        {
            return NeuralNetwork;
        }        
    }
}