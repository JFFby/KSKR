using System;
using System.Windows.Forms;
using Domain;
using MathNet.Numerics.LinearAlgebra;

namespace UI
{
    public partial class LinerMatrixForm : Form
    {
        public EventHandler<LinerMatrixCreationEventArgs> MatrixCreated;

        private const int cellHeight = 30;
        private const int cellwidth = 80;
        private readonly int c, r, width;

        public LinerMatrixForm(int width, int rows)
        {
            c = width * 2 + 1;
            r = rows;
            this.width = width;
            InitializeComponent();
            SetWndsize(c, r);
            InitGrid(c, r);
        }

        private void SetWndsize(int column, int rows)
        {
            Width = cellwidth * column + 95;
            Height = cellHeight * rows + 85;
            dataGridView1.Width = cellwidth * column + 45;
            dataGridView1.Height = cellHeight * rows + 60;
        }

        private void InitGrid(int columns, int rows)
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

                for (int j = 0; j < columns; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0.ToString();
                }
            }
        }

        private void OnMatrixCreated(Matrix<double> matrix)
        {
            EventHandler<LinerMatrixCreationEventArgs> temp = MatrixCreated;
            if (temp != null)
            {
                temp(this, new LinerMatrixCreationEventArgs(matrix));
            }
        }

        private void LinerMatrixForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                var matrix = Matrix<double>.Build.Dense(r, r);
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        var value = dataGridView1.Rows[i].Cells[j].Value ?? 0;
                        matrix[i, j] = double.Parse(value.ToString());
                    }
                }

                var nm = MatrixHelper.LinearToNormal(matrix, width);
                OnMatrixCreated(nm);
            }
            catch (Exception ex)
            {

                OnMatrixCreated(Matrix<double>.Build.Dense(width, width));
            }
        }
    }
}
