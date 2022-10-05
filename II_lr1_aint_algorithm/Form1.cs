namespace II_lr1_aint_algorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Gv.antColony = new AntColony();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //todo проверка на дурака
            int alpha = Int32.Parse(textBox1.Text);
            int beta = Int32.Parse(textBox2.Text);
            double rho = Double.Parse(textBox3.Text);
            double Q = Double.Parse(textBox4.Text);

            int numCities = Int32.Parse(textBox5.Text);
            int numAnts = Int32.Parse(textBox6.Text);
            int maxTime = Int32.Parse(textBox7.Text);
            Gv.antColony = new AntColony(alpha, beta, rho, Q, numCities, numAnts, maxTime);
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowDialogueForm(new Init());
        }

        private void ShowDialogueForm(Form form)
        {
            form.ShowDialog(this);
            form.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowDialogueForm(new File());
        }
    }
}