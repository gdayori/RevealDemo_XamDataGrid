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
using System.Reflection;
using Infragistics.Controls.Editors;

namespace RevealFromGrid
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private Boolean _isIgnoreFilterTrigger = true;
        private DateTime _from = new DateTime(2016, 1, 1);
        private DateTime _to = new DateTime(2019, 12, 31);

        public Dashboard()
        {


            InitializeComponent();

            this.Loaded += Dashboard_Loaded;
            RevealView.FormattingProvider = new CustomFormattingProvider();
        }

        public class CustomFormattingProvider : IRVFormattingProvider
        {
            public RVBaseFormattingService GetFormattingService()
            {
                return new CustomFormattingService();
            }
        }
        public class CustomFormattingService : RVBaseFormattingService
        {
            //public override string FormatAggregatedDate(DateTime value, RVDashboardDataType type, RVDashboardDateAggregationType aggregation, RVDateFormattingSpec formatting)
            //{
            //    return base.FormatAggregatedDate(value, type, aggregation, formatting);
            //}

            public override string FormatAggregatedDate(DateTime value, RVDashboardDataType type, RVDashboardDateAggregationType aggregation, RVDateFormattingSpec formatting)
            {
                if (aggregation == RVDashboardDateAggregationType.Year)
                {
                    return string.Format("{0:yyyy年}", value);
                }
                if (aggregation == RVDashboardDateAggregationType.Month)
                {
                    return string.Format("{0:yyyy年MM月}", value);
                }
                if (aggregation == RVDashboardDateAggregationType.Day)
                {
                    return string.Format("{0:yyyy年MM月dd日}", value);
                }

                return base.FormatAggregatedDate(value, type, aggregation, formatting);
            }
        }

        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            //インメモリデータを設定するためのイベントハンダラ
            this.revealView1.DataSourcesRequested
                += RevealView1_DataSourcesRequested;

            //ダッシュボード定義情報の保存のためのイベントハンダラ
            this.revealView1.SaveDashboard += RevealView1_SaveDashboard;

            //ダッシュボード画像の保存のためのイベントハンダラ
            this.revealView1.ImageExported += RevealView1_ImageExported;
            

            //データプロバイダの設定
            this.revealView1.DataProvider =
                new EmbedDataProvider(this.DataContext as DashboardViewModel);

            //既に定義ファイルがある場合は読み込み、なければ新規ダッシュボードとして立ち上げる
            var path = @"..\..\Dashboards\Sales.rdash";
            if (UserInfo.showGlobalFilter == true)
            {
                path = @"..\..\Dashboards\SalesWithGlobalFilter.rdash";
            }
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
            settings.ShowExportToPowerpoint = true;
            settings.ShowExportToPDF = true;


            //外部グローバルフィルターの設定
            if (UserInfo.showGlobalFilter == true)
            {
                // Filter settings
                globalFilter.Visibility = Visibility.Visible;
                _from = fromThumb.Value;
                _to = toThumb.Value;
                settings.ShowFilters = false;
                _isIgnoreFilterTrigger = false;
                settings.DateFilter = new RVDateDashboardFilter(RVDateFilterType.CustomRange, new RVDateRange(_from, _to));
            }
            else
            {
                globalFilter.Visibility = Visibility.Collapsed;
            }

            // 上記設定情報をRevealViewに適用
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

            List<object> datasources = new List<object>();
            List<object> datasourceItems = new List<object>();

            // インメモリデータがデータソースの場合
            var inMemoryDSI1 = new RVInMemoryDataSourceItem("SalesRecords");
            inMemoryDSI1.Title = "案件情報";
            inMemoryDSI1.Description = "SalesRecords";
            datasourceItems.Add(inMemoryDSI1);

            var inMemoryDSI2 = new RVInMemoryDataSourceItem(
                "SalesAmountByProductData");
            inMemoryDSI2.Title = "商品別_売上合計";
            inMemoryDSI2.Description = "SalesAmountByProductData";
            datasourceItems.Add(inMemoryDSI2);

            var inMemoryDSI3 = new RVInMemoryDataSourceItem("Top30LargeDeals");
            inMemoryDSI3.Title = "大規模案件_Top30";
            inMemoryDSI3.Description = "Top30LargeDeals";
            datasourceItems.Add(inMemoryDSI3);

            var inMemoryDSI4 = new RVInMemoryDataSourceItem("MonthlySalesAmount");
            inMemoryDSI4.Title = "月別_売上合計";
            inMemoryDSI4.Description = "MonthlySalesAmount";
            datasourceItems.Add(inMemoryDSI4);

            // Excelファイルがデータソースの場合
            RVLocalFileDataSourceItem localExcelDatasource = new RVLocalFileDataSourceItem();
            localExcelDatasource.Uri = "local:/SampleData.xlsx";
            RVExcelDataSourceItem excelDatasourceItem = new RVExcelDataSourceItem(localExcelDatasource);
            excelDatasourceItem.Title = "Excelデータ";
            datasourceItems.Add(excelDatasourceItem);

            // CSVファイルがデータソースの場合
            RVLocalFileDataSourceItem localCsvDatasource = new RVLocalFileDataSourceItem();
            localCsvDatasource.Uri = "local:/SampleData.csv";
            RVExcelDataSourceItem csvDatasourceItem = new RVExcelDataSourceItem(localCsvDatasource);
            csvDatasourceItem.Title = "CSVデータ";
            datasourceItems.Add(csvDatasourceItem);


            e.Callback(new RevealDataSources(
                    null,
                    datasourceItems,
                    false));
        }

        private async void RevealView1_SaveDashboard(object sender, DashboardSaveEventArgs args)
        {
            //Save file
            var data = await args.Serialize();
            var path = @"..\..\Dashboards\Sales.rdash";
            if (UserInfo.showGlobalFilter == true)
            {
                path = @"..\..\Dashboards\SalesWithGlobalFilter.rdash";
            }
            //using (var output = File.OpenWrite($"{args.Name}.rdash"))
            File.Delete(path);
            using (var output = File.OpenWrite(path))
            {
                output.Write(data, 0, data.Length);
            }
            args.SaveFinished();
        }

        private void DateFromChanged(object sender, RoutedPropertyChangedEventArgs<DateTime> e)
        {
            var thumb = sender as XamSliderDateTimeThumb;
            var value = thumb.Value;
            lblFrom.Text = AdjustFromDate(value).ToString("yyyy/MM/dd");

            if (revealView1 != null && _isIgnoreFilterTrigger == false)
            {
                _from = value;
                UpdateDateFilter();
            }

        }

        private void DateToChanged(object sender, RoutedPropertyChangedEventArgs<DateTime> e)
        {

            var thumb = sender as XamSliderDateTimeThumb;
            var value = thumb.Value;
            lblTo.Text = AdjustToDate(value).ToString("yyyy/MM/dd");

            if (revealView1 != null && _isIgnoreFilterTrigger == false)
            {
                _to = value;
                UpdateDateFilter();
            }
        }

        private void UpdateDateFilter()
        {
            var from = AdjustFromDate(_from);
            var to = AdjustToDate(_to);
            var range = new RVDateRange(from, to);
            var filter = new RVDateDashboardFilter(RVDateFilterType.CustomRange, range);

            revealView1.SetDateFilter(filter);
        }

        private DateTime AdjustFromDate(DateTime from)
        {
            return new DateTime(from.Year, from.Month, 1);
        }

        private DateTime AdjustToDate(DateTime to)
        {
            return new DateTime(to.Year, to.Month, 1).AddMonths(1).AddDays(-1);
        }

        private void fromThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _isIgnoreFilterTrigger = false;

            var thumb = sender as XamSliderDateTimeThumb;
            var value = thumb.Value;
            _from = value;
            UpdateDateFilter();

        }
        private void toThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _isIgnoreFilterTrigger = false;

            var thumb = sender as XamSliderDateTimeThumb;
            var value = thumb.Value;
            _to = value;
            UpdateDateFilter();

        }

        private void fromThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isIgnoreFilterTrigger = true;
        }

        private void toThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isIgnoreFilterTrigger = true;

        }

        private void item_Checked(object sender, RoutedEventArgs e)
        {
            if (_isIgnoreFilterTrigger == false)
                applyCategoryFilter();
        }

        private void item_Unchecked(object sender, RoutedEventArgs e)
        {
            if(_isIgnoreFilterTrigger==false)
                applyCategoryFilter();
        }

        private void applyCategoryFilter()
        {
            List<object> selectedProducts = new List<object>();
            selectedProducts.Clear();
            if (check1.IsChecked == true)
                selectedProducts.Add(check1.Content);
            if (check2.IsChecked == true)
                selectedProducts.Add(check2.Content);
            if (check3.IsChecked == true)
                selectedProducts.Add(check3.Content);
            if (check4.IsChecked == true)
                selectedProducts.Add(check4.Content);
            if(selectedProducts.Count == 0)
                selectedProducts.Add("---");
            revealView1.SetFilterSelectedValues(revealView1.Dashboard.GetFilterByTitle("商品名"), selectedProducts);
        }
    }

    public class EmbedDataProvider : IRVDataProvider
    {
        private DashboardViewModel vm;
        public EmbedDataProvider(DashboardViewModel _vm)
        {
            vm = _vm;
        }

        // インメモリデータソースの割り当て
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
}