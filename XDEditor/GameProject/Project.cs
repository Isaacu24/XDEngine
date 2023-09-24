using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace XDEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewwModeBase
    {
        public static string Extension { get; } = ".xd";
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Path { get; private set; }

        public string FullPath => $"{Path}{Name}{Extension}";

        [DataMember(Name = "Scenes")]
        public ObservableCollection<Scene> scenes = new ObservableCollection<Scene>();

        public ReadOnlyObservableCollection<Scene> Scenes
        {
            get;
        }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            scenes.Add(new Scene(this, "Default Scene"));
        }
    }
}