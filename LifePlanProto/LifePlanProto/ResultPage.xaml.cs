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
            Dictionary<string, List<long>> result = GetSimulationResult(userInput);

            // データを文字列で表示（確認用）
            labelOutput.Text = GetResultString(result);

            // 資産総額グラフを表示
            List<Entry> assetEntries = GetChartEntries("TotalAsset", result, "#3200FF");
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
            List<Entry> incomeEntries = GetChartEntries("AnnualIncome", result, "#CC2504");
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
            List<Entry> expEntries = GetChartEntries("AnnualExpenditure", result, "#CC2504");
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

        // デバッグ用にデータを表示する関数
        public static string GetResultString(Dictionary<string, List<long>> dic)
        {
            string result = "";

            foreach (KeyValuePair<string, List<long>> kvp in dic)
            {
                result += $"{kvp.Key}";
                result += System.Environment.NewLine;
                foreach (var v in kvp.Value)
                {
                    result += $" {v}";
                }
                result += System.Environment.NewLine;
            }

            return result;
        }

        // スクレイプ情報を入力すると計算結果を格納したDictを返す関数
        public static Dictionary<string, List<long>> GetSimulationResult(Dictionary<string, long> userInput)
        {
            // 計算結果を格納（デバッグ用）
            Dictionary<string, List<long>> result = new Dictionary<string, List<long>> { };

            // 年齢を計算
            HouseHolderAge ageGenerator = new HouseHolderAge(userInput["age"]);
            result["Age"] = ageGenerator.GenHouseHolderAge();

            // 賃金カーブを作成
            AnnualIncome incomeGenerator = new AnnualIncome(userInput["age"], userInput["income"]);
            result["AnnualIncome"] = incomeGenerator.GenWageCurve();

            // 支出カーブを作成
            AnnualExpenditure expGenerator = new AnnualExpenditure(userInput["age"], userInput["expenditure"]);
            result["AnnualExpenditure"] = expGenerator.GenExpenditureCurve();


            // 資産総額推移を計算
            List<long> asset = new List<long> { };
            long thisAsset = userInput["asset"];
            for (int i = 0; i < result["Age"].Count; i++)
            {
                thisAsset = thisAsset + result["AnnualIncome"][i] - result["AnnualExpenditure"][i];
                asset.Add(thisAsset);
            }
            result["TotalAsset"] = asset;

            return result;
        }

        // Microchartsグラフのエントリーを作成する関数
        public static List<Entry> GetChartEntries(string colName, Dictionary<string, List<long>> result, string color = "#FF1943")
        {
            long age = result["Age"][0];
            List<Entry> entries = new List<Entry> { };
            
            foreach (var v in result[colName])
            {
                // 5の倍数の年齢のみ補助メモリを表示
                if (age % 5 == 0)
                {
                    entries.Add(
                        new Entry(v)
                        {
                            Label = "" + age,
                            ValueLabel = "" + (v / 10000) + "万円",
                            Color = SKColor.Parse(color),
                        });
                }
                else
                {
                    entries.Add(
                        new Entry(v)
                        {
                            Color = SKColor.Parse(color),
                        }
                    );
                }
                // 年齢を進める
                age++;
            }
            return entries;
        }
    }

    // 年齢を作成するクラス
    public class HouseHolderAge
    {
        private long age;

        public HouseHolderAge(long age)
        {
            this.age = age;
        }

        public List<long> GenHouseHolderAge()
        {
            List<long> ages = new List<long> { };
            for (long i = age; i <= 100; i++)
            {
                ages.Add(i);
            }
            return ages;
        }
    }

    // 賃金カーブを作成するクラス
    public class AnnualIncome
    {
        // パラメタ定義（csのベストプラクティスが分からない...）
        const long INIT_AGE = 22;
        const string FILE_NAME = "LifePlanProto.01_AnnualIncome.csv";

        // フィールドの定義
        private long age = 0;
        private long income = 0;

        public AnnualIncome(long age, long income)
        {
            this.age = age;
            this.income = income;
        }

        public List<long> GenWageCurve()
        {
            // オリジナルの賃金カーブをcsvから読み込んでListに格納
            var wageCurve = new List<long>();
            
            var assembly = typeof(AnnualIncome).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(FILE_NAME);

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    wageCurve.Add(long.Parse(reader.ReadLine()));
                }
            }

            // 賃金カーブ調整係数を準備
            // ログイン時点の年収を元に賃金カーブを定数倍して推定
            double adjRatio = income / (double)wageCurve[(int)(age - INIT_AGE)];

            // ユーザの賃金カーブを作成
            var userWageCurve = new List<long>();
            for (long i = 0; i < wageCurve.Count() - (age - INIT_AGE); i++)
            {
                userWageCurve.Add((long)(wageCurve[(int)((i + age) - INIT_AGE)] * adjRatio));
            }

            return userWageCurve;
        }
    }

    // 支出カーブを作成するクラス
    public class AnnualExpenditure
    {
        private long age = 0;
        private long expenditure = 0;

        public AnnualExpenditure(long age, long expenditure)
        {
            this.age = age;
            this.expenditure = expenditure;
        }

        public List<long> GenExpenditureCurve()
        {
            var userExpCurve = new List<long>();
            for (long i = age; i <= 100; i++)
            {
                userExpCurve.Add(expenditure);
            }
            return userExpCurve;
        }
    }
}
