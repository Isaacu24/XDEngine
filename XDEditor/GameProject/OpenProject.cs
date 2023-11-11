﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XDEditor.Utilities;

namespace XDEditor.GameProject
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName {  get; set; }
        [DataMember]
        public string ProjectPath { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }

        public byte[] Icon { get; set; }

        public byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }
     
    class OpenProject
    {
        private static readonly string _appplicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\XDEditor";
        private static readonly string _projectDataPath;
         
        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects
        { 
            get; 
        }

        private static void ReadProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);
                _projects.Clear();

                foreach (var project in projects)
                {
                    if(File.Exists(project.FullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.XD\Icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.XD\Screenshot.png");
                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projects.OrderBy(p => p.ProjectName).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }

        public static Project Open(ProjectData data)
        {
            ReadProjectData();

            var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);

            if (null != project)
            {
                project.Date =  DateTime.Now;
            }

            else
            {
                project = data;
                project.Date =  DateTime.Now;
                _projects.Add(project);
            }

            WriteProjectData();
            return Project.Load(project.FullPath);
        }

        static OpenProject()
        {
            try
            {
                if(false == Directory.Exists(_appplicationDataPath))
                {
                    Directory.CreateDirectory(_appplicationDataPath);
                }

                _projectDataPath = $@"{_appplicationDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
                ReadProjectData();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
