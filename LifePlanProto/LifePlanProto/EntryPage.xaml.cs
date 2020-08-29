using System;
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
            // ResultPageに推移
            // コンストラクタにユーザの入力を渡す
            ResultPage resultPage = new ResultPage(int.Parse(inputAge.Text), ulong.Parse(inputAnnualIncome.Text));
            Navigation.PushAsync(resultPage);
        }
    }
}