using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentManager;
using Sitecore.Shell.Applications.ContentManager.Sidebars;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.Playground.Sitecore.Shell.Applications.ContentManager
{
    public class CustomContentEditorForm: ContentEditorForm
    {
        protected override Sidebar GetSidebar()
        {
            CustomTree result = new CustomTree
            {
                ID = "Tree",
                DataContext = new DataContext
                {
                    DataViewName = "Master"
                }
            };
            return Assert.ResultNotNull(result);
        }
    }
}