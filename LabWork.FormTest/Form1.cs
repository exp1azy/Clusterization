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
                // ������ ������ �������� ��������� ������ - ���� � ����� � ��������� �������..
                var filename = textBox1.Text;

                // ���� �� ������� �� ������ ���������, �������� �� ������ � ������� �� ���������.
                if (string.IsNullOrWhiteSpace(filename))
                {
                    throw new ApplicationException("������� ��� �����, ���������� ����������� �������� ��������.");
                }

                // ������ ������� �� �����.
                var vectors = File.ReadLines(filename)
                    .Select(line => new Vector(line.Split(";").Select(i => float.Parse(i))));

                // ������ ���������, ���������� ������������� � ������������� ��� ���������.
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

                // ��������� �������������.
                var result = clusterer.Execute(vectors);

                // �� ����������� ������������� ��������� ������ ��� ������ � ���� � ����� �� � ����.
                var vectorLines = result.OrderBy(r => r.Index).Select(r => $"{r.Index};{r.ClusterId};{string.Join(';', r.ReducedVector)}");
                File.WriteAllLines("out.vectors.csv", vectorLines);

                // �� ����������� ��������� �������, �������������� �������� � ����� �� � ����.
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
                MessageBox.Show(this, ex.Message, "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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