using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls.Data;
using Sitecore.Web.UI.Sheer;
using Sitecore.XA.Foundation.TokenResolution;
using Page = System.Web.UI.Page;

namespace Sitecore.Playground.XA.WorkaroundFactory.LookupNameLookupValue
{
    [UsedImplicitly]
    public class LookupNameLookupValue12 : Input
    {
        public string FieldName
        {
            get
            {
                return GetViewStateString("FieldName");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("FieldName", value);
            }
        }

        public string ItemId
        {
            get
            {
                return GetViewStateString("ItemID");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("ItemID", value);
            }
        }

        public string Source
        {
            get
            {
                return GetViewStateString("Source");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("Source", value);
            }
        }

        public LookupNameLookupValue12()
        {
            Activation = true;
            Class = "scContentControl";
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (Sitecore.Context.ClientPage.IsEvent)
            {
                LoadValue();
            }
            else
            {
                BuildControl();
            }
        }

        protected virtual void LoadValue()
        {
            if (!ReadOnly && !Disabled)
            {
                NameValueCollection form;
                Page handler = HttpContext.Current.Handler as Page;
                form = handler != null ? handler.Request.Form : new NameValueCollection();
                UrlString map = new UrlString();
                foreach (string id in form.Keys)
                {
                    if ((!string.IsNullOrEmpty(id) && id.StartsWith(ID + "_Param")) && !id.EndsWith("_value"))
                    {
                        string key = form[id];
                        string value = form[id + "_value"];
                        if (!string.IsNullOrEmpty(key))
                        {
                            map[key] = value ?? string.Empty;
                        }
                    }
                }
                if (Value != map.ToString())
                {
                    Value = map.ToString();
                    SetModified();
                }
            }
        }

        protected override void SetModified()
        {
            base.SetModified();
            if (TrackModified)
            {
                Sitecore.Context.ClientPage.Modified = true;
            }
        }

        protected virtual void BuildControl()
        {
            UrlString str = new UrlString(Value);
            foreach (string str2 in str.Parameters.Keys)
            {
                if (str2.Length > 0)
                {
                    Controls.Add(new LiteralControl(BuildParameterKeyValue(str2, str.Parameters[str2])));
                }
            }
            Controls.Add(new LiteralControl(BuildParameterKeyValue(string.Empty, string.Empty)));
        }

        protected virtual string BuildParameterKeyValue(string key, string value)
        {
            Assert.ArgumentNotNull(key, "key");
            Assert.ArgumentNotNull(value, "value");

            string uniqueId = GetUniqueID(ID + "_Param");
            Sitecore.Context.ClientPage.ServerProperties[ID + "_LastParameterID"] = uniqueId;
            string clientEvent = Sitecore.Context.ClientPage.GetClientEvent(ID + ".ParameterChange");
            string readOnly = ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string disabled = Disabled ? " disabled=\"disabled\"" : string.Empty;
            string keyHtmlControl = GetKeyHtmlControl(uniqueId, StringUtil.EscapeQuote(HttpUtility.UrlDecode(key)), readOnly, disabled, clientEvent);
            string valueHtmlControl = GetValueHtmlControl(uniqueId, StringUtil.EscapeQuote(HttpUtility.UrlDecode(value)));
            return $"<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\"><tr><td width=\"50%\">{keyHtmlControl}</td><td width=\"50%\">{valueHtmlControl}</td></tr></table>";
        }

        protected virtual string GetKeyHtmlControl(string id, string key, string readOnly, string disabled, string clientEvent)
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());
            Item current = Sitecore.Context.ContentDatabase.GetItem(ItemId);
            Item[] items = LookupSources.GetItems(current, SourcePart(0, current));
            writer.Write("<select id=\"" + id + "\" name=\"" + id + "\"" + readOnly + disabled + " style=\"width:100%\" onchange=\"" + clientEvent + "\"" + GetControlAttributes() + ">");
            writer.Write("<option" + (string.IsNullOrEmpty(key) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");
            foreach (Item item2 in items)
            {
                string itemHeader = GetItemHeader(item2);
                bool flag = item2.ID.ToString() == key;
                writer.Write("<option value=\"" + item2.ID + "\"" + (flag ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
            }
            writer.Write("</select>");
            return writer.InnerWriter.ToString();
        }

        protected virtual string GetValueHtmlControl(string id, string value)
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());
            Item current = Sitecore.Context.ContentDatabase.GetItem(ItemId);
            Item[] items = LookupSources.GetItems(current, SourcePart(1, current));
            writer.Write("<select id=\"" + id + "_value\" name=\"" + id + "_value\" style=\"width:100%\"" + GetControlAttributes() + ">");
            writer.Write("<option" + (string.IsNullOrEmpty(value) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");
            foreach (Item item2 in items)
            {
                string itemHeader = GetItemHeader(item2);
                bool flag = item2.ID.ToString() == value;
                writer.Write("<option value=\"" + item2.ID + "\"" + (flag ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
            }
            writer.Write("</select>");
            return writer.InnerWriter.ToString();
        }

        protected virtual string GetItemHeader(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            string str2 = StringUtil.GetString(new[] { FieldName });
            if (str2.StartsWith("@"))
            {
                return item[str2.Substring(1)];
            }
            if (str2.Length > 0)
            {
                return item[FieldName];
            }
            return item.DisplayName;
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            SetWidthAndHeightStyle();
            output.Write("<div" + ControlAttributes + ">");
            RenderChildren(output);
            output.Write("</div>");
        }

        [UsedImplicitly]
        protected virtual void ParameterChange()
        {
            ClientPage clientPage = Sitecore.Context.ClientPage;
            if (clientPage.ClientRequest.Source == StringUtil.GetString(clientPage.ServerProperties[ID + "_LastParameterID"]))
            {
                string str = clientPage.ClientRequest.Form[clientPage.ClientRequest.Source];
                if (!string.IsNullOrEmpty(str))
                {
                    string str2 = BuildParameterKeyValue(string.Empty, string.Empty);
                    clientPage.ClientResponse.Insert(ID, "beforeEnd", str2);
                }
            }
            NameValueCollection form = null;
            Page handler = HttpContext.Current.Handler as Page;
            if (handler != null)
            {
                form = handler.Request.Form;
            }
            if (form != null)
            {
                clientPage.ClientResponse.SetReturnValue(true);
            }
        }

        protected virtual string SourcePart(int index, Item item)
        {
            string[] sources = Source.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            if (sources.Length > index)
            {
                string source = sources[index].Trim();
                if (source.Contains("$"))
                {
                    source = TokenResolver.Resolve(source, item);
                }
                return source;
            }
            return "";
        }
    }
}