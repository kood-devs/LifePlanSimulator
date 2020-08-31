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
    /// <summary>
    /// 結果の集約に関する処理を記述するクラス
    /// </summary>
    public class APIMain
    {
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
        // 本当は複数のグラフを重ねて表示したい
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

        // デバッグ用に計算結果をを一つの文字列に変換して返す関数
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
    }
}
