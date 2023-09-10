using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
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
        private readonly string templatepath = @"..\..\XDEditor\ProjectTemplates\";
        private string projectName = "NewProject";

        public string ProjectName
        { 
            get => projectName;
            set
            { 
                if (projectName != value)
                {
                    projectName = value;
                    ValidateProjectPath();
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
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(projectPath));
                }
            }
        }

        private bool isValid;

        public bool IsValid
        {
            get => isValid;
            set
            {
                if(value != isValid)
                {
                    isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string errorMsg;

        public string ErrorMsg
        {
            get => errorMsg;
            set
            {
                if (value != errorMsg)
                {
                    errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> projectTemplates = new ObservableCollection<ProjectTemplate>();

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        {
            get;
        }

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;

            if (false == Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
            }

            path += $@"{ProjectName}\";

            IsValid = false;

            if(true == string.IsNullOrWhiteSpace(ProjectName.Trim())) 
            {
                ErrorMsg = "Type in a project name.";
            }

            else if(-1 != ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()))
            {
                ErrorMsg = "Invalid character(s) used in project name.";
            }

            else if(true == string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Select a valid project folder.";
            }

            else if (-1 != ProjectPath.IndexOfAny(Path.GetInvalidPathChars()))
            {
                ErrorMsg = "Invalid character(s) used in project path.";
            }

            else if (true == Directory.Exists(path) 
                && true == Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected project folder already exists and is not empty.";
            }

            else
            {
                ErrorMsg = string.Empty;
                IsValid = true;
            }

            return IsValid;
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

                ValidateProjectPath();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: leg error
            }
        }
    }

}
