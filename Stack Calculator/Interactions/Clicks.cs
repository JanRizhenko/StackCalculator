using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Stack_Calculator;

namespace Stack_Calculator.Interactions
{
    public class Clicks
    {
        private Calculator _calculator;

        public Clicks(Calculator calculator)
        {
            _calculator = calculator;
        }
    
        public void Memory_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.MemoryShow);
            MessageBox.Show("Memory: " + _calculator.memory.ToString(), "Memory Value", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.MemoryClear);
            _calculator.memory = 0;
        }

        public void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.MemoryRecall);
            if (_calculator.BoxMain.Text.Length == 0)
            {
                if (_calculator.memory != 0)
                {
                    _calculator.BoxMain.Text += _calculator.memory.ToString();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (_calculator.memory != 0 && (char.IsDigit(_calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1]) || _calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1] == ')' || _calculator.BoxMain.Text.EndsWith("e") || _calculator.BoxMain.Text.EndsWith("π")))
                {
                    if (_calculator.memory >= 0)
                    {
                        _calculator.BoxMain.Text += '+' + _calculator.memory.ToString();
                    }
                    else
                    {
                        _calculator.BoxMain.Text += '-' + _calculator.memory.ToString();
                    }
                }
            }
        }

        public void MemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(_calculator.Result.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                FlashButtonDark(_calculator.MemoryAdd);
                _calculator.memory += result;
            }
        }

        public void MemorySubstract_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(_calculator.Result.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                FlashButtonDark(_calculator.MemorySubstract);
                _calculator.memory -= result;
            }
        }
        public void Plus_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Plus);
            ReplaceLastOperator('+');

        }

        public void Multiply_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Mul);
            ReplaceLastOperator('*');
        }

        public void Devi_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Dev);
            ReplaceLastOperator('/');
        }

        public void Minus_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Minus);
            ReplaceLastOperator('-');
        }



        public void NineClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Nine);
            _calculator.BoxMain.Text += "9";
        }

        public void SixClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Six);
            _calculator.BoxMain.Text += "6";
        }

        public void ThreeClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Three);
            _calculator.BoxMain.Text += "3";
        }

        public void EightClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Eight);
            _calculator.BoxMain.Text += "8";
        }

        public void TwoClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Two);
            _calculator.BoxMain.Text += "2";
        }

        public void FiveClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Five);
            _calculator.BoxMain.Text += "5";
        }

        public void OneClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.One);
            _calculator.BoxMain.Text += "1";
        }

        public void FourClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Four);
            _calculator.BoxMain.Text += "4";
        }

        public void SevenClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Seven);
            _calculator.BoxMain.Text += "7";
        }

        public void ZeroClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.Zero);
            _calculator.BoxMain.Text += "0";
        }

        public void DotClick(object sender, RoutedEventArgs e)
        {
            if (CanInsertDot())
            {
                FlashButton(_calculator.Dot);
                _calculator.BoxMain.Text += ".";
            }
        }

        public void OpenClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Open);
            _calculator.BoxMain.Text += "(";
        }

        public void CloseClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Close);
            int openParenthesesCount = _calculator.BoxMain.Text.Count(c => c == '(');
            int closeParenthesesCount = _calculator.BoxMain.Text.Count(c => c == ')');

            if (openParenthesesCount > closeParenthesesCount)
            {
                _calculator.BoxMain.Text += ")";
            }
        }

        public void FactorialClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Factorial1);
            if (_calculator.BoxMain.Text.Length > 0)
            {
                char lastChar = _calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || _calculator.IsOperator(lastChar) || lastChar == ')')
                {
                    ReplaceLastOperator('!');
                }
            }
        }
        public void RootClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Root);
            if (_calculator.BoxMain.Text.Length > 0)
            {
                char lastChar = _calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || _calculator.IsOperator(lastChar) || lastChar == ')' || _calculator.BoxMain.Text.EndsWith("e") || _calculator.BoxMain.Text.EndsWith("π"))
                {
                    ReplaceLastOperator('√');
                }
            }
        }
        public void PercentClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Percent);
            if (_calculator.BoxMain.Text.Length > 0)
            {
                char lastChar = _calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || _calculator.IsOperator(lastChar) || lastChar == ')' || _calculator.BoxMain.Text.EndsWith("e") || _calculator.BoxMain.Text.EndsWith("π"))
                {
                    ReplaceLastOperator('%');
                }
            }
        }

        public void ClearClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.Clear);
            _calculator.BoxMain.Clear();
            _calculator.Result.Text = "";
        }
        public void ClearLastClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.DeleteLast);
            if (_calculator.BoxMain.Text.Length > 0)
            {
                _calculator.BoxMain.Text = _calculator.BoxMain.Text.Remove(_calculator.BoxMain.Text.Length - 1);
            }
        }
        public void PowerOfClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.PowerOf);
            if (_calculator.BoxMain.Text.Length > 0)
            {
                char lastChar = _calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || _calculator.IsOperator(lastChar) || lastChar == ')' || _calculator.BoxMain.Text.EndsWith("e") || _calculator.BoxMain.Text.EndsWith("π"))
                {
                    ReplaceLastOperator('^');
                }
            }
        }
        public void EClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(_calculator.EButton);
            if (_calculator.BoxMain.Text.EndsWith("π"))
            {
                _calculator.BoxMain.Text = _calculator.BoxMain.Text.Remove(_calculator.BoxMain.Text.Length - 1);
            }
            if (_calculator.BoxMain.Text.Length == 0 || !(_calculator.BoxMain.Text.EndsWith("π") || _calculator.BoxMain.Text.EndsWith("e")) && !_calculator.BoxMain.Text.EndsWith("."))
            {
                _calculator.BoxMain.Text += "e";
            }
        }

        public void PiClick(object sender, RoutedEventArgs e)
        {
            FlashButton(_calculator.PiButton);
            if (_calculator.BoxMain.Text.EndsWith("e"))
            {
                _calculator.BoxMain.Text = _calculator.BoxMain.Text.Remove(_calculator.BoxMain.Text.Length - 1);
            }
            if (_calculator.BoxMain.Text.Length == 0 || !(_calculator.BoxMain.Text.EndsWith("π")) && !_calculator.BoxMain.Text.EndsWith("."))
            {
                _calculator.BoxMain.Text += "π";
            }
        }
        public void Equals_Click(object sender, RoutedEventArgs e)
        {
            char lastChar = _calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1];
            if (_calculator.IsOperator(lastChar) && lastChar != '!')
            {
                _calculator.BoxMain.Text = _calculator.BoxMain.Text.Remove(_calculator.BoxMain.Text.Length - 1);
            }
            Brush originalBrush = _calculator.Equals.Background;
            _calculator.Equals.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0x91, 0xD3));
            Task.Delay(TimeSpan.FromSeconds(0.05)).ContinueWith(task =>
            {
                _calculator.Equals.Background = originalBrush;
            }, TaskScheduler.FromCurrentSynchronizationContext());
            _calculator.Result.Text = _calculator.Calculate(_calculator.BoxMain.Text);
        }
        public void FlashButton(Button button)
        {
            Brush originalBrush = button.Background;
            button.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x2D, 0x35, 0x33));
            Task.Delay(TimeSpan.FromSeconds(0.05)).ContinueWith(task =>
            {
                button.Background = originalBrush;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void FlashButtonDark(Button button)
        {
            Brush originalBrush = button.Background;
            button.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x37, 0x3F, 0x3F));
            Task.Delay(TimeSpan.FromSeconds(0.05)).ContinueWith(task =>
            {
                button.Background = originalBrush;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void ReplaceLastOperator(char newOperator)
        {
            if (_calculator.BoxMain.Text.Length > 0)
            {
                char lastChar = _calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1];
                if (lastChar == '-' && _calculator.BoxMain.Text.Length == 1)
                {
                    return;
                }
                if (lastChar == '!' || lastChar == ')')
                {
                    _calculator.BoxMain.Text += newOperator;
                    return;
                }
                if (_calculator.IsOperator(lastChar) || lastChar == '.' || lastChar == '(')
                {
                    _calculator.BoxMain.Text = _calculator.BoxMain.Text.Remove(_calculator.BoxMain.Text.Length - 1) + newOperator;
                }
                else
                {
                    _calculator.BoxMain.Text += newOperator;
                }
            }
            else
            {
                if (newOperator == '-')
                {
                    _calculator.BoxMain.Text += newOperator;
                }
            }
        }
        public bool CanInsertDot()
        {
            if (_calculator.BoxMain.Text.Length == 0 || !char.IsDigit(_calculator.BoxMain.Text[_calculator.BoxMain.Text.Length - 1]))
            {
                return false;
            }
            int lastOperatorIndex = FindLastOperatorIndex(_calculator.BoxMain.Text);
            int lastDotIndex = _calculator.BoxMain.Text.LastIndexOf('.');

            if (lastDotIndex > lastOperatorIndex)
            {
                return false;
            }

            return true;
        }

        public int FindLastOperatorIndex(string text)
        {
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (_calculator.IsOperator(text[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
