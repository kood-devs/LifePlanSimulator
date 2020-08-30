﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
    }
}