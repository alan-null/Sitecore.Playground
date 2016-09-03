using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Playground.Pipelines.RenderTreeNode;
using Sitecore.Shell.Applications.ContentManager.Sidebars;

namespace Sitecore.Playground.Sitecore.Shell.Applications.ContentManager
{
    public class CustomTree : Tree
    {
        public override string RenderChildNodes(ID parent)
        {
            Assert.ArgumentNotNull(parent, "parent");
            Assert.IsNotNull(FolderItem, "FolderItem");
            Item currentItem = FolderItem.Database.GetItem(parent, FolderItem.Language);
            HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
            if (currentItem != null)
            {
                foreach (Item filterChild in FilterChildren(currentItem))
                    CustomRenderTreeNode(output, filterChild, string.Empty, filterChild.ID == FolderItem.ID);
            }
            return output.InnerWriter.ToString();
        }

        public override string RenderTree(bool showRoot)
        {
            Item parent = FolderItem.Parent;
            string inner = FolderItem.ID == RootItem.ID || parent == null ? RenderTree(RootItem, FolderItem, null, string.Empty) : RenderTree(RootItem, parent, null, string.Empty);
            if (!showRoot)
                return inner;
            HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
            CustomRenderTreeNode(output, RootItem, inner, RootItem.ID == FolderItem.ID);
            return output.InnerWriter.ToString();
        }

        private string RenderTree(Item rootItem, Item currentItem, Item innerItem, string inner)
        {
            Assert.ArgumentNotNull(rootItem, "rootItem");
            Assert.ArgumentNotNull(currentItem, "currentItem");
            Assert.ArgumentNotNull(inner, "inner");
            HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
            foreach (Item filterChild in FilterChildren(currentItem))
                CustomRenderTreeNode(output, filterChild, innerItem == null || !(filterChild.ID == innerItem.ID) ? string.Empty : inner, filterChild.ID == FolderItem.ID);
            if (currentItem.ID != rootItem.ID)
            {
                Item parent = currentItem.Parent;
                if (parent != null)
                    return RenderTree(rootItem, parent, currentItem, output.InnerWriter.ToString());
            }
            return output.InnerWriter.ToString();
        }

        protected virtual void CustomRenderTreeNode(HtmlTextWriter output, Item item, string inner, bool active)
        {
            Assert.ArgumentNotNull(output, "output");
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(inner, "inner");

            var args = new RenderTreeNodeArgs
            {
                Output = output,
                Item = item,
                Inner = inner,
                Active = active
            };
            CorePipeline.Run("renderTreeNode", args);


            var dynMethod = typeof(Tree).GetRuntimeMethods().FirstOrDefault(info => info.Name.Equals("RenderTreeNode"));
            dynMethod?.Invoke(this, new object[] { args.Output, args.Item, args.Inner, args.Active });
        }
    }
}
