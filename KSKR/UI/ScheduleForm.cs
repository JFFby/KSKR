using System.Collections.Generic;
using System.Windows.Forms;
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
                chart1.Series[0].Points.AddXY(state.Time, state.MovementU[0]);
                chart1.Series[1].Points.AddXY(state.Time, state.MovementU[1]);
            }
        }
    }
}
