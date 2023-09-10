using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XDEditor.Utilities;

namespace XDEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get;  set; }

        [DataMember]
        public string ProjectFile { get; set; }

        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        
        public string IconFilePath { get; set; }

        public byte[] Screenshot { get; set; }

        public string ScreenshotFilePath { get; set; }

        public string ProjectFilePath { get; set; }

    }

    class NewProject : ViewwModeBase
    {
        // TODO: get the path from the installation location.
        private readonly string templatepath = @"..\..\XDEditor\ProjectTemplates";
        private string projectName = "NewProject";

        public string ProjectName
        { 
            get => projectName;
            set
            { 
                if (projectName != value)
                {
                    projectName = value;

                    OnPropertyChanged(nameof(projectName));
                }
            }
        }

        private string projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\XDProject\";
        public string ProjectPath
        {
            get => projectPath;
            set
            {
                if (projectPath!= value)
                {
                    projectPath = value;

                    OnPropertyChanged(nameof(projectPath));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> projectTemplates = new ObservableCollection<ProjectTemplate>();

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        {
            get;
        }

        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(projectTemplates);

            try
            {
                var templateFiles = Directory.GetFiles(templatepath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());

                foreach (var file in templateFiles)
                {
                    //Read template files.
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    projectTemplates.Add(template);

                    //Write template files. (not use.)
                    //var template = new ProjectTemplate()
                    //{
                    //    ProjectType = "Empty Project",
                    //    ProjectFile = "project.xd",
                    //    Folders = new List<string> { ".Xd", "Content", "GameCode" }
                    //};

                    //Serializer.ToFile(template, file);
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: leg error
            }
        }
    }

}
