using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZTRForm
{
    public partial class Form1 : Form
    {
        public RobotConnection rc { get; set; }
        public Form1()
        {
            InitializeComponent();
            rc = new RobotConnection();
        }

        private void SetText()
        {
            txtValue.Text = string.Format("L{0:000}R{1:000}", slLeft.Value * 10, slRight.Value * 10);
            txtResults.Text = rc.Send(txtValue.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetText();
        }

        private void slRight_ValueChanged(object sender, EventArgs e)
        {
            SetText();
        }

        private void slLeft_MouseUp(object sender, MouseEventArgs e)
        {
            slLeft.Value = 0;
        }

        private void slRight_MouseUp(object sender, MouseEventArgs e)
        {
            slRight.Value = 0;
        }

        private void txtIpAddress_TextChanged(object sender, EventArgs e)
        {
            rc.address = txtIpAddress.Text;
        }
    }
}
