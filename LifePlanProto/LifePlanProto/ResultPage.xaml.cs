using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

// グラフ描写用
using SkiaSharp;
using Microcharts;
using Entry = Microcharts.ChartEntry;


namespace LifePlanProto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage : ContentPage
    {
        // メインの処理
        public ResultPage(Dictionary<string, long> userInput)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            // 計算結果を取得
            Dictionary<string, List<long>> result = APIMain.GetSimulationResult(userInput);

            // データを文字列で表示（確認用）
            // 計算過程を確認したい場合には、コメントを外す
            // labelOutput.Text = APIMain.GetResultString(result);

            // 資産総額グラフを表示
            List<Entry> assetEntries = APIMain.GetChartEntries("TotalAsset", result, "#3200FF");
            AssetChart.Chart = new LineChart()
            {
                Entries = assetEntries,
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 0,
                LabelTextSize = 30,
                LabelColor = SKColor.Parse("#000000"),
            };

            if (result["TotalAsset"][result["TotalAsset"].Count-1] >= 0)
            {
                labelResult.Text = "目標達成！！！";
                labelResult.TextColor = Color.Blue;
            }
            else
            {
                labelResult.Text = "目標未達成...";
                labelResult.TextColor = Color.Red;
            }

            // 収入グラフを表示
            List<Entry> incomeEntries = APIMain.GetChartEntries("CashInFlow", result, "#CC2504");
            IncomeChart.Chart = new LineChart() 
            {
                Entries = incomeEntries,
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 0,
                LabelTextSize = 30,
                LabelColor = SKColor.Parse("#000000"),
            };

            // 支出グラフを表示
            List<Entry> expEntries = APIMain.GetChartEntries("CashOutFlow", result, "#CC2504");
            ExpenditureChart.Chart = new LineChart()
            {
                Entries = expEntries,
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 0,
                LabelTextSize = 30,
                LabelColor = SKColor.Parse("#000000"),
            };
        }
    }
}
