using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace II_lr1_aint_algorithm
{
    public partial class File : Form
    {
        public File()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Gv.filename = textBox1.Text;
            try
            {
                Gv.antColony.SetDists(ReadFile(Gv.filename, Gv.numCities));
                this.Close();
            }
            catch (Exception err) //todo ошибка - несуществующи файл
            {
                throw new Exception(err.Message);
            }
        }

        private static int[][] ReadFile(string name, int numCities)
        {
            //открываем файл для чтения
            StreamReader FileReader;
            try
            {
                FileReader = new StreamReader(name);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                throw new Exception("Bad open file");
            }
            //считываем количество городов
            string info;
            info = FileReader.ReadLine();
            try
            {
                numCities = Int32.Parse(info);
            }
            catch (Exception)
            {
                throw new Exception("Bad File");
            }
            //иниицализируем матрицу смежности
            int[][] dists = new int[numCities][];
            for (int i = 0; i < dists.Length; ++i)
            {
                dists[i] = new int[numCities];
                for (int j = 0; j < dists.Length; ++j)
                    dists[i][j] = Int32.MaxValue;
            }
            //последовательно из файла вводим рёбра
            while (!FileReader.EndOfStream)
            {
                info = FileReader.ReadLine();
                string[] temp = new string[3];
                int cur = 0;
                if (info.Length < 1)
                    break;
                for (int i = 0; i < 3 && cur < info.Length; i++)
                {
                    while (info[cur] != ' ')
                    {
                        temp[i] += info[cur];
                        cur++;
                        if (cur == info.Length)
                            break;
                    }
                    cur++;
                }
                dists[Int32.Parse(temp[0]) - 1][Int32.Parse(temp[1]) - 1] = Int32.Parse(temp[2]);
            }
            return dists;
        }


    }
}
