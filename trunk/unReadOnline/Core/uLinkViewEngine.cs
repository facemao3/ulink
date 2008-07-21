using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;

namespace unReadOnline.Core
{
    public class uLinkViewEngine : WebFormViewEngine
    {
        #region Private members

        IViewLocator _viewLocator = null;

        #endregion

        #region IViewEngine Members : RenderView()

        protected override void RenderView(ViewContext viewContext)
        {
            base.ViewLocator = this.ViewLocator;
            base.RenderView(viewContext);
        }

        #endregion

        #region Public properties : ViewLocator

        public IViewLocator ViewLocator
        {
            get
            {
                if (this._viewLocator == null)
                {
                    this._viewLocator = new uLinkViewLocator();
                }
                return this._viewLocator;
            }
            set
            {
                this._viewLocator = value;
            }
        }

        #endregion

    }
}
