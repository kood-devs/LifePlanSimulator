using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

using Microcharts;
using Entry = Microcharts.ChartEntry;
using SkiaSharp;

namespace LifePlanProto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage : ContentPage
    {
        private int userAge;
        private ulong userIncome;

        // グラフ用
        public List<Entry> entries = new List<Entry>{ };

        public ResultPage(int userAge, ulong userIncome)
        {
            // ユーザ入力の年齢と年収を設定
            this.userAge = userAge;
            this.userIncome = userIncome * 10000;  // 万円→円に修正

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            // 賃金カーブを作成
            AnnalIncome incomeGenerator = new AnnalIncome(this.userAge, this.userIncome);
            List<ulong> income = incomeGenerator.GenWageCurve();

            // データを文字列で表示（確認用）
            string incomeString = "Income:";
            foreach (var v in income)
            {
                incomeString = incomeString + " " + v;
            }
            labelTestIncome.Text = incomeString;


            // グラフにデータをセット
            int spotAge = userAge;
            foreach(var spotIncome in income)
            {
                if (spotAge % 5 == 0)
                {
                    entries.Add(
                        new Entry(spotIncome)
                        {
                            Label = "" + spotAge,
                            ValueLabel = "" + (spotIncome / 10000) + "万円",
                            Color = SKColor.Parse("#FF1943"),
                        });
                }
                else
                {
                    entries.Add(
                        new Entry(spotIncome)
                        {
                            Color = SKColor.Parse("#FF1943"),
                        }
                    );
                }
                // 年齢を進める
                spotAge++;
            }

            // グラフを表示
            IncomeChart.Chart = new LineChart() 
            {
                Entries = entries,
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 0,
                LabelTextSize = 30,
                LabelColor = SKColor.Parse("#000000"),
            };
        }
    }

    public class AnnalIncome
    {
        // パラメタ定義（csのベストプラクティスが分からない...）
        const int INIT_AGE = 22;
        const string FILE_NAME = "LifePlanProto.01_AnnualIncome.csv";

        // フィールドの定義
        private int age = 0;
        private ulong income = 0;

        public AnnalIncome(int age, ulong income)
        {
            this.age = age;
            this.income = income;
        }

        public List<ulong> GenWageCurve()
        {
            // オリジナルの賃金カーブをcsvから読み込んでListに格納
            var wageCurve = new List<ulong>();
            
            var assembly = typeof(AnnalIncome).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(FILE_NAME);

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    wageCurve.Add(ulong.Parse(reader.ReadLine()));
                }
            }

            // 賃金カーブ調整係数を準備
            double adjRatio = income / (double)wageCurve[age - INIT_AGE];

            // ユーザの賃金カーブを作成
            var userWageCurve = new List<ulong>();
            for (int i = 0; i < wageCurve.Count() - (age - INIT_AGE); i++)
            {
                userWageCurve.Add((ulong)(wageCurve[(i + age) - INIT_AGE] * adjRatio));
            }

            return userWageCurve;
        }
    }
}