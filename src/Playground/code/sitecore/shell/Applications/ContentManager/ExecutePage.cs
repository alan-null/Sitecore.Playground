using System;
using System.Collections.Specialized;
using ComponentArt.Web.UI;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Data.Treeviews;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Reflection;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using Page = System.Web.UI.Page;

namespace Sitecore.Playground.Sitecore.Shell.Applications.ContentManager
{
    public class ExecutePage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            if (!ShellPage.IsLoggedIn())
                return;
            string s = string.Empty;
            switch (WebUtil.GetQueryString("cmd").ToLowerInvariant())
            {
                case "gettreeviewchildren":
                    s = GetTreeViewChildren();
                    break;
                case "expandtreeviewtonode":
                    s = ExpandTreeViewToNode();
                    break;
                case "getcontextualtabs":
                    s = GetContextualTabs();
                    break;
                case "convert":
                    s = DesignTimeConvert();
                    break;
                case "treeviewcontent":
                    s = GetTreeViewContent();
                    break;
            }
            Response.Write(s);
        }

        private static string ExpandTreeViewToNode()
        {
            string str1 = WebUtil.GetQueryString("root");
            string str2 = WebUtil.GetQueryString("id");
            string queryString = WebUtil.GetQueryString("la");
            if (str2.IndexOf('_') >= 0)
                str2 = StringUtil.Mid(str2, str2.LastIndexOf('_') + 1);
            if (str1.IndexOf('_') >= 0)
                str1 = StringUtil.Mid(str1, str1.LastIndexOf('_') + 1);
            if (str2.Length > 0 && str1.Length > 0)
            {
                Language language = Language.Parse(queryString);
                Item folder = Client.ContentDatabase.GetItem(ShortID.DecodeID(str2), language);
                Item root = Client.ContentDatabase.GetItem(ShortID.DecodeID(str1), language);
                if (folder != null && root != null)
                    return GetTree(folder, root).RenderTree(false);
            }
            return string.Empty;
        }

        private static string GetTreeViewChildren()
        {
            string queryString1 = WebUtil.GetQueryString("id");
            string queryString2 = WebUtil.GetQueryString("la");
            if (string.IsNullOrEmpty(queryString1))
                return string.Empty;
            Language language = Language.Parse(queryString2);
            Item folder = Client.ContentDatabase.GetItem(ShortID.DecodeID(queryString1), language);
            if (folder == null)
                return string.Empty;
            Item rootItem = folder.Database.GetRootItem(language);
            if (rootItem == null)
                return string.Empty;
            return GetTree(folder, rootItem).RenderChildNodes(folder.ID);
        }

        private static CustomTree GetTree(Item folder, Item root)
        {
            Assert.IsNotNull(folder, "folder is null");
            Assert.IsNotNull(root, "root is null");
            CustomTree tree = new CustomTree();
            tree.ID = WebUtil.GetSafeQueryString("treeid");
            tree.FolderItem = folder;
            tree.RootItem = root;
            tree.DataContext = new DataContext()
            {
                DataViewName = "Master"
            };
            return tree;
        }

        private string DesignTimeConvert()
        {
            string queryString = WebUtil.GetQueryString("mode");
            string body = StringUtil.GetString(Request.Form["html"]);
            string str;
            if (queryString == "HTML")
            {
                str = RuntimeHtml.Convert(body, Settings.HtmlEditor.SupportWebControls);
            }
            else
            {
                NameValueCollection querystring = new NameValueCollection(1) { { "sc_live", "0" } };
                string controlName = string.Empty;
                if (Settings.HtmlEditor.SupportWebControls)
                    controlName = "control:IDEHtmlEditorControl";
                str = DesignTimeHtml.Convert(body, controlName, querystring);
            }
            return "<?xml:namespace prefix = sc />" + str;
        }

        private string GetContextualTabs()
        {
            string queryString = WebUtil.GetQueryString("parameters");
            if (string.IsNullOrEmpty(queryString) || queryString.IndexOf("&fld=", StringComparison.InvariantCulture) < 0)
                return string.Empty;
            return GetFieldContextualTab(queryString);
        }

        private string GetFieldContextualTab(string parameters)
        {
            Assert.ArgumentNotNull(parameters, "parameters");
            int num = parameters.IndexOf("&fld=", StringComparison.InvariantCulture);
            ItemUri uri = ItemUri.Parse(StringUtil.Left(parameters, num));
            if (uri == null)
                return string.Empty;
            Item obj = Database.GetItem(uri);
            if (obj == null)
                return string.Empty;
            NameValueCollection urlParameters = WebUtil.ParseUrlParameters(StringUtil.Mid(parameters, num));
            string index = StringUtil.GetString(urlParameters["fld"]);
            string str = StringUtil.GetString(urlParameters["ctl"]);
            Field field = obj.Fields[index];
            if (field == null)
                return string.Empty;
            TemplateField templateField = field.GetTemplateField();
            if (templateField == null)
                return string.Empty;
            string fieldType = StringUtil.GetString(templateField.TypeKey, "text");
            Item fieldTypeItem = FieldTypeManager.GetFieldTypeItem(fieldType);
            if (fieldTypeItem == null)
                return string.Empty;
            Database database = global::Sitecore.Context.Database;
            if (database == null)
                return string.Empty;
            Item child;
            if (fieldType == "rich text")
            {
                string queryString = WebUtil.GetQueryString("mo", "Editor");
                string path = StringUtil.GetString(templateField.Source, queryString == "IDE" ? "/sitecore/system/Settings/Html Editor Profiles/Rich Text IDE" : Settings.HtmlEditor.DefaultProfile) + "/Ribbon";
                child = database.GetItem(path);
            }
            else
                child = fieldTypeItem.Children["Ribbon"];
            if (child == null)
                return string.Empty;
            Ribbon ribbon1 = new Ribbon();
            ribbon1.ID = "Ribbon";
            Ribbon ribbon2 = ribbon1;
            CommandContext commandContext = new CommandContext(obj);
            ribbon2.CommandContext = commandContext;
            commandContext.Parameters["FieldID"] = index;
            commandContext.Parameters["ControlID"] = str;
            string navigator;
            string strips;
            ribbon2.Render(child, true, out navigator, out strips);
            Response.Write("{ \"navigator\": " + StringUtil.EscapeJavascriptString(navigator) + ", \"strips\": " + StringUtil.EscapeJavascriptString(strips) + " }");
            return string.Empty;
        }

        private string GetTreeViewContent()
        {
            Response.ContentType = "text/xml";
            ItemUri queryString = ItemUri.ParseQueryString();
            if (queryString == null)
                return string.Empty;
            Item parent = Database.GetItem(queryString);
            if (parent == null)
                return string.Empty;
            Type typeInfo = ReflectionUtil.GetTypeInfo(WebUtil.GetQueryString("typ"));
            if (typeInfo == null)
                return string.Empty;
            TreeviewSource treeviewSource = ReflectionUtil.CreateObject(typeInfo) as TreeviewSource;
            if (treeviewSource == null)
                return string.Empty;
            TreeView treeview = new TreeView();
            treeviewSource.Render(treeview, parent);
            return treeview.GetXml();
        }
    }
}
