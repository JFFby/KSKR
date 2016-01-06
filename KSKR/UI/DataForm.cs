using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Domain.Common;
using System.IO;

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
            var rows = states.Count;
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

        private void SaveDataInFileToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int oneCellWidth = GetMaxLengthString(0);
                int otherCellWidth = GetMaxLengthString(1);
                
                FileStream fs = new FileStream(saveFileDialog.FileName+ ".txt", FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fs);
                WriteLineFromSymbols(streamWriter, "-");
                streamWriter.WriteLine();

                for (int i = 0; i < dataGridView1.Rows[0].Cells.Count; i++)
                {
                    if (i == 0)
                    {
                        streamWriter.Write("| " + DataReturnForFileReader(dataGridView1.Columns[i].HeaderText, oneCellWidth) + "  ");
                    }
                    else
                    {
                        streamWriter.Write("| " + DataReturnForFileReader(dataGridView1.Columns[i].HeaderText, otherCellWidth) + "  ");
                    }
                }

                streamWriter.WriteLine();
                WriteLineFromSymbols(streamWriter, "+");
                streamWriter.WriteLine();

                try
                {
                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        for (int i = 0; i < dataGridView1.Rows[j].Cells.Count; i++)
                        {
                            if (i == 0)
                            {
                                streamWriter.Write("| " + DataReturnForFileReader(dataGridView1.Rows[j].Cells[i].Value.ToString(), oneCellWidth) + "  ");
                            }
                            else
                            {
                                streamWriter.Write("| " + DataReturnForFileReader(dataGridView1.Rows[j].Cells[i].Value.ToString(), otherCellWidth) + "  ");
                            }
                        }

                        streamWriter.WriteLine();
                    }
                    streamWriter.Close();
                    fs.Close();
                    MessageBox.Show("Файл успешно сохранен");
                }
                catch
                {
                    MessageBox.Show("Ошибка при сохранении файла!");
                }
            }
        }

        private void WriteLineFromSymbols(StreamWriter streamWriter, string symbol)
        {
            int maxLength = 0;
            string line = string.Empty;
            for (int i = 0; i < dataGridView1.Rows[0].Cells.Count; i++)
            {
                if (i == 0)
                {
                    maxLength += GetMaxLengthString(0);
                }
                else
                {
                    maxLength += GetMaxLengthString(1);
                }
            }
            for (int i = 0; i < maxLength+10; i++)
            {
                line += symbol;
            }
            streamWriter.Write(line);
        }

        private int GetMaxLengthString(int cellNumber)
        {
            int length = 0;
            string Value = string.Empty;
            for (int j = 0; j < dataGridView1.Rows.Count; j++)
            {
                if(dataGridView1.Rows[j].Cells[cellNumber].Value.ToString().Length > Value.Length)
                {
                    length = dataGridView1.Rows[j].Cells[cellNumber].Value.ToString().Length;
                }
            }
            return length;
        }

        private string DataReturnForFileReader(string inputString, int maxLength)
        {
            string result = string.Empty;
            for (int i = 0; i < maxLength; i++)
            {
                try
                {
                    result = result + inputString[i].ToString();
                }
                catch(System.IndexOutOfRangeException)
                {
                    result = result + " ";
                }
            }
            return result;
        }
    }
}
