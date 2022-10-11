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
    public partial class Countries : Form
    {
        public Countries()
        {
            InitializeComponent();
        }

        public void set_labels_banned(string str)
        {
            label3.Text = str;
        }

        public void set_labels_hosts(string str)
        {
            label4.Text = str;
        }
    }
}
