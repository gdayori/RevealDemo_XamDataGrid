using Infragistics.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevealFromGrid.Services
{
    public class LocProv : IRVLocalizationProvider
    {
        public IRVLocalizationService GetLocalizationService()
        {
            return new LocServ();
        }

        public class test : RVBaseFormattingService
        {

        }
        public class LocServ : IRVLocalizationService
        {
            
            public RVFormattingSpec GetFormattingSettingsForField(string fieldName, RVDashboardDataType dataType, RVFormattingSpec currentSettings, bool isAggregated)
            {
                if (dataType == RVDashboardDataType.Date && isAggregated == false)
                {
                    var newSettings = new RVDateFormattingSpec()
                    {
                        DateFormat = "YYYY年MM月dd日"
                    };

                    return newSettings;
                }
                else if (dataType == RVDashboardDataType.Number)
                {
                    if (currentSettings != null && (currentSettings as RVNumberFormattingSpec).FormatType == RVDashboardNumberFormattingType.Currency)
                    {
                        var numberFormatting = new RVNumberFormattingSpec();
                        switch ((currentSettings as RVNumberFormattingSpec).FormatType)
                        {
                            case RVDashboardNumberFormattingType.Currency:
                                numberFormatting.ApplyMkFormat = false;
                                numberFormatting.ShowGroupingSeparator = true;
                                numberFormatting.DecimalDigits = 0;
                                numberFormatting.CurrencySymbol = "¥";
                                numberFormatting.FormatType = RVDashboardNumberFormattingType.Currency;
                                return numberFormatting;
                            case RVDashboardNumberFormattingType.Number:
                                break;
                            case RVDashboardNumberFormattingType.Percent:
                                break;
                            default:
                                break;
                        }
                    }
                }

                return currentSettings;
            }


            public string GetLocalizedString(string originalValue, RVLocalizationElementType elementType)
            {
                //if (originalValue != null)
                //{
                //    // ラベル（データスキーマやWidgetタイトル、Dashboardタイトル等）のローカライズ
                //    return originalValue.Replace("Sales", "売上");
                //}
                return originalValue;
                // ↑リソースファイルを利用する場合はループで対応
            }
        }
    }
}
