﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Domain.CentralDifference;
using Domain.Common;
using Domain.Habolt;
using Domain.Vilson;
using Domain.Numark;
using MathNet.Numerics.LinearAlgebra.Double;
using UI.Properties;

namespace UI
{

    public partial class MainForm : Form
    {
        private Inputs _inputs;
        private string InputsPath { get { return ConfigurationManager.AppSettings["inputsPath"]; } }
        private readonly List<KeyValuePair<string, IMethod>> _methods;
        private int _freedomDegreese;
        private readonly ConstansForm constantform = new ConstansForm();

        public MainForm()
        {
            _methods = new List<KeyValuePair<string, IMethod>>
            {
                new KeyValuePair<string, IMethod>("Метод центральных разностей", new CentralDifference()),
                new KeyValuePair<string, IMethod>("Метод Хаболта", new Habolt()),
                new KeyValuePair<string, IMethod>("Метод Вилсона", new Vilson()),
                new KeyValuePair<string, IMethod>("Метод Ньюмарка", new Numark())
            };
            PreInitial();
            InitializeComponent();
            InitializeControls();
            constantform.FormClosing += Constantform_FormClosing;
        }

        private void Constantform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                RunSolve();
                constantform.Hide();
            }
        }

        private int ActiveMethod
        {
            get { return groupBox1.Controls.OfType<MyRadioButton>().First(x => x.Checked).Value; }
        }

        private void InitializeControls()
        {
            _inputs = Inputs.LoadObject(InputsPath) ?? new Inputs();
            _inputs.OnInputsUpdate += IsSolveBtnEnable;

            InitFreedomDegreese();
            InitMethodSelector();
            IsSolveBtnEnable();
            InitTValues();
            AlphaBethaInit();
        }

        private void InitTValues()
        {
            textBox1.Text = _inputs.T0.ToString();
            textBox2.Text = _inputs.DeltaT.ToString();
            textBox3.Text = _inputs.Tk.ToString();
        }

        private void AlphaBethaInit()
        {
            textBox4.Text = _inputs.Alpha.ToString();
            textBox5.Text = _inputs.Beta.ToString();
        }

        private void InitMethodSelector()
        {
            var methodRbtn = groupBox1.Controls.OfType<MyRadioButton>()
                .First(x => x.Value == Int32.Parse(ConfigurationManager.AppSettings["method"]));
            methodRbtn.Select();
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
            SaveSolveMethod();
        }

        private void UpdateInput()
        {
            textBox1_TextChanged(textBox1, null);
            textBox2_TextChanged(textBox2, null);
            textBox3_TextChanged(textBox3, null);
        }

        private void solveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateInput();
                if(ActiveMethod==2) { ShowConstatntForm(1); }
                else
                if (ActiveMethod == 3) { ShowConstatntForm(2); }
                else
                {
                    RunSolve();
                }             
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show(Resources.mainForm_solveBtn_Click_Метод_не_реализован);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Не удалось распознать аргументы.");
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка, повторите расчёт.");
            }
        }

        private void RunSolve()
        {
            var method = _methods[ActiveMethod];
            var result = method.Value.Solve(_inputs);
            if (result.Any())
            {
                var form = new ScheduleForm(result, method.Key);
                form.Show();

                var dataForm = new DataForm(result, method.Key);
                dataForm.Show();
            }
        }

        private void ShowConstatntForm(int index)
        {
            constantform.ChoiceMethod(index);
            constantform.SetConstantsValue += Constantform_SetConstantsValue;
            constantform.Show();
        }

        private void Constantform_SetConstantsValue(double constant, int indexMethod)
        {
            if (indexMethod == 1) { _inputs.Teta = constant; }
            if (indexMethod == 2) { _inputs.Delta = constant; }
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
            numericUpDown1.Maximum = 6;
            numericUpDown1.Minimum = 2;
            numericUpDown1.Value = numericUpDown1.Maximum > _freedomDegreese
                                   && numericUpDown1.Minimum < _freedomDegreese
                ? _freedomDegreese
                : 2; 
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
        }

        private void SaveFreedomDeegrese()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings["freedomDegreese"].Value = _freedomDegreese.ToString();
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void SaveSolveMethod()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings["method"].Value = ActiveMethod.ToString();
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

        private void uSetBtn_Click(object sender, EventArgs e)
        {
            _inputs.MovementU = _inputs.MovementU ?? DenseVector.OfArray(new double[_freedomDegreese]);
            var form = new MatrixForm("Вектор перемещений", _freedomDegreese, 1, _inputs.MovementU);
            form.OnUpdateVector += uUpdateVector;
            form.Show();
        }

        private void kSet_Click(object sender, EventArgs e)
        {
            _inputs.K = _inputs.K ?? DenseMatrix.OfArray(new double[_freedomDegreese, _freedomDegreese]);
            var form = new MatrixForm("Матрица Жесткости", _freedomDegreese, _freedomDegreese, _inputs.K);
            form.OnUpdateMatrix += KUpdateMatrix;
            form.Show();
        }

        private void KUpdateMatrix(IEnumerable<IEnumerable<double>> matrix)
        {
            _inputs.K = DenseMatrix.OfRows(matrix);
        }

        private void _uSetBtn_Click(object sender, EventArgs e)
        {
            _inputs.SpeedU = _inputs.SpeedU ?? DenseVector.OfArray(new double[_freedomDegreese]);
            var form = new MatrixForm("Вектор скоростей", _freedomDegreese, 1, _inputs.SpeedU);
            form.OnUpdateVector += _uUpdateVector;
            form.Show();
        }

        private void _uUpdateVector(IEnumerable<double> vector)
        {
            _inputs.SpeedU = DenseVector.OfEnumerable(vector);
        }

        private void rSetBtn_Click(object sender, EventArgs e)
        {
            _inputs.R = _inputs.R ?? new LoadsVector(new string[_freedomDegreese]);
            var form = new MatrixForm("Вектор скоростей", _freedomDegreese, 1, _inputs.R);
            form.OnLoadsVectorUpdate += rUpdateVector;
            form.Show();
        }

        private void rUpdateVector(string[] vector)
        {
            _inputs.R = new LoadsVector(vector);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            _inputs.Alpha = ProccessTextBoxValue(textBox4.Text);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            _inputs.Beta = ProccessTextBoxValue(textBox5.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            _inputs.DeltaT = ProccessTextBoxValue(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            _inputs.Tk = ProccessTextBoxValue(textBox3.Text);
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["help"];
            Process.Start(url);
        }
    }
}