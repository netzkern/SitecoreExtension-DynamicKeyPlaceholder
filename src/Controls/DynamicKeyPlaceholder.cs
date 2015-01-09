namespace SitecoreExtension.DynamicKeyPlaceholder.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Web.UI.WebControls;
    using Sitecore.Layouts;
    using Sitecore.Web.UI;
    using Sitecore.Common;
    using System.Web.UI;

    public class DynamicKeyPlaceholder : WebControl, IExpandable
    {
        protected string _key = Placeholder.DefaultPlaceholderKey;
        protected string _dynamicKey = null;
        protected Placeholder _placeholder;

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value.ToLower();
                if (_placeholder != null)
                {
                    // need to reset child controls if key is changed after placeholder was initialized.
                    this.Controls.Clear();
                    CreateChildControls();
                }
            }
        }

        protected string DynamicKey
        {
            get
            {
                _dynamicKey = _key;

                //find the last placeholder processed, will help us find our parent
                Stack<Placeholder> stack = Switcher<Placeholder, PlaceholderSwitcher>.GetStack(false);
                if (stack.Count > 0)
                {
                    Placeholder current = stack.Peek();

                    //find the rendering reference we are contained in
                    var renderings =
                        Sitecore.Context.Page.Renderings.Where(
                            rendering =>
                                (rendering.Placeholder == current.ContextKey || rendering.Placeholder == current.Key)
                                && rendering.AddedToPage);
                    if (renderings.Any())
                    {
                        //last one added to page defines our parent
                        var thisRendering = renderings.Last();
                        _dynamicKey = _key + thisRendering.UniqueId;
                    }
                }
                else
                {
                    // Stack is empty -> This can happen inside of a listview... let's check.
                    var editFrame = this.Parent;
                    var closestListView = GetAncestorOfType(editFrame, "System.Web.UI.WebControls.ListViewDataItem");
                    if (closestListView != null)
                    {
                        // Ok this is inside of a listview, here things work a bit different.
                        // Since we still need a unique placeholderkey, we will just use unique id of our editFrame
                        _dynamicKey = _key + "{{" + editFrame.UniqueID.Replace("$", "") + "}}";
                    }
                }
                return _dynamicKey;
            }
        }

        public static Control GetAncestorOfType(Control ctrl, string typeName)
        {
            if (ctrl.Parent == null)
            {
                return null;
            }
            if (ctrl.GetType().ToString().Equals(typeName))
            {
                return ctrl;
            }
            return GetAncestorOfType(ctrl.Parent, typeName);
        }

        protected override void CreateChildControls()
        {
            Sitecore.Diagnostics.Tracer.Debug("DynamicKeyPlaceholder: Adding dynamic placeholder with Key " + DynamicKey);
            _placeholder = new Placeholder();
            _placeholder.Key = this.DynamicKey;
            this.Controls.Add(_placeholder);
            _placeholder.Expand();
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            base.RenderChildren(output);
        }

        #region IExpandable Members

        public void Expand()
        {
            this.EnsureChildControls();
        }

        #endregion
    }
}
