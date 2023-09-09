using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDEditor.GameProject
{
    public class ProjectTemplate
    { 
        public string ProjectType { get;  set; }
        public string ProjectFile { get; set; }
    }

    class NewProject : ViewwModeBase
    {
        private string name = "NewProject";
        public string Name
        { 
            get => name;
            set
            { 
                if (name != value)
                {
                    name = value;

                    OnPropertyChanged(nameof(name));
                }
            }
        }

        private string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\XDProject\";
        public string Path
        {
            get => path;
            set
            {
                if (path!= value)
                {
                    path = value;

                    OnPropertyChanged(nameof(path));
                }
            }
        }
    }

}
