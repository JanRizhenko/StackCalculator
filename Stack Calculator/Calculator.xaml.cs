using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.DirectoryServices.ActiveDirectory;
using System;
using Stack_Calculator.Interactions;

namespace Stack_Calculator
{
    public partial class Calculator : Window
    {
        private Keys keyHandler;
        private Clicks _clicks;
        public double memory = 0;
        public const double Pi = Math.PI;
        public const double E = Math.E;
        public Calculator()
        {
            InitializeComponent();
            _clicks = new Clicks(this);
            keyHandler = new Keys(this, _clicks);
            BoxMain.Focus();
            this.KeyDown += keyHandler.OnKeyDown;

            Plus.Click += _clicks.Plus_Click;
            Minus.Click += _clicks.Minus_Click;
            Mul.Click += _clicks.Multiply_Click;
            Dev.Click += _clicks.Devi_Click;
            Equals.Click += _clicks.Equals_Click;

            Three.Click += _clicks.ThreeClick;
            Six.Click += _clicks.SixClick;
            Nine.Click += _clicks.NineClick;
            Eight.Click += _clicks.EightClick;
            Five.Click += _clicks.FiveClick;
            Two.Click += _clicks.TwoClick;
            Seven.Click += _clicks.SevenClick;
            Four.Click += _clicks.FourClick;
            One.Click += _clicks.OneClick;
            Zero.Click += _clicks.ZeroClick;
            Dot.Click += _clicks.DotClick;

            Open.Click += _clicks.OpenClick;
            Close.Click += _clicks.CloseClick;
            Factorial1.Click += _clicks.FactorialClick;
            Clear.Click += _clicks.ClearClick;
            DeleteLast.Click += _clicks.ClearLastClick;

            PowerOf.Click += _clicks.PowerOfClick;
            Root.Click += _clicks.RootClick;
            Percent.Click += _clicks.PercentClick;
            EButton.Click += _clicks.EClick;
            PiButton.Click += _clicks.PiClick;

            MemoryAdd.Click += _clicks.MemoryAdd_Click;
            MemorySubstract.Click += _clicks.MemorySubstract_Click;
            MemoryRecall.Click += _clicks.MemoryRecall_Click;
            MemoryClear.Click += _clicks.MemoryClear_Click;
            MemoryShow.Click += _clicks.Memory_Click;
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
                string expressionWithConstants = ReplaceConstants(balancedExpression);
                string evaluatedExpression = EvaluateParentheses(AddMultiplicationOperator(expressionWithConstants));
                double finalAnswer = EvaluateExpression(evaluatedExpression);
                return Math.Round(finalAnswer, 4).ToString();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        public string ReplaceConstants(string expression)
        {
            expression = expression.Replace("π", Pi.ToString(CultureInfo.InvariantCulture));
            expression = expression.Replace("e", E.ToString(CultureInfo.InvariantCulture));
            return expression;
        }
        public string BalanceParentheses(string expression)
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

        public double EvaluateExpression(string expression)
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
        public string AddMultiplicationOperator(string expression)
        {
            StringBuilder modifiedExpression = new StringBuilder();
            for (int i = 0; i < expression.Length; i++)
            {
                if (i > 0)
                {
                    if ((char.IsDigit(expression[i - 1]) || expression[i - 1] == ')' || expression[i - 1] == 'π' || expression[i - 1] == 'e' || expression[i - 1] == '!') &&
                        (expression[i] == '(' || (char.IsLetter(expression[i]) && expression[i - 1] != '(')))
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


        public string EvaluateParentheses(string expression)
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

        public double ApplyOperation(char operation, double b, double a)
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

        public double Factorial(double n)
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


        public int Precedence(char op)
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

        public bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '!' || c == '^' || c == '√' || c == '%';
        }
    }
    }