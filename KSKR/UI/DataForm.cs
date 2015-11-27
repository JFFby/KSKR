using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Domain.Common;

namespace UI
{
    public partial class DataForm : Form
    {
        private const int cellHeight = 30;
        private const int cellwidth = 80;

        public DataForm(IList<State> states, string methodName)
        {
            InitializeComponent();
            Text = methodName;
            var columns = states.First().MovementU.Count + 1;
            var rows = states.Count();
            InitGrid(columns, rows, states);
            SetWndsize(columns, rows);
        }

        private void InitGrid(int columns, int rows, IList<State> states)
        {
            for (int i = 0; i < columns; i++)
            {
                if (i == 0)
                {
                    dataGridView1.Columns.Add((i + 1).ToString(), "T");
                    dataGridView1.Columns[i].Width = cellwidth;
                }
                else
                {
                    dataGridView1.Columns.Add((i + 1).ToString(), "U" + i.ToString());
                    dataGridView1.Columns[i].Width = cellwidth;
                }

            }

            for (int i = 0; i < rows; i++)
            {
                dataGridView1.Rows.Add(new DataGridViewRow()
                {
                    Height = cellHeight,
                    HeaderCell = new DataGridViewRowHeaderCell() { Value = (i + 1).ToString() }
                });

                for (int x = 0; x < columns; ++x)
                {
                    if (x == 0)
                    {
                        dataGridView1.Rows[i].Cells[x].Value = states[i].Time;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[x].Value = states[i].MovementU[x - 1];
                    }
                }

            }
        }

        private void SetWndsize(int column, int rows)
        {
            Width = cellwidth * column + 65;
            Height = cellHeight * rows + 75;
            dataGridView1.Width = cellwidth * column + 40;
            dataGridView1.Height = cellHeight * rows + 60;
        }
    }
}
