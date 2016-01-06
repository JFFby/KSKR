using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Domain;
using Domain.Common;
using Domain.Enums;
using MathNet.Numerics.LinearAlgebra;
using UI.Properties;

namespace UI
{
    public delegate void UpdateMatrix(IEnumerable<IEnumerable<double>> matix);

    public delegate void UpdateVector(IEnumerable<double> vector);

    public delegate void UpdateLoadsVector(string[] vector);

    internal partial class MatrixForm : Form
    {
        private const int cellHeight = 30;
        private const int cellwidth = 80;
        private readonly object _algebraObject;
        private readonly VectorType vectorType;
        private readonly int columns;
        private readonly int rows;
        private readonly List<RadioButton> readingTypes;
        private readonly List<RadioButton> matrixTypes;
        private readonly int readingType;
        private readonly int matrixType;
        private readonly bool isMatrix;

        public event UpdateMatrix OnUpdateMatrix;
        public event UpdateVector OnUpdateVector;
        public event UpdateLoadsVector OnLoadsVectorUpdate;

        public MatrixForm(string title, int columns, int rows, object algebraObject)
        {
            InitializeComponent();
            isMatrix = algebraObject is Matrix<double>;
            matrixTypes = new List<RadioButton> { ordinarMatrix, lineMatrix };
            readingTypes = new List<RadioButton> { radioButton1, radioButton2 };
            readingType = Int32.Parse(ConfigurationManager.AppSettings["vectorReadingType"]);
            matrixType = Int32.Parse(ConfigurationManager.AppSettings["matrixType"]);
            InitRadioButtins();
            Text = title;
            this.columns = columns;
            this.rows = rows;
            SetWndsize(columns, rows);
            InitGrid();
            vectorType = SetGrid(algebraObject, columns, rows);
            this._algebraObject = algebraObject;
        }

        private void InitRadioButtins()
        {
            label1.Visible = false;
            loadFromFileBtn.Visible = false;
            textBox1.Visible = false;
            button2.Visible = false;
            groupBox2.Visible = false;
            if (isMatrix)
            {
                dataGridView1.Visible = false;
                matrixTypes[matrixType].Checked = true;
                menuStrip1.Visible = false;
            }
            else
            {
                dataGridView1.Visible = true;
                groupBox1.Visible = false;
                var location = dataGridView1.Location;
                dataGridView1.Location = new Point(location.X, location.Y - 160);
            }
        }

        private void SetWndsize(int column, int rows)
        {
            Width = (cellwidth * column > groupBox1.Width) || !isMatrix 
                ? cellwidth * column + (isMatrix ? 90 : 75)
                : groupBox1.Width + 5;
            Height = cellHeight * rows + (isMatrix ? 285 : 95);
            dataGridView1.Width = cellwidth * column + 45;
            dataGridView1.Height = cellHeight * rows + 60;
            groupBox1.Width = dataGridView1.Width - 5;
            groupBox2.Width = dataGridView1.Width - 5;
            loadFromFileBtn.Width = dataGridView1.Width - 5;
        }

        private void InitGrid()
        {
            for (int i = 0; i < columns; i++)
            {
                dataGridView1.Columns.Add((i + 1).ToString(), (i + 1).ToString());
                dataGridView1.Columns[i].Width = cellwidth;

            }

            for (int i = 0; i < rows; i++)
            {
                dataGridView1.Rows.Add(new DataGridViewRow()
                {
                    Height = cellHeight,
                    HeaderCell = new DataGridViewRowHeaderCell() { Value = (i + 1).ToString() }
                });
            }
        }

        private VectorType SetGrid(object obj, int colCount, int rowCount)
        {
            var type = VectorType.Undefined;
            if (obj != null)
            {
                if (isMatrix)
                {
                    SetGridMatrixVlaues(obj as Matrix<double>, colCount, rowCount);
                    type = VectorType.Matrix;
                }

                if (obj is Vector<double>)
                {
                    SetGridVectorValues(obj as Vector<double>, colCount);
                    type = VectorType.Vector;
                }

                if (obj is LoadsVector)
                {
                    SetGridLoadsVectorValues(obj as LoadsVector, colCount);
                    type = VectorType.LoadsVector;
                }
            }

            return type;
        }

        private void SetGridMatrixVlaues(Matrix<double> matrix, int colCount, int rowCount)
        {
            if (matrix.RowCount != rowCount || matrix.ColumnCount != colCount)
            {
                SetWithZero(colCount, rowCount);
                return;
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = matrix[i, j];
                }
            }
        }

        private void SetWithZero(int colCount, int rowCount)
        {
            MessageBox.Show("Текущая матрица имеет недопустимые размеры. Значения будут обнулены");
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0;
                }
            }
        }

        private void SetGridVectorValues(Vector<double> vector, int colCount)
        {
            for (int i = 0; i < colCount; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = vector[i];
            }
        }

        private void SetGridLoadsVectorValues(LoadsVector vector, int colCount)
        {
            for (int i = 0; i < colCount; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = vector.Vector == null
                    ? "0"
                    : string.IsNullOrEmpty(vector.Vector[i]) ? "0" : vector.Vector[i];
            }
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (_algebraObject is Vector<double>)
            {
                OnUpdateVector(GetVectorValue());
            }

            if (isMatrix)
            {
                OnUpdateMatrix(GetMatrixValue());
            }

            if (_algebraObject is LoadsVector)
            {
                OnLoadsVectorUpdate(GetLoadsVector());
            }

            if (isMatrix)
            {
                SaveMatrixReadinType();
            }
        }

        private void SaveMatrixReadinType()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            var rt = this.readingTypes.IndexOf(this.readingTypes.First(x => x.Checked));
            var mt = this.matrixTypes.IndexOf(this.matrixTypes.First(x => x.Checked));
            config.AppSettings.Settings["vectorReadingType"].Value = rt.ToString();
            config.AppSettings.Settings["matrixType"].Value = mt.ToString();
            config.Save(ConfigurationSaveMode.Modified);
        }

        private IEnumerable<double> GetVectorValue()
        {
            var vector = new List<double>();
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                vector.Add(GetCellValue(dataGridView1.Rows[0].Cells[i].Value.ToString()));
            }

            return vector;
        }

        private IEnumerable<IEnumerable<double>> GetMatrixValue()
        {
            var matrix = new List<List<double>>();
            matrix.Add(GetVectorValue().ToList());
            for (int i = 1; i < dataGridView1.RowCount; i++)
            {
                var vector = new List<double>();
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    vector.Add(GetCellValue(dataGridView1.Rows[i].Cells[j].Value.ToString()));
                }

                matrix.Add(vector);
            }

            return matrix;
        }

        private double GetCellValue(string value)
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

        private string[] GetLoadsVector()
        {
            var vector = new string[dataGridView1.ColumnCount];
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                var value = dataGridView1.Rows[0].Cells[i].Value;
                vector[i] = string.IsNullOrEmpty(value == null ? "" : value.ToString()) ? "0" : value.ToString();
            }

            return vector;
        }

        private bool IsLinerReadingType()
        {
            return matrixTypes[(int)MatrixType.Linear].Checked;
        }

        private void matrixtype_CheckedChanged(object sender, EventArgs e)
        {
            if (matrixTypes.Any(x => x.Checked))
            {
                if (groupBox2.Visible)
                {
                    var mt = (MatrixType)matrixTypes.IndexOf(matrixTypes.First(x => x.Checked));
                    var rt = (MatrixReadingType)readingTypes.IndexOf(readingTypes.First(x => x.Checked));
                    UpdateControlsVisibiblity(mt, rt);
                }
                else
                {
                    groupBox2.Visible = true;
                    readingTypes[readingType].Checked = true;
                }
            }
        }

        private void readingType_CheckedChanged(object sender, EventArgs e)
        {
            if (readingTypes.Any(x => x.Checked))
            {
                var mt = (MatrixType)matrixTypes.IndexOf(matrixTypes.First(x => x.Checked));
                var rt = (MatrixReadingType)readingTypes.IndexOf(readingTypes.First(x => x.Checked));
                UpdateControlsVisibiblity(mt, rt);
            }
        }

        private void UpdateControlsVisibiblity(MatrixType mt, MatrixReadingType rt)
        {
            if (mt == MatrixType.Normal && rt == MatrixReadingType.Typing)
            {
                dataGridView1.Visible = true;
                label1.Visible = false;
                loadFromFileBtn.Visible = false;
                textBox1.Visible = false;
                button2.Visible = false;
            }

            if (rt == MatrixReadingType.FromFile)
            {
                dataGridView1.Visible = false;
                label1.Visible = false;
                loadFromFileBtn.Visible = true;
                textBox1.Visible = false;
                button2.Visible = false;
            }

            if (mt == MatrixType.Linear && rt == MatrixReadingType.Typing)
            {
                dataGridView1.Visible = false;
                label1.Visible = true;
                loadFromFileBtn.Visible = false;
                textBox1.Visible = true;
                button2.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int lColCount = 0;
            if (Int32.TryParse(textBox1.Text, out lColCount) && lColCount < columns && lColCount > 0)
            {
                var lmf = new LinerMatrixForm(lColCount, rows);
                lmf.MatrixCreated += SetMatrixValue;
                lmf.Show();
            }
            else
            {
                MessageBox.Show("Недопустимое значение, повторите попытку");
            }
        }

        private void SetMatrixValue(object sender, LinerMatrixCreationEventArgs e)
        {
            SetGridMatrixVlaues(e.Matrix, e.Matrix.ColumnCount, e.Matrix.ColumnCount);
            dataGridView1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadFromFile();
        }

        private void считатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadFromFile();
        }

        private void ReadFromFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var vectorString = File.ReadAllText(openFileDialog1.FileName);
                var matrixHelper = new MatrixHelper(vectorString, vectorType, columns, rows, IsLinerReadingType());
                var result = matrixHelper.Resolve();
                if (!string.IsNullOrEmpty(result.Item2))
                {
                    MessageBox.Show(result.Item2);
                }

                this.SetGrid(result.Item1, columns, rows);
                dataGridView1.Visible = true;
            }
        }
    }
}