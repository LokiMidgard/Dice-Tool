using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiceTool.Viewmodel
{
    class ParameterSetupViewmodel : DependencyObject
    {

        public ParameterSetupViewmodel()
        {
            this.NumberParameter = new NumberParameter();
            this.StringParameter = new StringParameter();
            Configurations.Add(NumberParameter);
            Configurations.Add(StringParameter);
                    }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(ParameterSetupViewmodel), new PropertyMetadata(false));

        public IList<ParameterSetupConfigurationViewmodel> Configurations { get; } = new List<ParameterSetupConfigurationViewmodel>();



        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(ParameterSetupViewmodel), new PropertyMetadata(-1, SelectedIndexChanged));

        private static void SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as ParameterSetupViewmodel;
            for (int i = 0; i < me.Configurations.Count; i++)
                me.Configurations[i].IsEnabled = (int)e.NewValue == i;
        }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public NumberParameter NumberParameter { get; private set; }
        public StringParameter StringParameter { get; private set; }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register(nameof(Name), typeof(string), typeof(ParameterSetupViewmodel), new PropertyMetadata(null));





    }

    abstract class ParameterSetupConfigurationViewmodel : DependencyObject
    {
        public ParameterSetupConfigurationViewmodel(string name)
        {
            Name = name;
        }
        public String Name { get; }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(ParameterSetupConfigurationViewmodel), new PropertyMetadata(false));
    }

    abstract class ParameterSetupConfigurationViewmodel<T> : ParameterSetupConfigurationViewmodel
    {
        public ParameterSetupConfigurationViewmodel(string name) : base(name) { }
        public Type Type { get; } = typeof(T);

        public abstract IEnumerable<T> Values { get; }

    }
    class NumberParameter : ParameterSetupConfigurationViewmodel<int>
    {
        public NumberParameter() : base("Number")
        {

        }

        public int From
        {
            get { return (int)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(int), typeof(NumberParameter), new PropertyMetadata(0));

        public int To
        {
            get { return (int)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public override IEnumerable<int> Values
        {
            get
            {
                return Enumerable.Range(From, To - From);
            }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(int), typeof(NumberParameter), new PropertyMetadata(0));
    }

    class StringParameter : ParameterSetupConfigurationViewmodel<String>
    {
        public StringParameter() : base("String") { }

        public ObservableCollection<String> Strings { get; } = new ObservableCollection<string>();

        public override IEnumerable<string> Values => Strings;

    }

}
