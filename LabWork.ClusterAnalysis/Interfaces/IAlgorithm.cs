using LabWork.ClusterAnalysis.Entities;

namespace LabWork.ClusterAnalysis.Interfaces
{
    /// <summary>
    /// Интерфейс компонента, реализующего алгоритм кластеризации.
    /// </summary>
    public interface IAlgorithm
    {
        /// <summary>
        /// Метод кластеризации.
        /// </summary>
        /// <param name="vectors">Коллекция векторов, представляющих объекты для кластеризации.</param>
        /// <returns>Коллекция векторов, распределённых по кластерам..</returns>
        ICollection<Label> Execute(ICollection<Label> vectors);
    }
}