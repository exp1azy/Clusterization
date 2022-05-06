using LabWork.ClusterAnalysis;
using LabWork.ClusterAnalysis.Entities;
using System.Diagnostics;

namespace LabWork.FormTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Читаем первый аргумент командной строки - путь к файлу с исходными данными..
                var filename = textBox1.Text;

                // Если не найдено ни одного аргумента, сообщаем об ошибке и выходим из программы.
                if (string.IsNullOrWhiteSpace(filename))
                {
                    throw new ApplicationException("Укажите имя файла, содержащее признаковое описание объектов.");
                }

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

                var startInfo = new ProcessStartInfo()
                {
                    FileName = "python.exe",
                    Arguments = "LabWork.Visualization.py out.vectors.csv",
                    UseShellExecute = false, CreateNoWindow = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is not OpenFileDialog ofd)
                return;

            textBox1.Text = ofd.FileName;
        }
    }
}