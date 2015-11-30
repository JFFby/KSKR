using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Domain.Common;

namespace UI
{
    public partial class ScheduleForm : Form
    {
        private IList<State> states;

        public ScheduleForm(IList<State> states, string name)
        {
            InitializeComponent();
            this.Text = name;
            this.states = states;
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

        private void button1_Click(object sender, System.EventArgs e)
        {
            var text = textBox1.Text;
            if (Regex.IsMatch(text, @"\d\s*-\s*\d"))
            {
                var indexes = text.Split('-').Select(x => int.Parse(x.Trim())).ToArray();
                RedrawStates(s => RenderWithInterval(indexes[0] - 1, indexes[1] - 1, s));
            }
            else if (Regex.IsMatch(text, ",") || Regex.IsMatch(text, @"\s*\d\s*"))
            {
                var indexes = text.Split(',').Select(x => int.Parse(x.Trim())).ToArray();
                RedrawStates(s => RenderMany(indexes, s));
            }
        }

        private void RedrawStates(Action<State> action)
        {
            chart1.Series.Clear();
            foreach (var state in states)
            {
                action(state);
            }
        }

        private void RenderMany(IList<int> indexes, State state)
        {
            for (int i = 0; i < indexes.Count; ++i)
            {
                if (chart1.Series.Count < i + 1)
                {
                    chart1.Series.Add(new Series("U" + indexes[i]) { ChartType = SeriesChartType.Spline, BorderWidth = 3 });
                }

                chart1.Series[i].Points.AddXY(state.Time, state.MovementU[indexes[i] - 1]);
            }
        }

        private void RenderWithInterval(int from, int to, State state)
        {
            for (int i = from, j = 0; i <= to; i++, ++j)
            {
                if (chart1.Series.Count < j + 1)
                {
                    chart1.Series.Add(new Series("U" + i) { ChartType = SeriesChartType.Spline, BorderWidth = 3 });
                }

                chart1.Series[j].Points.AddXY(state.Time, state.MovementU[i]);
            }
        }
    }
}
