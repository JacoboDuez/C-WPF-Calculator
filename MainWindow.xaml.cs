using System.CodeDom;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Schema;



namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            tbResult.Text = "";
            tbCalculatorHistoric.Text = "0";

        }

        // I initialize the global variables
        long numberOfClicks = 0;
        float currentCalculation = 0;
        string priorEntry = "";
        int currentPosition = 0;
        string resultInString, currentString;
        double priorResult;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = sender as Button;
            bool bOperator = false;
            bool bFunction = false;
            string sValueAfterParenthesis = "";
            bool bBypass = false;
            bool bEvaluate = false;
            bool bDelete = false;

            string buttonContent = ClickedButton.Content.ToString();



            switch (buttonContent.ToUpper())
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "0":
                case ".":
                    break;
                case "+":
                case "-":
                case "*":
                case "/":
                    bOperator = true;
                    break;
                case "SIN":
                case "COS":
                case "TAN":
                case "COSH":
                case "SINH":
                case "TANH":
                    bFunction = true;
                    break;
                case "=":
                    bEvaluate = true;
                    break;
                case "X":case "AC":
                    bDelete = true;
                    break;
                default:
                    break;

            }

            string currentHistoric = tbCalculatorHistoric.Text;
            string currentResult = tbResult.Text;

            if (bDelete)
            {
                tbResult.Text = "";
                tbCalculatorHistoric.Text = "";
                numberOfClicks = 0;
            }
            else { 
                if (numberOfClicks == 0)
                {
                    tbCalculatorHistoric.Text = buttonContent.ToString();
                    if (bFunction)
                    {
                        currentPosition = tbCalculatorHistoric.Text.Length - 1;
                        priorEntry = "Function";
                        tbCalculatorHistoric.Text = buttonContent.ToString() + "()";

                    }
                }
                else
                {
                    if (bFunction)
                    {
                        var textToFind = buttonContent.ToString();
                        var functionPos = tbCalculatorHistoric.Text.LastIndexOf(textToFind);
                        tbCalculatorHistoric.Text = textToFind + '(' + tbCalculatorHistoric.Text + ')';
                    }
                    if (priorEntry == "Function" && currentPosition > 0)
                    {
                        if (!bEvaluate)
                        {
                            // safer, clearer insert-before-currentPosition (keeps the closing ')' intact)
                            var pos = currentPosition;//Math.Clamp(currentPosition, 0, currentHistoric.Length - 1);
                            tbCalculatorHistoric.Text = currentHistoric.Insert(pos + 2, buttonContent);
                            currentPosition = tbCalculatorHistoric.Text.Length - 1;
                            currentString += buttonContent.ToString();
                            tbResult.Text = EvaluateExpression(tbCalculatorHistoric.Text);
                        }
                        else
                        {

                            tbResult.Text = "Math.Error";
                        }
                    }
                    else
                    {
                        if (!bEvaluate)
                        {
                            if (currentPosition > 0)
                            {
                                tbCalculatorHistoric.Text = currentHistoric.Substring(0, currentPosition) + buttonContent.ToString() + currentHistoric.Substring(currentPosition);
                            }
                            else
                            {
                                if (!bFunction) { 
                                tbCalculatorHistoric.Text += buttonContent.ToString();
                                }
                            }
                        }
                        else
                        {
                             try
                                {
                                     resultInString = EvaluateExpression(tbCalculatorHistoric.Text);
                                     tbResult.Text = resultInString;
                                }
                                catch
                                {
                                    Console.Error.WriteLine("Math Error");
                                    tbResult.Text = "Math Error!";
                                }
                        }

                        
                    }

                }
            }


            numberOfClicks += 1;
            //In case more than 0 clicks have been entered then I will enable the X for the user to delete his actions
            if (numberOfClicks == 1)
            {
                btnClear.Content = "X";
            }
        }


        private static string EvaluateExpression2(string expression, double priorResult)
        {
            bool bEvaluate = true;
            bool bBypass = false;
            double currResult;
            string currResultAsString = "";
           for(int i=expression.Length;i<=0;i--){
                switch (expression[i])
                {
                    case '+': case '-': case '*': case '/':

                        bEvaluate = true;
                        break;
                    case ')':
                        bBypass = true;
                        break;
                    default:
                        break;
                }
                if (!bEvaluate && bBypass)
                {
                    currResultAsString += expression[i].ToString();
                }
                else
                {
                    if(bEvaluate && bBypass)
                    {
                        currResultAsString += expression[i].ToString();
                        currResult = Double.Parse(currResultAsString);
                    }
                    else
                    {
                        if (bBypass)
                        {
                            
                        }
                    }
                }

            }
            return currResultAsString;

        }

        private static string EvaluateExpression(string expression)
        {


            Debug.Print(expression);

            try
            {
                var table = new DataTable();
                var result = table.Compute(expression, string.Empty);
                var sResult =  result.ToString();
                return sResult;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid mathematical expression.", ex);
            }


        }
           
        

    }
}