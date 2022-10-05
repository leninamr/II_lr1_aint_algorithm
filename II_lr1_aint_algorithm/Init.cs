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
    public partial class Init : Form
    {
        public Init()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //todo защита от дурака
            Gv.numCities = Int32.Parse(textBox1.Text);
            //последовательно считываем данные о рёбрах
            if (Gv.numCities > 0)  
                ShowDialogueForm(new InitEdge());
            this.Close();
        }
        private void ShowDialogueForm(Form form)
        {
            form.ShowDialog(this);
            form.Dispose();
        }
    }
}
