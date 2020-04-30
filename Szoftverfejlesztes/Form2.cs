using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Szoftverfejlesztes
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (listBox1.SelectedItem.ToString())
            {
                case "Programtervező informatikus(BSc)":
                    Global.szakid = 8;
                    Form1 f2 = new Form1();
                    f2.Show();
                    break;
                case "Mérnökinformatikus(BSc)":
                    Global.szakid = 6;
                    Form1 f3 = new Form1();
                    f3.Show();
                    break;
            }
        }
    }
}
