namespace LabWork.ClusterAnalysis.Entities
{
    /// <summary>
    /// Многомерный вектор из начала координат.
    /// </summary>
    public class Vector
    {
        private readonly float[] _values;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="values">Координаты конца вектора в многомерном пространстве.</param>
        public Vector(IEnumerable<float> values)
        {
            _values = values.ToArray();
        }

        /// <summary>
        /// Координаты конца вектора в многомерном пространстве.
        /// </summary>
        public float[] Values => _values;

        /// <summary>
        /// Кол-во измерений, в которых определён вектор.
        /// </summary>
        public int Length => _values.Length;
    }
}