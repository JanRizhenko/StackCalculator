using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stack_Calculator;

namespace Stack_Calculator.Interactions
{
    public class Keys
    {
        private Calculator _calculator;
        private Clicks _clicks;
        public Keys(Calculator calculator, Clicks clicks) 
        {

            _clicks = clicks;
            _calculator = calculator;   
        }
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.OemPlus)
            {
                _clicks.Equals_Click(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D0)
            {
                _clicks.CloseClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D0)
            {
                _clicks.ZeroClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D1)
            {
                _clicks.FactorialClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D1)
            {
                _clicks.OneClick(sender, e);
            }
            else if (e.Key == Key.D2)
            {
                _clicks.TwoClick(sender, e);
            }
            else if (e.Key == Key.D3)
            {
                _clicks.ThreeClick(sender, e);
            }
            else if (e.Key == Key.D4)
            {
                _clicks.FourClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D5)
            {
                _clicks.FiveClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D6)
            {
                _clicks.SixClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D6)
            {
                _clicks.PowerOfClick(sender, e);
            }
            else if (e.Key == Key.D7)
            {
                _clicks.SevenClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D8)
            {
                _clicks.Multiply_Click(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D8)
            {
                _clicks.EightClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D9)
            {
                _clicks.OpenClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D9)
            {
                _clicks.NineClick(sender, e);
            }
            else if (e.Key == Key.OemPeriod)
            {
                _clicks.DotClick(sender, e);
            }
            else if (e.Key == Key.OemQuestion)
            {
                _clicks.Devi_Click(sender, e);
            }
            else if (e.Key == Key.OemMinus)
            {
                _clicks.Minus_Click(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.OemPlus)
            {
                _clicks.Plus_Click(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.OemPlus)
            {
                _clicks.Equals_Click(sender, e);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && e.Key == Key.OemCloseBrackets)
            {
                _clicks.ClearClick(sender, e);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift && e.Key == Key.OemCloseBrackets)
            {
                _clicks.ClearLastClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D5)
            {
                _clicks.PercentClick(sender, e);
            }
            else if (e.Key == Key.E)
            {
                _clicks.EClick(sender, e);
            }
            else if (e.Key == Key.P)
            {
                _clicks.PiClick(sender, e);
            }
        }
    }
}
