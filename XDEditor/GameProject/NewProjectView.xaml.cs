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
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {   
        public NewProjectView()
        {
            InitializeComponent();
        }

        private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as NewProject;
            var projectPath = viewModel.CreateProject(templateListBox.SelectedItem as ProjectTemplate);
            bool dialogResult = false;
            var window = Window.GetWindow(this);
            
            if (false == string.IsNullOrEmpty(projectPath))
            { 
                dialogResult = true;

                var project = OpenProject.Open(new ProjectData()
                {
                    ProjectName = viewModel.ProjectName,
                    ProjectPath = projectPath
                });
                window.DataContext = project;
            }

            window.DialogResult = dialogResult;
            window.Close();
        }
    }
}
