﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Infragistics.Olap.FlatData;
using Infragistics.Samples.Data.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevealFromGrid.ViewModel
{
    class PivotViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PivotViewModel()
        {
            //Get all data required in the pivot controls
            SalesDataSample salesDataSample = new SalesDataSample();
            var flatDataSource = new Infragistics.Olap.FlatData.FlatDataSource();
            flatDataSource.ItemsSource = salesDataSample.SalesData;
            salesFlatDataSource = flatDataSource;
        }

        //Flat DataSource to be bound to pivot controls
        private FlatDataSource salesFlatDataSource;
        public FlatDataSource SalesFlatDataSource
        {
            get { return salesFlatDataSource; }
        }
    }

}
