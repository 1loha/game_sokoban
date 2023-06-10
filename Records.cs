using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SokobanKR
{
    public partial class Records : Form
    {
        public Records(string str)
        {
            InitializeComponent();
            recordText.Text = str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
