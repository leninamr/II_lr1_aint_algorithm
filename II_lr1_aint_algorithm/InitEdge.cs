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
    public partial class InitEdge : Form
    {
        int[][] dists = new int[Gv.numCities][];

        public InitEdge()
        {
            InitializeComponent();
            for (int i = 0; i < dists.Length; ++i)
            {
                dists[i] = new int[Gv.numCities];
                for (int j = 0; j < dists.Length; ++j)
                    dists[i][j] = Int32.MaxValue;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Gv.antColony.SetDists(dists);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //todo проверка на дурака
            dists[Int32.Parse(textBox1.Text) - 1][Int32.Parse(textBox2.Text) - 1] = Int32.Parse(textBox3.Text);
            if (true)  //todo проверка на максимум ребер
            {
                textBox1.Text = "0";
                textBox2.Text = "0";
                textBox3.Text = "0";
                textBox1.Focus();
            } else 
            {
                this.Close();
                Gv.antColony.SetDists(dists);
            }
        }
    }
}
