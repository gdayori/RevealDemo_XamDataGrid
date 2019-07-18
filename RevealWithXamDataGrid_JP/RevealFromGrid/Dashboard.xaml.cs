using RevealFromGrid.ViewModel;
using Infragistics.Samples.Data.Models;
using Infragistics.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevealFromGrid
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            this.revealView1.DataSourcesRequested
                += RevealView1_DataSourcesRequested;

            this.revealView1.SaveDashboard += RevealView1_SaveDashboard;

            //Data Source Provide
            this.revealView1.DataSourceProvider =
                new SampleDataSourceProvider();
            //Data Provider
            this.revealView1.DataProvider =
                new EmbedDataProvider(this.DataContext as DashboardViewModel);

            //Show/Hide option
            var settings = new RevealSettings(null);
            settings.CanEdit = true;
            //settings.ShowMenu = false;
            //settings.ShowChangeVisualization = false;
            //settings.ShowExportImage = false;
            //settings.ShowFilters = false;
            //settings.ShowRefresh = false;
            //Set Maximized visualization
            //settings.MaximizedVisualization = settings.Dashboard.Visualizations.First();
            this.revealView1.Settings = settings;


        }
        private void RevealView1_DataSourcesRequested(
            object sender, DataSourcesRequestedEventArgs e)
        {
            //var inMemoryDSI1 = new RVInMemoryDataSourceItem("SalesRecords");
            //inMemoryDSI1.Title = "SalesRecords";
            //inMemoryDSI1.Description = "SalesRecords";

            //var inMemoryDSI2 = new RVInMemoryDataSourceItem(
            //    "SalesAmountByProductData");
            //inMemoryDSI2.Title = "SalesAmountByProductData";
            //inMemoryDSI2.Description = "SalesAmountByProductData";

            //var inMemoryDSI3 = new RVInMemoryDataSourceItem("Top30LargeDeals");
            //inMemoryDSI3.Title = "Top30LargeDeals";
            //inMemoryDSI3.Description = "Top30LargeDeals";

            //var inMemoryDSI4 = new RVInMemoryDataSourceItem("MonthlySalesAmount");
            //inMemoryDSI4.Title = "MonthlySalesAmount";
            //inMemoryDSI4.Description = "MonthlySalesAmount";


            var inMemoryDSI1 = new RVInMemoryDataSourceItem("SalesRecords");
            inMemoryDSI1.Title = "案件情報";
            inMemoryDSI1.Description = "SalesRecords";

            var inMemoryDSI2 = new RVInMemoryDataSourceItem(
                "SalesAmountByProductData");
            inMemoryDSI2.Title = "商品別_売上合計";
            inMemoryDSI2.Description = "SalesAmountByProductData";
            
            var inMemoryDSI3 = new RVInMemoryDataSourceItem("Top30LargeDeals");
            inMemoryDSI3.Title = "大規模案件_Top30";
            inMemoryDSI3.Description = "Top30LargeDeals";

            var inMemoryDSI4 = new RVInMemoryDataSourceItem("MonthlySalesAmount");
            inMemoryDSI4.Title = "月別_売上合計";
            inMemoryDSI4.Description = "MonthlySalesAmount";



            e.Callback(new RevealDataSources(
                null,
                         new List<object>() { inMemoryDSI1, inMemoryDSI2, inMemoryDSI3, inMemoryDSI4},
                    false));
        }

        private async void RevealView1_SaveDashboard(object sender, DashboardSaveEventArgs args)
        {
            //Save file
            var data = await args.Serialize();
            using (var output = File.OpenWrite($"{args.Name}.rdash"))
            {
                output.Write(data, 0, data.Length);
            }
            args.SaveFinished();
        }

        //<Grid.RowDefinitions>
        //    <RowDefinition Height = "35" />
        //    < RowDefinition Height="*"/>
        //</Grid.RowDefinitions>
        //<StackPanel Orientation = "Horizontal" >
        //    < CheckBox Content="CanEdit" IsChecked="False" Margin="10" Checked="CheckBox_Checked" Unchecked="CheckBox_Checked"/>
        //    <CheckBox Content = "ShowFilters" IsChecked="False" Margin="10" Checked="CheckBox_Checked" Unchecked="CheckBox_Checked"/>
        //    <CheckBox Content = "ShowRefresh" IsChecked="False" Margin="10" Checked="CheckBox_Checked" Unchecked="CheckBox_Checked"/>
        //    <CheckBox Content = "ShowExportImage" IsChecked="False" Margin="10" Checked="CheckBox_Checked" Unchecked="CheckBox_Checked"/>
        //    <CheckBox Content = "ShowChangeVisualization" IsChecked="False" Margin="10" Checked="CheckBox_Checked" Unchecked="CheckBox_Checked"/>
        //</StackPanel>
        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    var settings = new RevealSettings(null);
        //    switch ((sender as CheckBox).Content.ToString())
        //    {
        //        case "CanEdit":
        //            settings.CanEdit = (bool)(sender as CheckBox).IsChecked;
        //            break;
        //        case "ShowFilters":
        //            settings.ShowFilters = (bool)(sender as CheckBox).IsChecked;
        //            break;
        //        case "ShowRefresh":
        //            settings.ShowRefresh = (bool)(sender as CheckBox).IsChecked;
        //            break;
        //        case "ShowExportImage":
        //            settings.ShowExportImage = (bool)(sender as CheckBox).IsChecked;
        //            break;
        //        case "ShowChangeVisualization":
        //            settings.ShowChangeVisualization = (bool)(sender as CheckBox).IsChecked;
        //            break;
        //        default:
        //            break;
        //    }
        //    this.revealView1.Settings = settings;
        //}
    }


    public class SampleDataSourceProvider : IRVDataSourceProvider
    {
        public Task<RVDataSourceItem> ChangeDashboardFilterDataSourceItemAsync(
             RVDashboardFilter globalFilter, RVDataSourceItem dataSourceItem)
        {
            return Task.FromResult<RVDataSourceItem>(null);
        }

        public Task<RVDataSourceItem> ChangeVisualizationDataSourceItemAsync(
             RVVisualization visualization, RVDataSourceItem dataSourceItem)
        {

            var csvDsi = dataSourceItem as RVCsvDataSourceItem;
            if (csvDsi != null)
            {
                var inMemDsi = new RVInMemoryDataSourceItem(csvDsi.Id);

                return Task.FromResult((RVDataSourceItem)inMemDsi);
            }
            return Task.FromResult((RVDataSourceItem)null);

        }


    }

    public class EmbedDataProvider : IRVDataProvider
    {
        private DashboardViewModel vm;
        public EmbedDataProvider(DashboardViewModel _vm)
        {
            vm = _vm;
        }

        public Task<IRVInMemoryData> GetData(
            RVInMemoryDataSourceItem dataSourceItem)
        {
            var datasetId = dataSourceItem.DatasetId;
            if (datasetId == "SalesAmountByProductData")
            {
                var data = vm.SalesAmountByProductData.ToList<SalesAmountByProduct>();

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<SalesAmountByProduct>(data));
            }
            if (datasetId == "SalesTargetThisYear")
            {
                var data = vm.SalesTargetThisYear;

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<int>(new List<int> { data }));
            }
            if (datasetId == "TotalSalesThisYear")
            {
                var data = vm.TotalSalesThisYear;

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<int>(new List<int> { data }));
            }
            if (datasetId == "Top30LargeDeals")
            {
                var data = vm.Top30LargeDeals.ToList<Sale>();

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<Sale>(data));
            }
            if (datasetId == "MonthlySalesAmount")
            {
                var data = vm.MonthlySalesAmount.ToList<MonthlySale>();

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<MonthlySale>(data));
            }
            if (datasetId == "SalesRecords")
            {
                var data = vm.SalesRecords.ToList<Sale>();

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<Sale>(data));
            }

            else
            {
                throw new Exception("Invalid data requested");
            }
        }
    }

   
}