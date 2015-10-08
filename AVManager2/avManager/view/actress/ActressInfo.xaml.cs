using avManager.model.data;
using libra.log4CSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AVManager2.avManager.view.actress
{
    /// <summary>
    /// ActressListContainer.xaml 的交互逻辑
    /// </summary>
    public partial class ActressInfo : UserControl
    {

        public event EventHandler Click;

        private Actress actress;
        public Actress Actress
        {
            get { return actress; }
            set
            {
                actress = value;
                if (actress == null)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    nameLabel.Text = actress.Name;
                    ageLabel.Text = string.Format("年龄:{0}", DateTime.Now.Year - actress.Birthday.Year);
                    heightLabel.Text = string.Format("身高:{0}", actress.Height);

                    string imgPath = string.Format("{0}{1}.jpg", Config.ACTRESS_IMG_PATH, actress.Code);
                    if (File.Exists(imgPath))
                    {
                        headImg.Source = new BitmapImage(new Uri(imgPath));
                    }
                    else
                    {
                        Logger.Error(string.Format("图片:{0}.jpg不存在", actress.Code));
                    }
                }
            }
        }

        public ActressInfo()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Click(this, e);
            e.Handled = false;
        }
    }
}
