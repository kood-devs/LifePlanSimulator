using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


// APIの個別項目を計算するクラス
namespace LifePlanProto
{
    /// <summary>
    /// 現在年齢～100歳までの年齢を作成するクラス
    /// </summary>
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

    /// <summary>
    /// 賃金カーブを作成するクラス
    /// </summary>
    public class AnnualIncome
    {
        // パラメタ定義
        const long INIT_AGE = 22;
        const string FILE_NAME = "LifePlanProto.01_AnnualIncome.csv";

        // フィールドの定義
        private long age;
        private long income;

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
            // ログイン時点の年収を元に賃金カーブを定数倍することで推定
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

    /// <summary>
    /// 支出カーブを作成するクラスを作成するクラス
    /// </summary>
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
