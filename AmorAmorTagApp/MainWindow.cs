using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amor_Amor_Impressão_de_Tag
{
    public partial class OptionForm : Form
    {

        public string ApiToken
        {
            get { return apiTokenTextBox.Text; }
            set { apiTokenTextBox.Text = value; }
        }

        public OptionForm(string currentApiToken)
        {
            InitializeComponent();
        }

    }
}