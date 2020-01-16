using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetworkTools.LearningAlgorythms
{
    /// <summary>
    /// Алгоритм обратного распространения ошибки.
    /// </summary>
    public class BackpropagationLearningAlgorythm : ILearningAlgorythm
    {
        public override string ToString() => "Алгоритм обратного распространения ошибки";

        /// <summary>
        /// Коэффициент момента обучения. Определяет степень влияния синаптических, полученных на предыдущих шагах обучения.
        /// </summary>
        public double Momentum { get; set; }

        /// <summary>
        /// Максимальное количество эпох обучения.
        /// </summary>
        public int LearningEpochMaxAmount { get; set; }

        /// <summary>
        /// Минимальное количество эпох обучения.
        /// </summary>
        private int _learningEpochMinAmount = 3;

        /// <param name="momentum">Коэффициент момента обучения. Определяет степень влияния синаптических, полученных на предыдущих шагах обучения.</param>
        /// <param name="learningEpochMaxAmount">Максимальное количество эпох обучения.</param>
        public BackpropagationLearningAlgorythm(double momentum = 0, int learningEpochMaxAmount = 1000)
        {
            Momentum = momentum;
            LearningEpochMaxAmount = learningEpochMaxAmount;
        }

        /// <summary>
        /// Обучает переданную модель искусственной нейронной сети.
        /// </summary>
        /// <param name="neuralNerwork">Модель обучаемой искусственной нейронной сети.</param>
        /// <param name="trainingSet">Тренировочная выборка вида (входной вектор, желаемый выходной вектор).</param>
        /// <param name="validationSet">Проверочная выборка вида (входной вектор, желаемый выходной вектор).</param>
        public INeuralNetwork Learn(INeuralNetwork neuralNetwork, IEnumerable<Tuple<IList<double>, IList<double>>> trainingSet, IEnumerable<Tuple<IList<double>, IList<double>>> validationSet)
        {
            Validate(neuralNetwork, trainingSet, validationSet);

            var normalizedTrainingSet = NormilizeSet(neuralNetwork, trainingSet);
            var normalizedValidationSet = NormilizeSet(neuralNetwork, validationSet);

            var currentNetworkErrorValue = CalculateNetworkErrorValue(neuralNetwork, normalizedValidationSet);
            var minNetworkErrorValue = currentNetworkErrorValue;
            var bestModel = (INeuralNetwork)neuralNetwork.Clone();

            for (var i = 0; i < LearningEpochMaxAmount; i++)
            {
                PerformEpoch(neuralNetwork, normalizedTrainingSet);

                currentNetworkErrorValue = CalculateNetworkErrorValue(neuralNetwork, normalizedValidationSet);

                if (currentNetworkErrorValue < minNetworkErrorValue && i + 1 >= _learningEpochMinAmount)
                {
                    minNetworkErrorValue = currentNetworkErrorValue;
                    bestModel = (INeuralNetwork)neuralNetwork.Clone();
                }
                else
                {
                    break;
                }
            }

            return bestModel;
        }

        /// <summary>
        /// Проверка входных данных: структуры искусственной нейронной сети, тренировочной и проверочной выборок.
        /// </summary>
        private static void Validate(INeuralNetwork neuralNetwork, IEnumerable<Tuple<IList<double>, IList<double>>> trainingSet, IEnumerable<Tuple<IList<double>, IList<double>>> validationSet)
        {
            if (neuralNetwork.Layers.Count() == 0)
            {
                throw new ArgumentException("Исскуственная нейронная сеть не содержит ни одного слоя нейронов.");
            }

            foreach (var trainingSample in trainingSet)
            {
                if (trainingSample.Item1.Count() != neuralNetwork.InputValueDimension)
                {
                    throw new ArgumentException($"Размерность вектора входных данных обучающей выборки не совпадает с количеством синаптических связей нейронов входного слоя. Ожидалось: {neuralNetwork.InputValueDimension}, получено: {trainingSample.Item1.Count()}.");
                }

                if (trainingSample.Item2.Count() != neuralNetwork.OutputValueDimension)
                {
                    throw new ArgumentException($"Размерность вектора ожидаемых выходных данных обучающей выборки не совпадает с количеством нейронов выходного слоя. Ожидалось: {neuralNetwork.OutputValueDimension}, получено: {trainingSample.Item2.Count()}.");
                }
            }

            foreach (var validationSample in validationSet)
            {
                if (validationSample.Item1.Count() != neuralNetwork.InputValueDimension)
                {
                    throw new ArgumentException($"Размерность вектора входных данных проверочной выборки не совпадает с количеством синаптических связей нейронов входного слоя. Ожидалось: {neuralNetwork.InputValueDimension}, получено: {validationSample.Item1.Count()}.");
                }

                if (validationSample.Item2.Count() != neuralNetwork.OutputValueDimension)
                {
                    throw new ArgumentException($"Размерность вектора ожидаемых выходных данных проверочной выборки не совпадает с количеством нейронов выходного слоя. Ожидалось: {neuralNetwork.OutputValueDimension}, получено: {validationSample.Item2.Count()}.");
                }
            }
        }

        /// <summary>
        /// Нормализует векторы выборки в соответствии с выбранной моделью искусственной нейронной сети.
        /// </summary>
        private static List<Tuple<IList<double>, IList<double>>> NormilizeSet(INeuralNetwork neuralNetwork, IEnumerable<Tuple<IList<double>, IList<double>>> set)
        {
            var inputSampleFeatureSets = GetFeatureSets(set.Select(sample => sample.Item1));
            var normalizedInputSamples = NormilizeFeatureSets(inputSampleFeatureSets);

            neuralNetwork.NormalizationValues.Clear();
            foreach (var inputSampleFeatureSet in inputSampleFeatureSets)
            {
                var featureSetMedianAndNormalizer = MathHelper.CalculateMedianAndNormalizer(inputSampleFeatureSet);
                neuralNetwork.NormalizationValues.Add(new Tuple<double, double>(featureSetMedianAndNormalizer.Item1, featureSetMedianAndNormalizer.Item2));
            }

            var desiredOutputFeatureSets = GetFeatureSets(set.Select(sample => sample.Item2));
            var normalizedDesiredOutputSamples = NormilizeFeatureSets(desiredOutputFeatureSets);

            var normalizedSet = new List<Tuple<IList<double>, IList<double>>>();
            for (var i = 0; i < normalizedInputSamples.Count(); i++)
            {
                normalizedSet.Add(new Tuple<IList<double>, IList<double>>(normalizedInputSamples[i], normalizedDesiredOutputSamples[i]));
            }

            return normalizedSet;
        }

        /// <summary>
        /// Возвращает список параметров выборки.
        /// </summary>
        private static List<List<double>> GetFeatureSets(IEnumerable<IList<double>> samples)
        {
            var featureSets = new List<List<double>>();
            foreach (var sampleFeature in samples.First())
            {
                featureSets.Add(new List<double>());
            }

            foreach (var sample in samples)
            {
                for (var i = 0; i < sample.Count(); i++)
                {
                    featureSets[i].Add(sample[i]);
                }
            }

            return featureSets;
        }

        /// <summary>
        /// Нормализует коллекцию параметров.
        /// </summary>
        private static List<List<double>> NormilizeFeatureSets(IEnumerable<IList<double>> featureSets)
        {
            var normalizedFeatureSets = new List<IList<double>>();
            foreach (var featureSet in featureSets)
            {
                normalizedFeatureSets.Add(MathHelper.Normalize(featureSet));
            }

            var normalizedSamples = new List<List<double>>();
            for (var i = 0; i < normalizedFeatureSets.First().Count; i++)
            {
                normalizedSamples.Add(new List<double>());
            }

            foreach (var normalizedFeatureSet in normalizedFeatureSets)
            {
                for (var i = 0; i < normalizedFeatureSet.Count; i++)
                {
                    normalizedSamples[i].Add(normalizedFeatureSet[i]);
                }
            }

            return normalizedSamples;
        }

        /// <summary>
        /// Вычисление значения функции ошибки для выбранной модели искусственной сети.
        /// </summary>
        private static double CalculateNetworkErrorValue(INeuralNetwork neuralNetwork, IEnumerable<Tuple<IList<double>, IList<double>>> validationSet)
        {
            var networkErrorValue = 0.0;

            Parallel.ForEach(validationSet, (Tuple<IList<double>, IList<double>> validationSample) =>
            {
                var inputVector = validationSample.Item1;
                var networkOutput = neuralNetwork.Calculate(inputVector);

                var desiredOutputVector = validationSample.Item2;
                for (var i = 0; i < desiredOutputVector.Count(); i++)
                {
                    networkErrorValue += Math.Abs(desiredOutputVector[i] - networkOutput[i]);
                }
            });

            return networkErrorValue;
        }

        /// <summary>
        /// Выполнение одной эпохи обучения.
        /// </summary>
        private void PerformEpoch(INeuralNetwork neuralNetwork, IEnumerable<Tuple<IList<double>, IList<double>>> trainingSet)
        {
            List<List<List<double>>> previousSynapticWeights = null;

            var shuffledTrainingSet = MathHelper.Shuffle(trainingSet);

            foreach (var trainingSample in shuffledTrainingSet)
            {
                LearnSample(neuralNetwork, previousSynapticWeights, trainingSample.Item1.ToList(), trainingSample.Item2.ToList());

                previousSynapticWeights = new List<List<List<double>>>();
                foreach (var layer in neuralNetwork.Layers)
                {
                    var layerSynapticWeights = new List<List<double>>();
                    foreach (var neuron in layer)
                    {
                        layerSynapticWeights.Add(neuron.SynapticWeights.ToList());
                    }

                    previousSynapticWeights.Add(layerSynapticWeights);
                }
            }
        }

        /// <summary>
        /// Обработка одного объекта обучающей выборки
        /// </summary>
        private void LearnSample(INeuralNetwork neuralNetwork, List<List<List<double>>> previousSynapticWeights, List<double> inputSample, List<double> desiredResult)
        {
            var neuronActivationPotentialsAndOutputs = CalculateNeuronActivationPotentialsAndNetworkOutput(neuralNetwork, inputSample);
            var neuronActivationPotentials = neuronActivationPotentialsAndOutputs.Item1;
            var neuronOutputs = neuronActivationPotentialsAndOutputs.Item2;
            var networkOutput = neuronOutputs.Last();

            var errorVector = CalculateErrorVector(desiredResult, networkOutput);

            var localGradients = CalculateLocalGradients(neuralNetwork, errorVector, neuronActivationPotentials);

            UpdateSynapticWeights(neuralNetwork, previousSynapticWeights, inputSample, neuronOutputs, localGradients);
        }

        /// <summary>
        /// Вычисление пар значений функции активации и выходного значения для всех искусственных нейронов.
        /// </summary>
        private static Tuple<List<List<double>>, List<List<double>>> CalculateNeuronActivationPotentialsAndNetworkOutput(INeuralNetwork neuralNetwork, List<double> inputSample)
        {            
            var neuronActivationPotentials = new List<List<double>>();
            var neuronOutputs = new List<List<double>>();

            var currentOutput = inputSample;
            foreach (var layer in neuralNetwork.Layers)
            {
                var layerNeuronActivationPotentials = new List<double>();
                var layerOutput = new List<double>();

                foreach (var neuron in layer)
                {
                    var neuronActivationPotential = neuron.CalculateActivationPotential(currentOutput);
                    var neuronOutput = neuron.CalculateOutputValue(neuronActivationPotential);

                    layerNeuronActivationPotentials.Add(neuronActivationPotential);
                    layerOutput.Add(neuronOutput);
                }

                neuronActivationPotentials.Add(layerNeuronActivationPotentials);
                neuronOutputs.Add(layerOutput);

                currentOutput = layerOutput;
            }

            return new Tuple<List<List<double>>, List<List<double>>>(neuronActivationPotentials, neuronOutputs);
        }

        /// <summary>
        /// Вычисление значение вектора ошибок.
        /// </summary>
        private static List<double> CalculateErrorVector(IList<double> desiredResult, IList<double> networkOutput)
        {
            var errorVector = new List<double>();

            for (var i = 0; i < desiredResult.Count; i++)
            {
                errorVector.Add(desiredResult[i] - networkOutput[i]);
            }

            return errorVector;
        }

        /// <summary>
        /// Вычисление локального градиента для обратного распространения ошибки.
        /// </summary>
        private static List<List<double>> CalculateLocalGradients(INeuralNetwork neuralNetwork, List<double> errorVector, List<List<double>> neuronActivationPotentials)
        {
            var localGradients = new List<double>[neuralNetwork.Layers.Count];

            for (var i = neuralNetwork.Layers.Count - 1; i >= 0; i--)
            {
                var layer = neuralNetwork.Layers[i];
                var layerLocalGradients = new List<double>();

                for (var j = 0; j < layer.Count; j++)
                {
                    var neuron = layer[j];

                    var localGradient = neuron.CalculateActivationFunctionDerivative(neuronActivationPotentials[i][j]);

                    if (i == neuralNetwork.Layers.Count - 1)
                    {
                        localGradient *= errorVector[j];
                    }
                    else
                    {
                        var nextLayer = neuralNetwork.Layers[i + 1];
                        var nextLayerLocalGradientWeightedSum = 0.0;

                        for (var k = 0; k < nextLayer.Count; k++)
                        {
                            var childNeuron = nextLayer[k];

                            nextLayerLocalGradientWeightedSum += localGradients[i + 1][k] * childNeuron.SynapticWeights[j];
                        }

                        localGradient *= nextLayerLocalGradientWeightedSum;
                    }

                    layerLocalGradients.Add(localGradient);
                }

                localGradients[i] = layerLocalGradients;
            }

            return localGradients.ToList();
        }

        /// <summary>
        /// Обновление синаптических весов обучаемой модели искусственной нейронной сети.
        /// </summary>
        private void UpdateSynapticWeights(INeuralNetwork neuralNetwork, List<List<List<double>>> previousSynapticWeights, List<double> inputSample, List<List<double>> neuronOutputs, List<List<double>> localGradients)
        {
            for (var i = 0; i < neuralNetwork.Layers.Count; i++)
            {
                for (var j = 0; j < neuralNetwork.Layers[i].Count; j++)
                {
                    var neuron = neuralNetwork.Layers[i][j];

                    for (var k = 0; k < neuron.SynapticWeights.Count(); k++)
                    {
                        var synapticWeightChange = neuron.LearningSpeed * localGradients[i][j];

                        if (i == 0)
                        {
                            synapticWeightChange *= inputSample[k];
                        }
                        else
                        {
                            synapticWeightChange *= neuronOutputs[i - 1][k];
                        }

                        if (previousSynapticWeights != null)
                        {
                            synapticWeightChange += Momentum * previousSynapticWeights[i][j][k];
                        }

                        neuron.SynapticWeights[k] += synapticWeightChange;
                    }
                }
            }
        }
    }
}