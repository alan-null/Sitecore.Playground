using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Pipelines;

namespace Sitecore.Playground.Pipelines.RenderTreeNode
{
    public class RenderTreeNodeArgs : PipelineArgs
    {
        public HtmlTextWriter Output { get; set; }
        public Item Item { get; set; }
        public string Inner { get; set; }
        public bool Active { get; set; }
    }
}