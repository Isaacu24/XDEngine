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

        //MyComputer
        private string projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\XDProject\";
        
        public string ProjectPath
        {
            get => projectPath;
            set
            {
                if (value != projectPath)
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

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();

            if(false == IsValid)
            {
                return string.Empty;
            } 

            if(false == Path.EndsInDirectorySeparator(ProjectPath))
            {
                ProjectPath += @"\";
            }

            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                if(false == Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }

                var dirInfo = new DirectoryInfo(path + @".xd\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                //Write file.
                //var project = new Project(ProjectName, path);
                //Serializer.ToFile(project, path + $"{ProjectName}" + Project.Extension);

                var projectXml = File.ReadAllText(template.ProjectFilePath);
                projectXml = string.Format(projectXml, ProjectName, ProjectPath);
                var projectpath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectpath, projectXml);

                return path;
            }

            catch (Exception ex)
            {
                return string.Empty;
            }
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
