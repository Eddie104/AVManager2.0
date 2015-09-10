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

        private Actress actress;
        public Actress Actress
        {
            get { return actress; }
            set
            {
                actress = value;
                nameLabel.Text = actress.Name;
                ageLabel.Text = string.Format("年龄:{0}", DateTime.Now.Year - actress.Birthday.Year);
                heightLabel.Text = string.Format("身高:{0}", actress.Height);
            }
        }

        public ActressInfo()
        {
            InitializeComponent();
        }
    }
}
