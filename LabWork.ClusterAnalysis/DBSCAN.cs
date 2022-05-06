using LabWork.ClusterAnalysis.Entities;
using LabWork.ClusterAnalysis.Interfaces;

namespace LabWork.ClusterAnalysis
{
    /// <summary>
    /// Реализация алгоритма кластеризации DBSCAN.
    /// </summary>
    public class DBSCAN : IAlgorithm
    {
        /// <summary>
        /// Максимальный радиус соседства.
        /// </summary>
        public float Epsilon { get; set; }

        /// <summary>
        /// Минимальное количество объектов, способных создать кластер.
        /// </summary>
        public int MinPts { get; set; }

        /// <summary>
        /// Компонент вычисления расстояния между векторами.
        /// </summary>
        public IDistanceCalculator DistanceCalculator { get; set; }

        /// <summary>
        /// Метод кластеризации.
        /// </summary>
        /// <param name="vectors">Коллекция векторов, представляющих объекты для кластеризации.</param>
        /// <returns>Коллекция векторов, распределённых по кластерам.</returns>
        /// <exception cref="InvalidOperationException">В случае, если заданы не корректные парметры алгоритма.</exception>
        public ICollection<Label> Execute(ICollection<Label> vectors)
        {
            if (Epsilon <= 0)
                throw new InvalidOperationException("Максимальный радиус соседства должен быть больше нуля.");

            if (MinPts <= 1)
                throw new InvalidOperationException("Максимальный радиус соседства должен быть больше 1.");

            if (DistanceCalculator == null)
                throw new InvalidOperationException("Не установлен калькулятор растояний.");

            var result = new List<Label>();

            var cluster = 0;
            var counter = 1;

            while (true)
            {
                var current = vectors.Skip(counter - 1).FirstOrDefault();
                if (current == null)
                    break;

                if(!result.Any(i => i.Index == current.Index))
                {
                    var neighbors = FindNeighbours(vectors, current);
                    if (neighbors.Count < MinPts)
                    {
                        current.ClusterId = 0;
                        result.Add(current);
                    }
                    else
                    {
                        cluster++;
                        current.ClusterId = cluster;
                        result.Add(current);

                        var seeds = neighbors.Where(s => s.Index != current.Index).ToList();
                        for (int i = 0; i < seeds.Count; i++)
                        {
                            var l = result.FirstOrDefault(r => r.Index == seeds[i].Index);
                            if (l != null && l.ClusterId == 0)
                                l.ClusterId = cluster;
                            if (l != null)
                                continue;
                             
                            seeds[i].ClusterId = cluster;
                            result.Add(seeds[i]);

                            var n = FindNeighbours(vectors, seeds[i]);
                            if (n.Count >= MinPts)
                                seeds.AddRange(n);
                            
                        }
                    }
                }
                counter++;
            }

            return result;
        }

        /// <summary>
        /// Метод поиска соседей.
        /// </summary>
        /// <param name="vectors">Коллекция векторов, в рамках которой будет выполняться поиск.</param>
        /// <param name="vector">Вектор, относительно которого будет выполняться поиск соседей.</param>
        /// <returns>Коллекция векторов-соседей.</returns>
        private List<Label> FindNeighbours(ICollection<Label> vectors, Label vector)
        {
            var result = new List<Label>();

            foreach (var v in vectors)
            {
                var distance = DistanceCalculator.Calc(v.ReducedVector, vector.ReducedVector);

                if (distance <= Epsilon)
                    result.Add(v);
            }

            return result;
        }
    }
}