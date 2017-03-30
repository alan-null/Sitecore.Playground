using System.Collections.Generic;
using Unicorn.Predicates;

namespace Sitecore.Playground.Serialization.Predicates
{
    public class CustomPresetTreeRoot : PresetTreeRoot
    {
        public CustomPresetTreeRoot(string name, string path, string databaseName) : base(name, path, databaseName) { }

        public IList<IPresetItemExclusion> AdvancedExclusions { get; set; }
    }
}