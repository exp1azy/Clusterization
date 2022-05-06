namespace LabWork.ClusterAnalysis.Interfaces
{
    /// <summary>
    /// Интерфейс компонента для вычисления расстояния между векторами.
    /// </summary>
    public interface IDistanceCalculator
    {
        /// <summary>
        /// Метод вычисления расстояния между векторами.
        /// </summary>
        /// <param name="v1">Координаты первого вектора.</param>
        /// <param name="v2">Координаты второго вектора.</param>
        /// <returns>Расстояние между векторами.</returns>
        float Calc(float[] v1, float[] v2);
    }
}