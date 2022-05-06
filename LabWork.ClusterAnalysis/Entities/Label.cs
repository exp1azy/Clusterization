namespace LabWork.ClusterAnalysis.Entities
{
    /// <summary>
    /// Размеченный объект.
    /// </summary>
    public class Label
    {
        /// <summary>
        /// Порядковый индекс объекта в исходной выборке.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Идентификатор кластера, к котрому принадлежит объект.
        /// </summary>
        public int ClusterId { get; set; }

        /// <summary>
        /// Вектор пониженной размерности, соответствующий объекту.
        /// </summary>
        public float[] ReducedVector { get; set; }
    }
}