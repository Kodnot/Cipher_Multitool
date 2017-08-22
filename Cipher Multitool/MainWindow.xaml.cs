using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cipher_Multitool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Set the initial input analysis results
            UpdateInputTextAnalysis();
        }

        private void SwitchButtonClick(object sender, RoutedEventArgs e)
        {
            inputTextBox.Text = outputTextBox.Text;
            outputTextBox.Text = "";
        }

        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            if (cipherOptionsList.SelectedItem == null)
            {
                return;
            }

            string inputText = inputTextBox.Text; // .Text returns a copy of the contents
            if (stripNonAlphanumericSymoblsCheckBox.IsChecked ?? false)
            {
                inputText = Regex.Replace(inputText, "[^a-zA-Z]+", string.Empty, RegexOptions.Compiled);
            }
            if (capitalizeChechBox.IsChecked ?? false)
            {
                inputText = inputText.ToUpper();
            }

            switch((cipherOptionsList.SelectedItem as ListBoxItem).Content.ToString())
            {
                case "Binary Encode":
                    outputTextBox.Text = CipherMethods.StringToBinary(inputText);
                    break;
                case "Binary Decode":
                    outputTextBox.Text = CipherMethods.BinaryToString(inputText);
                    break;
                case "Caesar Shift Encode":
                {
                    if (!int.TryParse(parametersTextBox.Text, out int shiftAmount))
                        return;
                        outputTextBox.Text = CipherMethods.CaesarShift(inputText, shiftAmount);
                    break;
                }
                case "Caesar Shift Decode":
                {
                    if (!int.TryParse(parametersTextBox.Text, out int shiftAmount))
                        return;
                        outputTextBox.Text = CipherMethods.CaesarShift(inputText, -shiftAmount);
                    break;
                }
                case "Vigenere Encode":
                    if (parametersTextBox.Text == "")
                        return;
                    outputTextBox.Text = CipherMethods.VigenereEncode(inputText, parametersTextBox.Text);
                    break;
                case "Vigenere Decode":
                    if (parametersTextBox.Text == "")
                        return;
                    outputTextBox.Text = CipherMethods.VigenereDecode(inputText, parametersTextBox.Text);
                    break;
                case "Substitution Encode":
                    if (parametersTextBox.Text == "")
                        return;
                    outputTextBox.Text = CipherMethods.SimpleSubstitutionEncode(inputText, parametersTextBox.Text);
                    break;
                case "Substitution Decode":
                    if (parametersTextBox.Text == "")
                        return;
                    outputTextBox.Text = CipherMethods.SimpleSubstitutionDecode(inputText, parametersTextBox.Text);
                    break;
                default:
                    outputTextBox.Text = "Something went wrong";
                    break;
            }
        }

        private void CipherOptionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((cipherOptionsList.SelectedItem as ListBoxItem).Content.ToString())
            {
                case "Binary Encode":
                case "Binary Decode":
                    parametersTextBoxLabel.Content = "No additional parameters.";
                    break;
                case "Caesar Shift Encode":
                case "Caesar Shift Decode":
                    parametersTextBoxLabel.Content = "Enter shift amount:";
                    break;
                case "Vigenere Encode":
                case "Vigenere Decode":
                case "Substitution Encode":
                case "Substitution Decode":
                    parametersTextBoxLabel.Content = "Enter key word:";
                    break;
                default:
                    outputTextBox.Text = "Something went wrong";
                    break;
            }
        }

        private void UpdateInputTextAnalysis()
        {
            string inputText = inputTextBox.Text;
            int textLength = inputText.Length;
            Analysis textAnal = new Analysis(inputText);

            bool ignoreCase = ignoreCaseCheckBox.IsChecked ?? false; // Should never be null
            bool displayNumbers = displayNumbersCheckBox.IsChecked ?? false; // Whether to display the count of numbers
            bool displayPunctuation = displayPunctuationCheckBox.IsChecked ?? false;

            int totalSymbolCount = textAnal.CountSymbols();
            int totalLetterCount = textAnal.CountLetters();
            int totalUppercaseCount = textAnal.CountUppercase();
            int totalLowercaseCount = textAnal.CountLowercase();
            int totalNumberCount = textAnal.CountNumbers();

            string rezSymbolCounts = "";
            if (ignoreCase)
            {
                var letterFrequences = textAnal.FreqLetter();

                foreach (var pair in letterFrequences)
                {
                    rezSymbolCounts += $"'{pair.Key.ToString()}': {pair.Value} /" +
                                        $" {Math.Round(pair.Value / (double)textLength * 100D, 2)}%;\n";
                }
            }
            else
            {
                var uppercaseFrequences = textAnal.FreqUppercase();
                var lowercaseFrequences = textAnal.FreqLowercase();
                foreach (var pair in uppercaseFrequences)
                {
                    rezSymbolCounts += $"'{pair.Key.ToString()}': {pair.Value} /" +
                                        $" {Math.Round(pair.Value / (double)textLength * 100D, 2)}%;\n";
                }
                foreach (var pair in lowercaseFrequences)
                {
                    rezSymbolCounts += $"'{pair.Key.ToString()}': {pair.Value} /" +
                                        $" {Math.Round(pair.Value / (double)textLength * 100D, 2)}%;\n";
                }
            }

            if (displayNumbers)
            {
                var numberFrequences = textAnal.FreqNumbers();
                foreach (var pair in numberFrequences)
                {
                    rezSymbolCounts += $"'{pair.Key.ToString()}': {pair.Value} /" +
                                        $" {Math.Round(pair.Value / (double)textLength * 100D, 2)}%;\n";
                }
            }

            if (displayPunctuation)
            {
                var symbolFrequences = textAnal.FreqSymbols();
                foreach (var p in symbolFrequences)
                {
                    rezSymbolCounts +=  $"'{(p.Key == ' ' ? "space" : p.Key == '\n' ? "newline" : p.Key == '\r' ? "carriagereturn" : p.Key.ToString())}':" +
                                        $" {p.Value} / {Math.Round(p.Value / (double)textLength * 100D, 2)}%;\n";
                }
            }


            var indexOfCoincidence = textAnal.FindPeriodicIoC(inputText.Length < 20 ? inputText.Length : 20);
            string indexOfCoincidenceRez = "";
            for (int i = 0; i < indexOfCoincidence.Count; ++i)
            {
                indexOfCoincidenceRez += $"{indexOfCoincidence[i].Key + 1} : {indexOfCoincidence[i].Value}\n";
            }

            inputTextLengthLabel.Content = textLength;
            inputTextSpecificTotalCountsTextBlock.Text = $"letters: {totalLetterCount},\nuppercase: {totalUppercaseCount},\nlowercase: {totalLowercaseCount},\nsymbols: {totalSymbolCount}";
            inputTextSymbolTextBlock.Text = rezSymbolCounts;
            inputTextIoCTextBlock.Text = indexOfCoincidenceRez;
        }

        #region CheckBox event handlers

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) // This function is also called on window initialization (probably)
                return;

            UpdateInputTextAnalysis();
        }

        private void IgnoreCaseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdateInputTextAnalysis();
        }

        private void IgnoreCaseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdateInputTextAnalysis();
        }

        private void DisplayNumbersCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdateInputTextAnalysis();
        }

        private void DisplayNumbersCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdateInputTextAnalysis();
        }

        private void DisplayPunctuationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdateInputTextAnalysis();
        }

        private void DisplayPunctuationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdateInputTextAnalysis();
        }

        #endregion
    }
}
