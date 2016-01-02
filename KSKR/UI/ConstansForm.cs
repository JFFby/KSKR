using System;
using System.Windows.Forms;
using UI.Properties;

namespace UI
{
    public partial class ConstansForm : Form
    {
        public delegate void SetConstants(double constant, int indexMethod);
        public event SetConstants SetConstantsValue;

        public ConstansForm()
        {
            InitializeComponent();
        }

        public void ChoiceMethod(int index)
        {
            if (index == 1)
            {
                VilsonGroupBox.Visible = true;
                NumarkGroupBox.Visible = false;
            }
            if (index == 2)
            {
                VilsonGroupBox.Visible = false;
                NumarkGroupBox.Visible = true;
            }
        }

        private void VilsonInputTextBox_TextChanged(object sender, EventArgs e)
        {
            SetConstantsValue(ProccessTextBoxValue(VilsonInputTextBox.Text), 1);
        }

        private void NumarkInputTextBox_TextChanged(object sender, EventArgs e)
        {
            SetConstantsValue(ProccessTextBoxValue(NumarkInputTextBox.Text), 2);
        }

        private double ProccessTextBoxValue(string value)
        {
            value = StringValueHelper.ProcessValue(value);
            try
            {
                return Double.Parse(value);
            }
            catch (FormatException)
            {
                MessageBox.Show(Resources.MainForm_ProccessTextBoxValue_Значение_должно_иметь_числовой_формат);
                return default(double);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
