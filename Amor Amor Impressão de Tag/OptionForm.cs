using Amor_Amor_Impressão_de_Tag.Properties;
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

        private void Confirmar_Click(object sender, EventArgs e)
        {
            // Save the API token value to your settings or configuration.
            // Assuming you have a static class named Settings to save the API token.
            // Replace Settings with your actual class or method to save the API token.
            Settings.ApiToken = apiTokenTextBox.Text;
            this.Close();
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            // Close the form without saving the changes.
            this.Close();
        }

        private void apiTokenTextBox_TextChanged(object sender, EventArgs e)
        {
            // Enable the Confirmar button only when the text box is not empty.
            Confirmar.Enabled = !string.IsNullOrEmpty(apiTokenTextBox.Text);
        }
    }
}
