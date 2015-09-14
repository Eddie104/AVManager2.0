using avManager.model.data;
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
