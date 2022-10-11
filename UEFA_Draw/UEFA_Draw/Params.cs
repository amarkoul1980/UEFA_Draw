using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UEFA_Draw
{
    public partial class Params : Form
    {
        IniFile MyIni;
        public Params()
        {
            InitializeComponent();

            MyIni = new IniFile("properties.ini");

            label8.Text= MyIni.Read("Pots");
            label9.Text= MyIni.Read("Groups");
            label10.Text= MyIni.Read("Small_Groups");
            label11.Text= MyIni.Read("Large_Groups");
            label12.Text= MyIni.Read("Small_Groups_Teams");
            label13.Text= MyIni.Read("Large_Groups_Teams");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
