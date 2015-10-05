using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using Domain.CentralDifference;
using Domain.Common;
using Domain.Habolt;
using MathNet.Numerics.LinearAlgebra.Double;
using UI.Properties;

namespace UI
{ 
    //TODO: 
    /// <summary>
    /// matrix k (as matrix M)
    /// alpha
    /// betha
    /// U (as matrix M)
    /// U' (as matrix M)
    /// R (as matrix M)
    /// T0
    /// Tk
    /// dT
    /// Methods selection
    /// </summary>
    public partial class MainForm : Form
    {
        private Inputs _inputs;
        private string InputsPath { get { return ConfigurationManager.AppSettings["inputsPath"]; } }
        private readonly List<IMethod> _methods;
        private int _freedomDegreese;

        public MainForm()
        {
            _methods = new List<IMethod> { new CentralDifference(), new Habolt() };
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            _inputs = Inputs.LoadObject(InputsPath) ?? new Inputs();
            _inputs.OnInputsUpdate += IsSolveBtnEnable;

            InitFreedomDegreese();
            IsSolveBtnEnable();
        }

        private bool IsInputsReady()
        {
            return _inputs.DeltaT > 0
               && _inputs.K != null
               && _inputs.R != null
               && _inputs.M != null
               && _inputs.MovementU != null
               && _inputs.SpeedU != null
               && _inputs.Tk > 0
               && groupBox1.Controls.OfType<RadioButton>().Any(x => x.Checked);
        }

        private void IsSolveBtnEnable()
        {
            solveBtn.Enabled = IsInputsReady();
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _inputs.Serrialize(InputsPath);
            SaveFreedomDeegrese();
        }

        private void solveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                _methods[groupBox1.Controls.OfType<MyRadioButton>().First(x => x.Checked).Value].Solve(_inputs);
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.mainForm_solveBtn_Click_Метод_не_реализован);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _inputs.M = null;
            _inputs.K = null;
            _inputs.MovementU = null;
            _inputs.SpeedU = null;
            _inputs.R = null;

            _freedomDegreese = (int)numericUpDown1.Value;
        }

        private void InitFreedomDegreese()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["freedomDegreese"]))
            {
                _freedomDegreese = Int32.Parse(ConfigurationManager.AppSettings["freedomDegreese"]);
            }
            else
            {
                _freedomDegreese = 2;
            }

            numericUpDown1.Value = _freedomDegreese;
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
        }

        private void SaveFreedomDeegrese()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings["freedomDegreese"].Value = _freedomDegreese.ToString();
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) _inputs.T0 = ProccessTextBoxValue(textBox.Text);
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

        private void mSet_Click(object sender, EventArgs e)
        {
            _inputs.M = _inputs.M ?? DenseMatrix.OfArray(new double[_freedomDegreese, _freedomDegreese]);
            var form = new MatrixForm("Матрица Масс", _freedomDegreese, _freedomDegreese, _inputs.M);
            form.OnUpdateMatrix += MUpdateMatrix;
            form.Show();
        }

        private void uUpdateVector(IEnumerable<double> vector)
        {
            _inputs.MovementU = DenseVector.OfEnumerable(vector);
        }

        private void MUpdateMatrix(IEnumerable<IEnumerable<double>> matrix)
        {
            _inputs.M = DenseMatrix.OfRows(matrix);
        }
    }
}