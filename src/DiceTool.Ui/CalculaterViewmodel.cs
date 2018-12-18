using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dice.Ui
{
    class CalculaterViewmodel : DependencyObject
    {



        public string Code
        {
            get { return (string)this.GetValue(CodeProperty); }
            set { this.SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(CalculaterViewmodel), new PropertyMetadata(""));



        public double Percentage
        {
            get { return (double)this.GetValue(PercentageProperty); }
            set { this.SetValue(PercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(CalculaterViewmodel), new PropertyMetadata(0.0));



        public bool IsBuisy
        {
            get { return (bool)this.GetValue(IsBuisyProperty); }
            private set { this.SetValue(IsBuisyPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for IsBuisy.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey IsBuisyPropertyKey =
            DependencyProperty.RegisterReadOnly("IsBuisy", typeof(bool), typeof(CalculaterViewmodel), new PropertyMetadata(false));
        public static readonly DependencyProperty IsBuisyProperty = IsBuisyPropertyKey.DependencyProperty;



        public ReturnType ReturnType
        {
            get { return (ReturnType)this.GetValue(ReturnTypeProperty); }
            set { this.SetValue(ReturnTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReturnType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReturnTypeProperty =
            DependencyProperty.Register("ReturnType", typeof(ReturnType), typeof(CalculaterViewmodel), new PropertyMetadata(ReturnType.Integer));


        public ObservableCollection<ResultViewmodel> Results { get; } = new ObservableCollection<ResultViewmodel>();

        private readonly Dictionary<object, int> indexLookup = new Dictionary<object, int>();
        private readonly Command calculateCommand;

        private void AddResult(object result, double propability)
        {
            int index;
            if (this.indexLookup.ContainsKey(result))
                index = this.indexLookup[result];
            else
            {
                index = this.Results.Count;
                this.indexLookup.Add(result, index);
                this.Results.Add(new ResultViewmodel() { Value = result });
            }

            var vm = this.Results[index];
            vm.Propability += propability;
        }


        public async Task StartCalculation()
        {
            try
            {
                this.IsBuisy = true;
                this.calculateCommand.FireCanExecuteChange();
                var executor = Dice.Parser.SimpleParser.ParseExpression<int>(this.Code);
                this.Percentage = 0;
                this.Results.Clear();
                indexLookup.Clear();

                var cal = executor.Calculate(0);
                await foreach (var t in cal)
                {
                    this.Percentage = t.CompletePercentage;
                    this.AddResult(t.Result, t.Propability*100);
                }

                this.Percentage = 1;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.IsBuisy = false;
                this.calculateCommand.FireCanExecuteChange();
            }
        }

        public ICommand CalculateCommand => this.calculateCommand;
        public CalculaterViewmodel()
        {
            this.calculateCommand = new Command(this);
        }

        private class Command : ICommand
        {
            private CalculaterViewmodel calculaterViewmodel;

            public Command(CalculaterViewmodel calculaterViewmodel)
            {
                this.calculaterViewmodel = calculaterViewmodel;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return !this.calculaterViewmodel.IsBuisy;
            }

            public async void Execute(object parameter)
            {

                await this.calculaterViewmodel.StartCalculation();


            }

            internal void FireCanExecuteChange()
            {
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public enum ReturnType
    {
        Integer,
        String,
        Boolean
    }

    public class ResultViewmodel : DependencyObject
    {



        public double Propability
        {
            get { return (double)this.GetValue(PropabilityProperty); }
            set { this.SetValue(PropabilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Propability.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropabilityProperty =
            DependencyProperty.Register("Propability", typeof(double), typeof(ResultViewmodel), new PropertyMetadata(0.0));



        public object Value
        {
            get { return (object)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ResultViewmodel), new PropertyMetadata(null));



    }

}
