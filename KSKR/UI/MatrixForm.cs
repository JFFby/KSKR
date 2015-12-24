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

    public partial class MatrixForm : Form
    {
        private const int cellHeight = 30;
        private const int cellwidth = 80;
        private object _algebraObject;
        private readonly VectorType vectorType;
        private int columns, rows;
        private List<RadioButton> readingTypes;
        private int readingType;
        private bool isMatrix;

        public event UpdateMatrix OnUpdateMatrix;
        public event UpdateVector OnUpdateVector;
        public event UpdateLoadsVector OnLoadsVectorUpdate;

        public MatrixForm(string title, int columns, int rows, object algebraObject)
        {
            InitializeComponent();
            isMatrix = algebraObject is Matrix<double>;
            readingTypes = new List<RadioButton> { ordinarMatrix, lineMatrix };
            readingType = Int32.Parse(ConfigurationManager.AppSettings["vectorReadingType"]);
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
            if (isMatrix)
            {
                readingTypes[readingType].Checked = true;
            }
            else
            {
                groupBox1.Visible = false;
                var location = dataGridView1.Location;
                dataGridView1.Location = new Point(location.X, location.Y - 75);
            }
        }

        private void SetWndsize(int column, int rows)
        {
            Width = cellwidth * column + 65;
            Height = cellHeight * rows + (isMatrix ? 165 : 90);
            dataGridView1.Width = cellwidth * column + 45;
            dataGridView1.Height = cellHeight * rows + 60;
            groupBox1.Width = dataGridView1.Width - 5;
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
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = matrix[i, j];
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
            var number = this.readingTypes.IndexOf(this.readingTypes.First(x => x.Checked));
            config.AppSettings.Settings["vectorReadingType"].Value = number.ToString();
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
            return readingTypes[(int) MatrixReadingType.Liner].Checked;
        }

        private void LoadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
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
            }
        }
    }
}