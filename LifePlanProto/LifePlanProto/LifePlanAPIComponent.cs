using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
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
    /// ・ログイン時点の年収を元に賃金カーブを定数倍することで推定
    /// ・賃金上昇率と退職金も考慮
    /// </summary>
    public class AnnualIncome
    {
        // パラメタ定義
        const long MIN_AGE = 22;
        const string FILE_NAME = "LifePlanProto.01_AnnualIncome.csv";
        const double WAGE_GROWTH_RATIO = 1.005;  // 賃金上昇率は一律0.5%に設定
        const long RETIREMENT_ALLOWANCE = 15000000;  // 退職金は一律1,500万円に設定

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
            List<long> wageCurve = new List<long>();

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
            double adjRatio = income / (double)wageCurve[(int)(age - MIN_AGE)];

            // ユーザの賃金カーブを作成
            long thisWage;
            List<long> userWageCurve = new List<long>();

            for (long i = 0; i < wageCurve.Count() - (age - MIN_AGE); i++)
            {
                thisWage = (long)(wageCurve[(int)((i + age) - MIN_AGE)] * adjRatio * Math.Pow(WAGE_GROWTH_RATIO, i));
                // 60歳で退職金を受け取る
                if (i + age == 60)
                {
                    thisWage += RETIREMENT_ALLOWANCE;
                }
                userWageCurve.Add(thisWage);
            }

            return userWageCurve;
        }
    }

    /// <summary>
    /// 年金支給額のカーブを作成するクラス
    /// </summary>
    public class Pension
    {
        // 簡単化のために以下を仮定
        // 年金受給開始年齢：65歳
        // 月間支給総額：150,000円

        // 定数
        const long PENSION_AGE = 65;
        const long AVG_PENSION_AMOUNT = 150000;

        // フィールドの定義
        private long age;

        public Pension(long age)
        {
            this.age = age;
        }

        public List<long> GenPensionCurve()
        {
            var pensionCurve = new List<long>();

            for (long i = age; i <= 100; i++)
            {
                if (i < PENSION_AGE)
                {
                    pensionCurve.Add(0);
                }
                else
                {
                    pensionCurve.Add(AVG_PENSION_AMOUNT * 12);
                }
            }

            return pensionCurve;
        }
    }

    /// <summary>
    /// 支出カーブを作成するクラス
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
