using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoNotifyBiz;

namespace WFVideoNotify
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetValue_Click(object sender, EventArgs e)
        {
            YoukuVideoNotify YVN = new YoukuVideoNotify();
            txtResult.Text=YVN.GetSiteValueByXpath(txtSiteUrl.Text,txtXpath.Text);
        }
    }
}
