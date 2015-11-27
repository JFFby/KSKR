using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Domain.Common;

namespace UI
{
    public partial class ScheduleForm : Form
    {
        public ScheduleForm(IList<State> states, string name)
        {
            InitializeComponent();
            this.Text = name;
            DrawChart(states);
        }

        private void DrawChart(IList<State> states)
        {
            foreach (var state in states)
            {
                for (var i = 0; i < state.MovementU.Count; ++i)
                {
                    if (chart1.Series.Count < i + 1)
                    {
                        var series = new Series("U" + (i + 1).ToString())
                        {
                            ChartType = SeriesChartType.Spline,
                            BorderWidth = 3
                        };
                        chart1.Series.Add(series);
                    }

                    chart1.Series[i].Points.AddXY(state.Time, state.MovementU[i]);
                }
            }

        }
    }
}
