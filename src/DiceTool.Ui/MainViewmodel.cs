using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Dice.Ui
{
    class MainViewmodel : DependencyObject
    {

        public ObservableCollection<CalculaterViewmodel> Data { get; } = new ObservableCollection<CalculaterViewmodel>();

        public ICommand AddCommand { get; }

        public MainViewmodel()
        {
            var appFolder = GetAppFolder();

            foreach (var item in appFolder.GetDirectories())
                this.Data.Add(new CalculaterViewmodel(item));

            AddCommand = new Command(this);

        }

        public void AddNewModel(string name)
        {
            if (this.Data.Any(x => x.Name == name))
                throw new ArgumentException($"Name {name} already in use");

            var modelFolder = GetAppFolder().CreateSubdirectory(name);

            this.Data.Add(new CalculaterViewmodel(modelFolder));

        }


        private static DirectoryInfo GetAppFolder()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
            var appFolder = new DirectoryInfo(Path.Combine(appDataFolder, ".DiceTool"));
            if (!appFolder.Exists)
                appFolder.Create();
            return appFolder;
        }

        private class Command : ICommand
        {
            private MainViewmodel mainViewmodel;

            public Command(MainViewmodel mainViewmodel)
            {
                this.mainViewmodel = mainViewmodel;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                var dialog = new InputDialog();
                if (dialog.ShowDialog() ?? false)
                {
                    mainViewmodel.AddNewModel(dialog.Input);
                }
            }
        }
    }
}
