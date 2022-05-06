using LabWork.ClusterAnalysis.Entities;
using LabWork.ClusterAnalysis.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace LabWork.ClusterAnalysis
{
    /// <summary>
    /// Основной компонент, выполняющий кластеризацию.
    /// </summary>
    public class Clusterer
    {
        /// <summary>
        /// Длина свернутых векторов, после PCA.
        /// </summary>
        public int ReducedVectorLenght { get; set; }

        /// <summary>
        /// Реализация алгоритма кластеризации.
        /// </summary>
        public IAlgorithm? Algorithm { get; set; }

        /// <summary>
        /// Метод запуска кластеризации.
        /// </summary>
        /// <param name="vectors">Перечисление векторов, представляющих объекты, подлежащие кластеризации.</param>
        /// <returns>Коллекция размеченых векторов, описывающих принадлежность объектов к кластерам.</returns>
        /// <exception cref="InvalidOperationException">В случае, если неверно установлены парметры кластеризации.</exception>
        /// <exception cref="ArgumentException">В случае, если входящие вектора не прошли проверку на корректность.</exception>
        public ICollection<Label> Execute(IEnumerable<Vector> vectors)
        {
            // Проверяем, что алгоритм выбран.
            if (Algorithm == null)
                throw new InvalidOperationException("Перед запуском кластеризации необходимо указать алгоритм.");

            // Длина свернутых векторов должна быть не меньше 2.
            if (ReducedVectorLenght < 2)
                throw new InvalidOperationException("Длина свернутых векторов должна быть больше 1.");

            // Вычисляем длину исходных векторов, попутно проверяем входные данные.
            var initialVectorLength = GetVectorLength(vectors);

            // Выполняем свертку исходных векторов до указанной размерности методом главных компонент.
            var reducedVectors = Pca(initialVectorLength, vectors, ReducedVectorLenght);

            // Выполняем кластеризацию и возвращаем результат.
            return Algorithm.Execute(reducedVectors);
        }

        /// <summary>
        /// Метод определения длины исходных векторов.
        /// </summary>
        /// <remarks>Реализует этап проверки входных данных.</remarks>
        /// <param name="vectors">Перечисление векторов.</param>
        /// <returns>Длина векторов.</returns>
        /// <exception cref="ArgumentException">В случае, если вектора не прошли проверку на корректность.</exception>
        private static int GetVectorLength(IEnumerable<Vector> vectors)
        {
            // Группируем векторы по их длине.
            var groups = vectors.GroupBy(v => v.Length).Count();

            // Если не получилось ни одной группы, значит была передана пустая коллекция исходных векторов.
            if (groups == 0)
                throw new ArgumentException("Коллекция векторов не может быть пустой", nameof(vectors));

            // Если групп больше одной, значит длины векторов разные.
            if (groups > 1)
                throw new ArgumentException("Вектора должны быть одинаковой длины", nameof(vectors));

            return vectors.First().Length;
        }

        /// <summary>
        /// Метод получения вектора пониженной размерности с помощью PCA.
        /// </summary>
        /// <param name="initialVectorLength">Размерность исходных векторов.</param>
        /// <param name="vectors">Перечисление исходных векторов</param>
        /// <param name="rank">Размерность свернутых векторов.</param>
        /// <returns>Коллекция свернутых векторов.</returns>
        private static List<Label> Pca(int initialVectorLength, IEnumerable<Vector> vectors, int rank)
        {
            // Создаём схему входных данных для конвейера.
            var schema = SchemaDefinition.Create(typeof(Vector));
            schema.RemoveAt(1);
            var featureColumn = schema[0];
            var itemType = ((VectorDataViewType)featureColumn.ColumnType).ItemType;
            featureColumn.ColumnType = new VectorDataViewType(itemType, initialVectorLength);

            // Создаём контекст ML.
            var mlContext = new MLContext();

            // Готовим входные данные.
            var inData = mlContext.Data.LoadFromEnumerable(vectors, schema);

            // Создаём конвейер, выполняющий PCA, и запускаем трансформацию.
            var pcaData = mlContext.Transforms.ProjectToPrincipalComponents(outputColumnName: "ReducedVector", inputColumnName: "Values", rank: rank, seed: rank)
                .Append(mlContext.Transforms.DropColumns("Values"))
                .Fit(inData).Transform(inData);

            // Возвращаем результат.
            return mlContext.Data.CreateEnumerable<Label>(pcaData, reuseRowObject: false, ignoreMissingColumns: true)
                .Select((v, i) =>
                {
                    v.Index = i;                    
                    return v;
                }).ToList();
        }
    }
}