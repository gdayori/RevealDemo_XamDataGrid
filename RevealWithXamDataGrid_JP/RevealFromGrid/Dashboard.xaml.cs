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
using System.Globalization;

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

            this.Loaded += Dashboard_Loaded;

        }

        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            //インメモリデータを設定するためのイベントハンダラ
            this.revealView1.DataSourcesRequested
                += RevealView1_DataSourcesRequested;

            //ダッシュボード保存のためのイベントハンダラ
            this.revealView1.SaveDashboard += RevealView1_SaveDashboard;

            this.revealView1.ImageExported += RevealView1_ImageExported;

            //データプロバイダの設定
            this.revealView1.DataProvider =
                new EmbedDataProvider(this.DataContext as DashboardViewModel);

            //既に定義ファイルがある場合は読み込み、なければ新規ダッシュボードとして立ち上げる
            var path = @"..\..\Dashboards\Sales.rdash";
            var revealView = new RevealView();
            RVDashboard dashboard = null; // nullを設定すると新規ダッシュボード作成となる

            if (File.Exists(path))
            {
                using (var fileStream = File.OpenRead(path))
                {
                    //定義ファイルの読み込み
                    dashboard = await RevealUtility.LoadDashboard(fileStream);
                }
            }
            var settings = new RevealSettings(dashboard);

            if (UserInfo.permissionLevel == 0)
            {
                //編集許可
                settings.CanEdit = true;
                settings.ShowMenu = true;
            }
            else
            {
                //編集禁止
                settings.CanEdit = false;
                settings.ShowMenu = true;

            }
            //その他オプション設定
            settings.ShowChangeVisualization = true;
            settings.CanSaveAs = false;  // rdash定義ファイルの保存メニュー
            settings.ShowExportImage = true;
            settings.ShowFilters = true;
            settings.ShowRefresh = true;

            //Set Maximized visualization
            //settings.MaximizedVisualization = settings.Dashboard.Visualizations.First();

            this.revealView1.Settings = settings;

            if (UserInfo.permissionLevel != 0 && dashboard is null)
            {
                //編集権限がなく、ダッシュボードが未だ存在しない場合
                this.revealView1.Visibility = Visibility.Collapsed;
                MessageBox.Show("ダッシュボードが定義されていません。");
            }

        }

        private void RevealView1_ImageExported(object sender, ImageExportedEventArgs e)
        {
            // ダッシュボード画像を保存する
            using (Stream stream = new FileStream(@"..\..\Images\SavedDashboardImg.png", FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(e.Image));
                encoder.Save(stream);
            }
            MessageBox.Show("画像を保存しました。");
            e.CloseExportDialog = false;
        }

        private void RevealView1_DataSourcesRequested(
            object sender, DataSourcesRequestedEventArgs e)
        {


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
            var path = @"..\..\Dashboards\Sales.rdash";
            //using (var output = File.OpenWrite($"{args.Name}.rdash"))
            using (var output = File.OpenWrite(path))
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

    //public class SampleDataSourceProvider : IRVDataSourceProvider
    //{
    //    public Task<RVDataSourceItem> ChangeDashboardFilterDataSourceItemAsync(
    //         RVDashboardFilter globalFilter, RVDataSourceItem dataSourceItem)
    //    {
    //        return Task.FromResult<RVDataSourceItem>(null);
    //    }

    //    public Task<RVDataSourceItem> ChangeVisualizationDataSourceItemAsync(
    //         RVVisualization visualization, RVDataSourceItem dataSourceItem)
    //    {

    //        var csvDsi = dataSourceItem as RVCsvDataSourceItem;
    //        if (csvDsi != null)
    //        {
    //            var inMemDsi = new RVInMemoryDataSourceItem(csvDsi.Id);

    //            return Task.FromResult((RVDataSourceItem)inMemDsi);
    //        }
    //        return Task.FromResult((RVDataSourceItem)null);

    //    }

    //}



}