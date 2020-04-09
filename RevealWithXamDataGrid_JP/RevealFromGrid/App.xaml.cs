using Infragistics.Sdk;
using RevealFromGrid.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace RevealFromGrid
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            // ダッシュボードのローカライズ
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-CN");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("th-TH");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            // ローカルファイルを読み込む場合、LocalDataFilesRootFolderプロパティでファイルの保管場所を指定する
            //var loc = Assembly.GetExecutingAssembly().Location;
            //var dir = System.IO.Path.GetDirectoryName(loc);
            //RevealView.LocalDataFilesRootFolder = dir + @"\Data";

            RevealView.LocalizationProvider = new LocProv();
        }
    }
}
