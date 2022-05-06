using LabWork.ClusterAnalysis.Entities;
using System.Diagnostics;

namespace LabWork.ClusterAnalysis.Test
{
    internal class Program
    {
        /// <summary>
        /// Точка входа в тестовую программу.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        static void Main(string[] args)
        {
            // Читаем первый аргумент командной строки - путь к файлу с исходными данными..
            var filename = args.FirstOrDefault();

            // Если не найдено ни одного аргумента, сообщаем об ошибке и выходим из программы.
            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("Укажите имя файла, содержащее признаковое описание объектов.");
                return;
            }

            Console.WriteLine("Идёт расчёт...");

            // Читаем вектора из файла.
            var vectors = File.ReadLines(filename)
                .Select(line => new Vector(line.Split(";").Select(i => float.Parse(i))));

            // Создаём компонент, выолняющий кластеризацию и устанавливаем его параметры.
            var clusterer = new Clusterer
            {
                ReducedVectorLenght = 2,
                Algorithm = new DBSCAN
                {
                    DistanceCalculator = new EuclidDistance(),
                    Epsilon = 0.003f,
                    MinPts = 5
                }
            };

            // Выполнякм кластеризацию.
            var result = clusterer.Execute(vectors);

            // Из результатов кластеризации формируем строки для записи в файл и пишем их в файл.
            var vectorLines = result.OrderBy(r => r.Index).Select(r => $"{r.Index};{r.ClusterId};{string.Join(';', r.ReducedVector)}");
            File.WriteAllLines("out.vectors.csv", vectorLines);            

            // Из результатов формируем объекты, представляющие кластеры и пишем их в файл.
            var clusters = result.OrderBy(i => i.ClusterId)
                .GroupBy(l => l.ClusterId)
                .Select(g => $"{g.Key};{g.Count()}");
            File.WriteAllLines("out.clusters.csv", clusters);

            Console.SetCursorPosition(0, Console.CursorTop - 1);
            ClearCurrentConsoleLine();
            Console.WriteLine("Расчёт выполнен.");

            var startInfo = new ProcessStartInfo()
            {
                FileName = "python.exe",
                Arguments = "LabWork.Visualization.py out.vectors.csv",
                UseShellExecute = false
            };
            Process.Start(startInfo);

            Console.ReadKey();
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}