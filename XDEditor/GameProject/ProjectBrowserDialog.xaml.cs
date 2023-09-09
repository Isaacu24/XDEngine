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

namespace XDEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialog.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        public ProjectBrowserDialog()
        {
            InitializeComponent();
        }

        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender == openProjectButton)
            {
                if(true == newProjectButton.IsChecked)
                {
                    newProjectButton.IsChecked = false;
                    browserContent.Margin = new Thickness(0);
                }

                newProjectButton.IsChecked = true;
            }

            else
            {

                if (true == openProjectButton.IsChecked)
                {
                    openProjectButton.IsChecked = false;
                    browserContent.Margin = new Thickness(-800, 0, 0, 0);
                }

                openProjectButton.IsChecked = true;
            }
        }

        private void newProjectButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
