using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace Dice.Ui
{
    class CalculaterViewmodel : DependencyObject
    {

        public DirectoryInfo DataFolder { get; }
        public string Name => this.DataFolder.Name;

        public IHighlightingDefinition HighlightingDefinition => HighlightingManager.Instance.GetDefinition("Dice Language");


        static CalculaterViewmodel()
        {
            using var stream = typeof(CalculaterViewmodel).Assembly.GetManifestResourceStream("Dice.Ui.dl.xshd");
            if (stream is not null)
            {
                using var reader = XmlReader.Create(stream);
                var definition = HighlightingLoader.LoadXshd(reader);
                var highlightDefinition = HighlightingLoader.Load(definition, HighlightingManager.Instance);
                HighlightingManager.Instance.RegisterHighlighting("Dice Language", new[] { ".dl" }, highlightDefinition);
            }
        }


        public CalculaterViewmodel(DirectoryInfo dataFolder)
        {
            this.DataFolder = dataFolder;
            var codeFile = this.GetCodeFile();

            if (codeFile.Exists)
                this.Code = new TextDocument(File.ReadAllText(codeFile.FullName, Encoding.UTF8));
            else
                this.Code = new TextDocument();
            this.Code.Changed += async (sender, e) =>
            {
                var codeFile = this.GetCodeFile();
                await File.WriteAllTextAsync(codeFile.FullName, this.Code.Text, Encoding.UTF8);
            };


            var resultFile = this.GetResultFile();
            if (resultFile.Exists)
                this.LoadResult(resultFile);

            //System.Drawing.Color.LimeGreen

            this.calculateCommand = new CalculateCommandImplementation(this);
            this.cancelCommand = new CancelCommandImplementation(this);
            this.formatCodeCommand = new FormatCommandImplementation(this);

        }


        public TimeSpan CalculationTime
        {
            get { return (TimeSpan)this.GetValue(CalculationTimeProperty); }
            private set { this.SetValue(CalculationTimePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for CalculationTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey CalculationTimePropertyKey =
            DependencyProperty.RegisterReadOnly("CalculationTime", typeof(TimeSpan), typeof(CalculaterViewmodel), new PropertyMetadata(default(TimeSpan)));
        public static readonly DependencyProperty CalculationTimeProperty = CalculationTimePropertyKey.DependencyProperty;




        public TimeSpan TimeSinceLastStep
        {
            get { return (TimeSpan)this.GetValue(TimeSinceLastStepProperty); }
            set { this.SetValue(TimeSinceLastStepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeSinceLastStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeSinceLastStepProperty =
            DependencyProperty.Register("TimeSinceLastStep", typeof(TimeSpan), typeof(CalculaterViewmodel), new PropertyMetadata(default(TimeSpan)));




        public TimeSpan LastStepTime
        {
            get { return (TimeSpan)this.GetValue(LastStepTimeProperty); }
            set { this.SetValue(LastStepTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for timeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastStepTimeProperty =
            DependencyProperty.Register("LastStepTime", typeof(TimeSpan), typeof(CalculaterViewmodel), new PropertyMetadata(default(TimeSpan)));



        public double LastStepPropabilityGain
        {
            get { return (double)this.GetValue(LastStepPropabilityGainProperty); }
            set { this.SetValue(LastStepPropabilityGainProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastStepPropabilityGain.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastStepPropabilityGainProperty =
            DependencyProperty.Register("LastStepPropabilityGain", typeof(double), typeof(CalculaterViewmodel), new PropertyMetadata(0.0));





        public TextDocument Code
        {
            get { return (TextDocument)this.GetValue(CodeProperty); }
            private set { this.SetValue(CodePropertyKey, value); }
        }

        private const string Prefix = "ns";
        private const string Namespace = "http://DiceTool/Result";
        private const string PropabilityAttribute = "propability";
        private const string TimeAttribute = "time";

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey CodePropertyKey =
            DependencyProperty.RegisterReadOnly("Code", typeof(TextDocument), typeof(CalculaterViewmodel), new PropertyMetadata(null));
        public static readonly DependencyProperty CodeProperty = CodePropertyKey.DependencyProperty;


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
            var me = (CalculaterViewmodel)d;
            me.IsReady = !(bool)e.NewValue;
        }

        public static readonly DependencyProperty IsBuisyProperty = IsBuisyPropertyKey.DependencyProperty;





        public ObservableCollection<ResultViewmodel> Results { get; } = new ObservableCollection<ResultViewmodel>();

        private readonly Dictionary<object, ResultViewmodel> resultLookup = new Dictionary<object, ResultViewmodel>();
        private readonly CalculateCommandImplementation calculateCommand;
        private readonly CancelCommandImplementation cancelCommand;
        private readonly FormatCommandImplementation formatCodeCommand;
        private CancellationTokenSource? cancel;

        private void AddResult(object result, double propability)
        {
            ResultViewmodel vm;
            if (this.resultLookup.ContainsKey(result))
                vm = this.resultLookup[result];
            else
            {
                vm = new ResultViewmodel() { Value = result };
                this.resultLookup.Add(result, vm);
                this.Results.Add(vm);

            }

            var index = this.Results.IndexOf(vm);
            vm.Propability += propability;
            var newIndex = index;
            while (newIndex > 0 && this.Results[newIndex - 1].Propability < vm.Propability)
                newIndex--;
            while (newIndex < this.Results.Count - 1 && this.Results[newIndex + 1].Propability > vm.Propability)
                newIndex++;
            if (newIndex != index)
            {
                this.Results.Move(index, newIndex);
            }
        }


        private async Task StartCalculation()
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            var oneTurnStopwatch = new System.Diagnostics.Stopwatch();

            try
            {
                this.IsBuisy = true;
                var updateTask = UpdateTimer();
                try
                {
                    var returnType = Parser.SimpleParser.GetReturnType(this.Code.Text) ?? throw new FormatException("Was unable to determan the return type.");
                    Task calculateTask;

                    if (returnType == typeof(int))
                        calculateTask = Calculate<int>();
                    else if (returnType == typeof(string))
                        calculateTask = Calculate<string>();
                    else if (returnType == typeof(bool))
                        calculateTask = Calculate<bool>();
                    else
                        throw new NotSupportedException($"Type {returnType} is not supported");


                    await calculateTask;

                }
                finally
                {
                    this.IsBuisy = false;
                    stopWatch.Stop();
                }

                await updateTask;


                async Task UpdateTimer()
                {
                    while (this.IsBuisy)
                    {
                        await Task.Delay(500);
                        this.CalculationTime = stopWatch.Elapsed;
                        this.TimeSinceLastStep = oneTurnStopwatch.Elapsed;
                    }
                }


                async Task Calculate<T>()
                    where T : notnull
                {
                    this.cancel = new System.Threading.CancellationTokenSource();
                    try
                    {
                        //var statistics = new RunningStatistics();
                        this.calculateCommand.FireCanExecuteChange();
                        this.cancelCommand.FireCanExecuteChange();
                        var executor = Dice.Parser.SimpleParser.ParseExpression<T>(this.Code.Text);
                        this.Percentage = 0;
                        this.Results.Clear();
                        this.resultLookup.Clear();
                        executor.Epsilon = 0.0001;
                        var cal = executor.Calculate();

                        oneTurnStopwatch.Start();
                        var lastPropability = 0.0;
                        await foreach (var t in cal.WithCancellation(this.cancel.Token))
                        {
                            var elapse = oneTurnStopwatch.Elapsed;
                            oneTurnStopwatch.Restart();

                            if (elapse.TotalSeconds > 1)
                            {
                                lastPropability = this.Percentage;
                                this.LastStepTime = elapse;
                            }
                            this.Percentage = t.CompletePercentage;
                            this.AddResult(t.Result, t.Propability * 100);

                            if (elapse.TotalSeconds <= 1)
                            {
                                this.LastStepPropabilityGain = this.Percentage - lastPropability;
                            }

                        }
                        //statistics.

                        this.Percentage = 1;
                        await this.PersistResult();
                    }
                    finally
                    {
                        oneTurnStopwatch.Stop();
                        this.cancel.Dispose();
                        this.cancel = null;
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.cancelCommand.FireCanExecuteChange();
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
                if (results is null)
                    throw new IOException($"{resultFile} did not had expected format.");
                this.CalculationTime = TimeSpan.Parse(results?.Attribute(XName.Get(TimeAttribute, Namespace))?.Value ?? throw new IOException($"{resultFile} did not had expected format."), System.Globalization.CultureInfo.InvariantCulture);

                foreach (var item in results.Nodes().OfType<XElement>())
                {
                    var propability = double.Parse(item?.Attribute(XName.Get(PropabilityAttribute, Namespace))?.Value ?? throw new IOException($"{resultFile} did not had expected format."), System.Globalization.CultureInfo.InvariantCulture);
                    var value = item.Value;
                    this.Results.Add(new ResultViewmodel() { Value = value, Propability = propability });
                }

            }

        }


        public ICommand CalculateCommand => this.calculateCommand;
        public ICommand CancelCommand => this.cancelCommand;
        public ICommand FormatCodeCommand => this.formatCodeCommand;


        private class CalculateCommandImplementation : ICommand
        {
            private CalculaterViewmodel calculaterViewmodel;

            public CalculateCommandImplementation(CalculaterViewmodel calculaterViewmodel)
            {
                this.calculaterViewmodel = calculaterViewmodel;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return !this.calculaterViewmodel.IsBuisy;
            }

            public async void Execute(object? parameter)
            {
                await this.calculaterViewmodel.StartCalculation();
            }

            internal void FireCanExecuteChange()
            {
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private class CancelCommandImplementation : ICommand
        {
            private CalculaterViewmodel calculaterViewmodel;

            public CancelCommandImplementation(CalculaterViewmodel calculaterViewmodel)
            {
                this.calculaterViewmodel = calculaterViewmodel;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return this.calculaterViewmodel.IsBuisy && this.calculaterViewmodel.cancel is not null && !this.calculaterViewmodel.cancel.IsCancellationRequested;
            }

            public void Execute(object? parameter)
            {
                this.calculaterViewmodel.cancel?.Cancel();
                this.FireCanExecuteChange();
            }

            internal void FireCanExecuteChange()
            {
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private class FormatCommandImplementation : ICommand
        {
            private CalculaterViewmodel calculaterViewmodel;

            public FormatCommandImplementation(CalculaterViewmodel calculaterViewmodel)
            {
                this.calculaterViewmodel = calculaterViewmodel;
            }

            public event EventHandler? CanExecuteChanged
            {
                add { }
                remove { }
            }

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                var code = this.calculaterViewmodel.Code.Text;
                code = Parser.SimpleParser.Format(code);
                this.calculaterViewmodel.Code.Text = code;
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
