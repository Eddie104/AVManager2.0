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
using avManager.model.data;
using System.IO;
using libra.log4CSharp;
using avManager.model;

namespace AVManager2.avManager.view.actress
{
    /// <summary>
    /// ActressDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ActressDetail : Window
    {
        private Actress actress;
        public Actress Actress {
            get { return actress; }
            set
            {
                actress = value;
                nameLabel.Text = actress.Name;
                ageLabel.Text = string.Format("年龄:{0}", DateTime.Now.Year - actress.Birthday.Year);
                heightLabel.Text = string.Format("身高:{0}", actress.Height);
                waistLabel.Text = string.Format("腰围:{0}", actress.Waist);
                bustLabel.Text = string.Format("胸围:{0}", actress.Bust);
                hipLabel.Text = string.Format("臀围:{0}", actress.Hip);
                cupLabel.Text = string.Format("罩杯:{0}", actress.Cup);
                scoreComboBox.SelectedIndex = actress.Score;

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

        public ActressDetail()
        {
            InitializeComponent();
        }

        private void scoreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Actress.Score = (sender as ComboBox).SelectedIndex;
            this.Actress.NeedUpdate = true;
        }
    }
}
