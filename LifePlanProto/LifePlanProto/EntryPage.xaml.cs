using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;

namespace LifePlanProto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryPage : ContentPage
    {
        public EntryPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            // 不正な入力がなされた場合、元のページに戻る
            // 本当は個別に入力制御したい
            try
            {
                // スクレイプ情報を整形
                Dictionary<string, long> userInput = new Dictionary<string, long> { };
                userInput["age"] = long.Parse(inputAge.Text);
                userInput["income"] = long.Parse(inputAnnualIncome.Text) * 10000;
                userInput["expenditure"] = long.Parse(inputMonthlyExpenditure.Text) * 10000 * 12;
                userInput["asset"] = long.Parse(inputAsset.Text) * 10000;
                
                // スクレイプ情報をコンストラクタに渡す
                ResultPage resultPage = new ResultPage(userInput);
                Navigation.PushAsync(resultPage);
            }
            catch (Exception)
            {
                // 入力が上手く行かない場合にはページ遷移をしない
                // 入力が不正である旨をアラートダイアログで表示
                DisplayAlert("インプットエラー", "入力内容を確認してください。", "OK");

                // 入力画面を再度表示する
                EntryPage entryPage = new EntryPage();
                Navigation.PushAsync(entryPage);
            }
        }
    }
}