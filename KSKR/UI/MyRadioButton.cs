using System.Windows.Forms;

namespace UI
{
    public class MyRadioButton: RadioButton
    {
        public MyRadioButton(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}
