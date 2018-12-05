using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserListener
{
    public partial class FormShowTotal : Form
    {
        public FormShowTotal(List<string> list)
        {
            InitializeComponent();
            foreach (var str in list)
            {
                listBox1.Items.Add(str);
            }
        }
    }
}