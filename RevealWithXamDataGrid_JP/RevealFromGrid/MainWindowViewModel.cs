using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Infragistics.Samples.Data.Models;
using System.Diagnostics;

namespace RevealFromGrid.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OpenDashboard { get; set; }
        public ICommand OpenBiWindow { get; set; }

        public MainWindowViewModel()
        {
            //Get sales data to be bound to grid
            SalesDataSample salesDataSample = new SalesDataSample();
            salesRecords = salesDataSample.SalesData;

            // Commands
            OpenDashboard = new OpenDashboardCommand();
            OpenBiWindow = new OpenPivotCommand();
        }

        private ObservableCollection<Sale> salesRecords;
        public ObservableCollection<Sale> SalesRecords
        {
            get { return salesRecords; }
        }
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
    
    public class OpenDashboardCommand : ICommand
    {
        public OpenDashboardCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            // 編集権限の設定
            UserInfo.permissionLevel = int.Parse(parameter as string);
            // ダッシュボードを開く
            var newWindow = new Dashboard();
            newWindow.Show();
        }
    }
    public class OpenPivotCommand : ICommand
    {
        public OpenPivotCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var newWindow = new Pivot();
            newWindow.Show();
        }
    }
}
