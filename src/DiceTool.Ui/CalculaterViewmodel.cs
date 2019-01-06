using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace Dice.Ui
{
    class CalculaterViewmodel : DependencyObject
    {

        public DirectoryInfo DataFolder { get; }
        public string Name => this.DataFolder.Name;



        public CalculaterViewmodel(DirectoryInfo dataFolder)
        {
            this.DataFolder = dataFolder;
            var codeFile = this.GetCodeFile();

            if (codeFile.Exists)
                this.Code = File.ReadAllText(codeFile.FullName, Encoding.UTF8);

            var resultFile = this.GetResultFile();
            if (resultFile.Exists)
                this.LoadResult(resultFile);



            this.calculateCommand = new Command(this);

        }


        public TimeSpan CalculationTime
        {
            get { return (TimeSpan)this.GetValue(CalculationTimeProperty); }
            set { this.SetValue(CalculationTimePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for CalculationTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey CalculationTimePropertyKey =
            DependencyProperty.RegisterReadOnly("CalculationTime", typeof(TimeSpan), typeof(CalculaterViewmodel), new PropertyMetadata(default(TimeSpan)));
        public static readonly DependencyProperty CalculationTimeProperty = CalculationTimePropertyKey.DependencyProperty;



        public string Code
        {
            get { return (string)this.GetValue(CodeProperty); }
            set { this.SetValue(CodeProperty, value); }
        }

        private const string Prefix = "ns";
        private const string Namespace = "http://DiceTool/Result";
        private const string PropabilityAttribute = "propability";
        private const string TimeAttribute = "time";

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(CalculaterViewmodel), new PropertyMetadata("", PropertyChagned));

        private static void PropertyChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as CalculaterViewmodel;
            var codeFile = me.GetCodeFile();
            File.WriteAllText(codeFile.FullName, (string)e.NewValue, Encoding.UTF8);
        }

        private FileInfo GetCodeFile()
        {
            var codeFile = new FileInfo(Path.Combine(this.DataFolder.FullName, "code"));
            return codeFile;
        }
        private FileInfo GetResultFile()
        {
            var codeFile = new FileInfo(Path.Combine(this.DataFolder.FullName, "result"));
            return codeFile;
        }

        public double Percentage
        {
            get { return (double)this.GetValue(PercentageProperty); }
            set { this.SetValue(PercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(CalculaterViewmodel), new PropertyMetadata(0.0));




        public bool IsReady
        {
            get { return (bool)this.GetValue(IsReadyProperty); }
            private set { this.SetValue(IsReadyPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for IsReady.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey IsReadyPropertyKey =
            DependencyProperty.RegisterReadOnly("IsReady", typeof(bool), typeof(CalculaterViewmodel), new PropertyMetadata(true));
        public static readonly DependencyProperty IsReadyProperty = IsReadyPropertyKey.DependencyProperty;




        public bool IsBuisy
        {
            get { return (bool)this.GetValue(IsBuisyProperty); }
            private set { this.SetValue(IsBuisyPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for IsBuisy.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey IsBuisyPropertyKey =
            DependencyProperty.RegisterReadOnly("IsBuisy", typeof(bool), typeof(CalculaterViewmodel), new PropertyMetadata(false, IsBusyChanging));

        private static void IsBusyChanging(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as CalculaterViewmodel;
            me.IsReady = !(bool)e.NewValue;
        }

        public static readonly DependencyProperty IsBuisyProperty = IsBuisyPropertyKey.DependencyProperty;





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


        private async Task StartCalculation()
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                this.IsBuisy = true;
                var returnType = Parser.SimpleParser.GetReturnType(this.Code);
                Task calculateTask;

                if (returnType == typeof(int))
                    calculateTask = Calculate<int>();
                else if (returnType == typeof(string))
                    calculateTask = Calculate<string>();
                else if (returnType == typeof(bool))
                    calculateTask = Calculate<bool>();
                else
                    throw new NotSupportedException($"Type {returnType} is not supported");

                var updateTask = UpdateTimer();

                await calculateTask;
                await updateTask;


                async Task UpdateTimer()
                {
                    while (this.IsBuisy)
                    {
                        await Task.Delay(200);
                        this.CalculationTime = stopWatch.Elapsed;
                    }
                }


                async Task Calculate<T>()
                {
                    try
                    {
                        this.calculateCommand.FireCanExecuteChange();
                        var executor = Dice.Parser.SimpleParser.ParseExpression<T>(this.Code);
                        this.Percentage = 0;
                        this.Results.Clear();
                        this.indexLookup.Clear();
                        executor.Epsylon = 0.0001;
                        var cal = executor.Calculate(0);
                        await foreach (var t in cal)
                        {
                            this.Percentage = t.CompletePercentage;
                            this.AddResult(t.Result, t.Propability * 100);
                        }

                        this.Percentage = 1;
                        await this.PersistResult();

                    }
                    finally
                    {
                        stopWatch.Stop();
                    }
                }

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

        private async Task PersistResult()
        {
            using (var stream = this.GetResultFile().Open(FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = System.Xml.XmlWriter.Create(stream, new System.Xml.XmlWriterSettings()
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                NamespaceHandling = System.Xml.NamespaceHandling.OmitDuplicates,
                CheckCharacters = true,
                ConformanceLevel = System.Xml.ConformanceLevel.Document
            }))
            {
                await writer.WriteStartDocumentAsync();

                await writer.WriteStartElementAsync(Prefix, "Results", Namespace);
                await writer.WriteAttributeStringAsync(Prefix, TimeAttribute, Namespace, this.CalculationTime.ToString("c", System.Globalization.CultureInfo.InvariantCulture));
                foreach (var item in this.Results)
                {
                    await writer.WriteStartElementAsync(Prefix, "Result", Namespace);
                    var value = item.Value?.ToString() ?? "<NULL>";
                    var propability = item.Propability;
                    await writer.WriteAttributeStringAsync(Prefix, PropabilityAttribute, Namespace, propability.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    await writer.WriteStringAsync(value);
                    await writer.WriteEndElementAsync();
                }
                await writer.WriteEndDocumentAsync();
            }
        }

        private void LoadResult(FileInfo resultFile)
        {
            using (var stream = resultFile.OpenRead())
            {
                var doc = XDocument.Load(stream);
                var results = doc.Root;
                this.CalculationTime = TimeSpan.Parse(results.Attribute(XName.Get(TimeAttribute, Namespace)).Value, System.Globalization.CultureInfo.InvariantCulture);

                foreach (var item in results.Nodes().OfType<XElement>())
                {
                    var propability = double.Parse(item.Attribute(XName.Get(PropabilityAttribute, Namespace)).Value, System.Globalization.CultureInfo.InvariantCulture);
                    var value = item.Value;
                    this.Results.Add(new ResultViewmodel() { Value = value, Propability = propability });
                }

            }

        }


        public ICommand CalculateCommand => this.calculateCommand;


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
