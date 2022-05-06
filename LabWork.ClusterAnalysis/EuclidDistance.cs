using LabWork.ClusterAnalysis.Interfaces;

namespace LabWork.ClusterAnalysis
{
    /// <summary>
    /// Реализация вычисления Евклидова расстояния между векторами.
    /// </summary>
    public class EuclidDistance : IDistanceCalculator
    {
        /// <summary>
        /// Метод вычисления Евклидова расстояния между векторами.
        /// </summary>
        /// <param name="v1">Координаты первого вектора.</param>
        /// <param name="v2">Координаты второго вектора.</param>
        /// <returns>Расстояние между векторами.</returns>
        public float Calc(float[] v1, float[] v2)
        {
            var e1 = v1.GetEnumerator();
            var e2 = v2.GetEnumerator();

            double acc = 0;

            while (e1.MoveNext() && e2.MoveNext())
            {
                acc += Math.Pow(Math.Abs((float)e1.Current - (float)e2.Current), 2);
            }

            return (float)Math.Sqrt(acc);
        }
    }
}