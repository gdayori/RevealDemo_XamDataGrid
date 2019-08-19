using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
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
            //Dashboard launguage setting
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-CN");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("th-TH");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
        }
    }
}
