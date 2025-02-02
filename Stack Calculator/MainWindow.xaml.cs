using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.DirectoryServices.ActiveDirectory;
using System;

namespace Stack_Calculator
{
    public partial class MainWindow : Window
    {
        private double memory = 0;
        public const double Pi = Math.PI;
        public const double E = Math.E;
        public MainWindow()
        {
            InitializeComponent();
            BoxMain.Focus();

        }

        public string Calculate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "Error: Empty input";
            }
            try
            {
                string balancedExpression = BalanceParentheses(text);
                string evaluatedExpression = EvaluateParentheses(AddMultiplicationOperator(balancedExpression));
                evaluatedExpression = ReplaceConstants(evaluatedExpression);
                double finalAnswer = EvaluateExpression(evaluatedExpression);
                return Math.Round(finalAnswer, 4).ToString();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private string ReplaceConstants(string expression)
        {
            expression = expression.Replace("π", Pi.ToString(CultureInfo.InvariantCulture));
            expression = expression.Replace("e", E.ToString(CultureInfo.InvariantCulture));
            return expression;
        }
        private string BalanceParentheses(string expression)
        {
            int openParenthesesCount = expression.Count(c => c == '(');
            int closeParenthesesCount = expression.Count(c => c == ')');

            int parenthesesToAdd = openParenthesesCount - closeParenthesesCount;
            if (parenthesesToAdd > 0)
            {
                expression += new string(')', parenthesesToAdd);
            }
            return expression;
        }

        private double EvaluateExpression(string expression)
        {
            Stack<double> numberStack = new Stack<double>();
            Stack<char> operatorStack = new Stack<char>();

            for (int i = 0; i < expression.Length; i++)
            {
                if (char.IsWhiteSpace(expression[i]))
                {
                    continue;
                }

                if (char.IsDigit(expression[i]) || (expression[i] == '-' && (i == 0 || IsOperator(expression[i - 1]) || expression[i - 1] == '(')))
                {
                    StringBuilder numberBuilder = new StringBuilder();
                    numberBuilder.Append(expression[i]);
                    i++;
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        numberBuilder.Append(expression[i]);
                        i++;
                    }
                    i--;

                    double parsedNumber = double.Parse(numberBuilder.ToString(), CultureInfo.InvariantCulture);
                    numberStack.Push(parsedNumber);
                }
                else if (expression[i] == '!')
                {
                    if (numberStack.Count == 0)
                    {
                        throw new InvalidOperationException("Invalid expression: Factorial operator (!) must be preceded by a number.");
                    }

                    double lastNumber = numberStack.Pop();
                    double result = Factorial(lastNumber);
                    numberStack.Push(result);
                    if (i + 1 < expression.Length && expression[i + 1] == '-')
                    {
                        i++;
                        operatorStack.Push('-');
                    }
                }
                else if (expression[i] == '(')
                {
                    operatorStack.Push(expression[i]);
                }
                else if (expression[i] == ')')
                {
                    while (operatorStack.Peek() != '(')
                    {
                        numberStack.Push(ApplyOperation(operatorStack.Pop(), numberStack.Pop(), numberStack.Pop()));
                    }
                    operatorStack.Pop();
                }
                else if (IsOperator(expression[i]))
                {
                    while (operatorStack.Count > 0 && Precedence(operatorStack.Peek()) >= Precedence(expression[i]))
                    {
                        numberStack.Push(ApplyOperation(operatorStack.Pop(), numberStack.Pop(), numberStack.Pop()));
                    }
                    operatorStack.Push(expression[i]);
                }
            }

            while (operatorStack.Count > 0)
            {
                numberStack.Push(ApplyOperation(operatorStack.Pop(), numberStack.Pop(), numberStack.Pop()));
            }

            return numberStack.Pop();
        }
        private string AddMultiplicationOperator(string expression)
        {
            StringBuilder modifiedExpression = new StringBuilder();
            for (int i = 0; i < expression.Length; i++)
            {
                if (i > 0)
                {
                    if ((char.IsDigit(expression[i - 1]) || expression[i - 1] == ')' || expression[i - 1] == 'π' || expression[i - 1] == 'e' || expression[i-1] == '!') &&
                        (expression[i] == '(' || char.IsLetter(expression[i])))
                    {
                        modifiedExpression.Append('*');
                    }
                    if (i > 0 && (expression[i - 1] == 'e' || expression[i - 1] == 'π') && char.IsDigit(expression[i]))
                    {
                        modifiedExpression.Append('*');
                    }
                }
                modifiedExpression.Append(expression[i]);
            }
            return modifiedExpression.ToString();

        }


        private string EvaluateParentheses(string expression)
        {
            int openIndex = expression.LastIndexOf('(');
            while (openIndex >= 0)
            {
                int closeIndex = expression.IndexOf(')', openIndex);
                if (closeIndex == -1) break;

                string innerExpression = expression.Substring(openIndex + 1, closeIndex - openIndex - 1);
                double evaluated = EvaluateExpression(innerExpression);
                expression = expression.Substring(0, openIndex) + evaluated.ToString(CultureInfo.InvariantCulture) + expression.Substring(closeIndex + 1);

                openIndex = expression.LastIndexOf('(');
            }
            return expression;
        }

        private double ApplyOperation(char operation, double b, double a)
        {
            switch (operation)
            {
                case '+':
                    return a + b;
                case '-':
                    return a - b;
                case '*':
                    return a * b;
                case '/':
                    if (b == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    return a / b;
                case '!':
                    return Factorial(b);
                case '^':
                    return Math.Pow(a, b);
                case '√':
                    return Math.Pow(b, 1 / a);
                case '%':
                    return a * (b / 100.0);
                default:
                    throw new InvalidOperationException("Unsupported operation");
            }
        }

        private double Factorial(double n)
        {
            if (n < 0)
                throw new InvalidOperationException("Factorial is not defined for negative numbers.");
            if (n % 1 != 0)
                throw new InvalidOperationException("Factorial is not defined for non-integer values.");

            if (n == 0 || n == 1)
                return 1;

            double result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }


        private int Precedence(char op)
        {
            switch (op)
            {
                case '+':
                    return 1;
                case '-':
                    return 1;
                case '*':
                    return 2;
                case '/':
                    return 2;
                case '%':
                    return 2;
                case '!':
                    return 3;
                case '^':
                    return 3;
                case '√':
                    return 3;
                default:
                    return -1;
            }
        }

        private bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '!' || c == '^' || c == '√' || c == '%';
        }
        private void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(MemoryClear);
            memory = 0;
        }

        private void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(MemoryRecall);
            if(BoxMain.Text.Length == 0)
            {
                if (memory != 0)
                {
                    BoxMain.Text += memory.ToString();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (memory != 0 && (char.IsDigit(BoxMain.Text[BoxMain.Text.Length - 1]) || BoxMain.Text[BoxMain.Text.Length - 1] == ')' || BoxMain.Text.EndsWith("e") || BoxMain.Text.EndsWith("π")))
                {
                    if (memory >= 0)
                    {
                        BoxMain.Text += '+' + memory.ToString();
                    }
                    else
                    {
                        BoxMain.Text += '-' + memory.ToString();
                    }
                }
            }
        }

        private void MemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Result.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                FlashButtonDark(MemoryAdd);
                memory += result;
            }
        }

        private void MemorySubstract_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Result.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                FlashButtonDark(MemorySubstract);
                memory -= result;
            }
        }
        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Plus);
            ReplaceLastOperator('+');

        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Mul);
            ReplaceLastOperator('*');
        }

        private void Devi_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Dev);
            ReplaceLastOperator('/');
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Minus);
            ReplaceLastOperator('-');
        }



        private void NineClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Nine);
            BoxMain.Text += "9";
        }

        private void SixClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Six);
            BoxMain.Text += "6";
        }

        private void ThreeClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Three);
            BoxMain.Text += "3";
        }

        private void EightClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Eight);
            BoxMain.Text += "8";
        }

        private void TwoClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Two);
            BoxMain.Text += "2";
        }

        private void FiveClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Five);
            BoxMain.Text += "5";
        }

        private void OneClick(object sender, RoutedEventArgs e)
        {
            FlashButton(One);
            BoxMain.Text += "1";
        }

        private void FourClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Four);
            BoxMain.Text += "4";
        }

        private void SevenClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Seven);
            BoxMain.Text += "7";
        }

        private void ZeroClick(object sender, RoutedEventArgs e)
        {
            FlashButton(Zero);
            BoxMain.Text += "0";
        }

        private void DotClick(object sender, RoutedEventArgs e)
        {
            if (CanInsertDot())
            {
                FlashButton(Dot);
                BoxMain.Text += ".";
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Open);
            BoxMain.Text += "(";
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Close);
            int openParenthesesCount = BoxMain.Text.Count(c => c == '(');
            int closeParenthesesCount = BoxMain.Text.Count(c => c == ')');

            if (openParenthesesCount > closeParenthesesCount)
            {
                BoxMain.Text += ")";
            }
        }

        private void FactorialClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Factorial1);
            if (BoxMain.Text.Length > 0)
            {
                char lastChar = BoxMain.Text[BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || IsOperator(lastChar) || lastChar == ')')
                {
                    ReplaceLastOperator('!');
                }
            }
        }
        private void RootClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Root);
            if (BoxMain.Text.Length > 0)
            {
                char lastChar = BoxMain.Text[BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || IsOperator(lastChar) || lastChar == ')' || BoxMain.Text.EndsWith("e") || BoxMain.Text.EndsWith("π"))
                {
                    ReplaceLastOperator('√');
                }
            }
        }
        private void PercentClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Percent);
            if (BoxMain.Text.Length > 0)
            {
                char lastChar = BoxMain.Text[BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || IsOperator(lastChar) || lastChar == ')' || BoxMain.Text.EndsWith("e") || BoxMain.Text.EndsWith("π"))
                {
                    ReplaceLastOperator('%');
                }
            }
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(Clear);
            BoxMain.Clear();
            Result.Text = "";
        }
        private void ClearLastClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(DeleteLast);
            if (BoxMain.Text.Length > 0)
            {
                BoxMain.Text = BoxMain.Text.Remove(BoxMain.Text.Length - 1);
            }
        }
        private void PowerOfClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(PowerOf);
            if (BoxMain.Text.Length > 0)
            {
                char lastChar = BoxMain.Text[BoxMain.Text.Length - 1];
                if (char.IsDigit(lastChar) || IsOperator(lastChar) || lastChar == ')' || BoxMain.Text.EndsWith("e") || BoxMain.Text.EndsWith("π"))
                {
                    ReplaceLastOperator('^');
                }
            }
        }
        private void EClick(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(EButton);
            if (BoxMain.Text.EndsWith("π"))
            {
                BoxMain.Text = BoxMain.Text.Remove(BoxMain.Text.Length - 1);
            }
            if (BoxMain.Text.Length == 0 || !(BoxMain.Text.EndsWith("π") || BoxMain.Text.EndsWith("e")) && !BoxMain.Text.EndsWith("."))
            {
                BoxMain.Text += "e";
            }
        }

        private void PiClick(object sender, RoutedEventArgs e)
        {
            FlashButton(PiButton);
            if (BoxMain.Text.EndsWith("e"))
            {
                BoxMain.Text = BoxMain.Text.Remove(BoxMain.Text.Length - 1);
            }
            if (BoxMain.Text.Length == 0 || !(BoxMain.Text.EndsWith("π")) && !BoxMain.Text.EndsWith("."))
            {
                BoxMain.Text += "π";
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.OemPlus)
            {
                Equals_Click(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D0)
            {
                CloseClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D0)
            {
                ZeroClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D1)
            {
                FactorialClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D1)
            {
                OneClick(sender, e);
            }
            else if (e.Key == Key.D2)
            {
                TwoClick(sender, e);
            }
            else if (e.Key == Key.D3)
            {
                ThreeClick(sender, e);
            }
            else if (e.Key == Key.D4)
            {
                FourClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D5)
            {
                FiveClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D6)
            {
                SixClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D6)
            {
                PowerOfClick(sender, e);
            }
            else if (e.Key == Key.D7)
            {
                SevenClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D8)
            {
                Multiply_Click(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D8)
            {
                EightClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D9)
            {
                OpenClick(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.D9)
            {
                NineClick(sender, e);
            }
            else if (e.Key == Key.OemPeriod)
            {
                DotClick(sender, e);
            }
            else if (e.Key == Key.OemQuestion)
            {
                Devi_Click(sender, e);
            }
            else if (e.Key == Key.OemMinus)
            {
                Minus_Click(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.OemPlus)
            {
                Plus_Click(sender, e);
            }
            else if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key == Key.OemPlus)
            {
                Equals_Click(sender, e);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && e.Key == Key.OemCloseBrackets)
            {
                ClearClick(sender, e);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift && e.Key == Key.OemCloseBrackets)
            {
                ClearLastClick(sender, e);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D5)
            {
                PercentClick(sender, e);
            }
            else if (e.Key == Key.E)
            {
                EClick(sender, e);
            }
            else if (e.Key == Key.P)
            {
                PiClick(sender, e);
            }
        }
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            char lastChar = BoxMain.Text[BoxMain.Text.Length - 1];
            if (IsOperator(lastChar) && lastChar != '!')
            {
                BoxMain.Text = BoxMain.Text.Remove(BoxMain.Text.Length - 1);
            }
            Brush originalBrush = Equals.Background;
            Equals.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0x91, 0xD3));
            Task.Delay(TimeSpan.FromSeconds(0.05)).ContinueWith(task =>
            {
                Equals.Background = originalBrush;
            }, TaskScheduler.FromCurrentSynchronizationContext());
            Result.Text = Calculate(BoxMain.Text);
        }
        private void FlashButton(Button button)
        {
            Brush originalBrush = button.Background;
            button.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x2D, 0x35, 0x33));
            Task.Delay(TimeSpan.FromSeconds(0.05)).ContinueWith(task =>
            {
                button.Background = originalBrush;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void FlashButtonDark(Button button)
        {
            Brush originalBrush = button.Background;
            button.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x37, 0x3F, 0x3F));
            Task.Delay(TimeSpan.FromSeconds(0.05)).ContinueWith(task =>
            {
                button.Background = originalBrush;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ReplaceLastOperator(char newOperator)
        {
            if (BoxMain.Text.Length > 0)
            {
                char lastChar = BoxMain.Text[BoxMain.Text.Length - 1];
                if (lastChar == '-' && BoxMain.Text.Length == 1)
                {
                    return;
                }
                if (lastChar == '!' || lastChar == ')')
                {
                    BoxMain.Text += newOperator;
                    return;
                }
                if (IsOperator(lastChar) || lastChar == '.')
                {
                    BoxMain.Text = BoxMain.Text.Remove(BoxMain.Text.Length - 1) + newOperator;
                }
                else
                {
                    BoxMain.Text += newOperator;
                }
            }
            else
            {
                if (newOperator == '-')
                {
                    BoxMain.Text += newOperator;
                }
            }
        }
        private bool CanInsertDot()
        {
            if (BoxMain.Text.Length == 0 || !char.IsDigit(BoxMain.Text[BoxMain.Text.Length - 1]))
            {
                return false;
            }
            int lastOperatorIndex = FindLastOperatorIndex(BoxMain.Text);
            int lastDotIndex = BoxMain.Text.LastIndexOf('.');

            if (lastDotIndex > lastOperatorIndex)
            {
                return false;
            }

            return true;
        }

        private int FindLastOperatorIndex(string text)
        {
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (IsOperator(text[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private void Memory_Click(object sender, RoutedEventArgs e)
        {
            FlashButtonDark(MemoryShow);
            MessageBox.Show("Memory: " + memory.ToString(), "Memory Value", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    }