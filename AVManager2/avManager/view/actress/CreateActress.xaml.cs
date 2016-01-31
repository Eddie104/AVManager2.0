using avManager.model;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AVManager2.avManager.view.actress
{
    /// <summary>
    /// CreateActress.xaml 的交互逻辑
    /// </summary>
    public partial class CreateActress : MetroWindow
    {
        public CreateActress()
        {
            InitializeComponent();
        }

        private void onCreateActress(object sender, RoutedEventArgs e)
        {
            string name = this.nameTextBox.Text;
            string alias = this.aliasTextBox.Text;

            string birtydayStr = this.birthdayTextBox.Text;
            var a = birtydayStr.Split(new char[] { '-' });
            int year = 0, month = 0, day = 0;
            int.TryParse(a[0], out year);
            int.TryParse(a[1], out month);
            int.TryParse(a[2], out day);
            DateTime birthday = new DateTime(year, month, day);

            int bust = 0;
            int.TryParse(this.bustTextBox.Text, out bust);
            int waist = 0;
            int.TryParse(this.waistTextBox.Text, out waist);
            int hip = 0;
            int.TryParse(this.hipTextBox.Text, out hip);
            string cup = this.cupTextBox.Text;
            string code = this.codeTextBox.Text;
            int height = 0;
            int.TryParse(this.heightTextBox.Text, out height);
            int score = 0;
            int.TryParse(this.scoreTextBox.Text, out score);
            ActressManager.GetInstance().AddActress(name, alias, birthday, height, bust, waist, hip, cup, code);
            MessageBox.Show("添加成功");
        }
    }
}
