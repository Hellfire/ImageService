using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fpi.ImageService
{
	[Designer("System.Web.UI.Design.WebControls.PreviewControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("ImageUrl"), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class ThumbnailImage : Image
	{
		protected virtual String FormatUrl(String url)
		{
			return String.Format("{0}?mw={1}&mh={2}&uid={3}", url, MaxWidth, MaxHeight, ImageUID);
		}

		[DefaultValue("~/ImageSizer.ashx"), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty]
		public override string ImageUrl
		{
			get
			{
				String value = ViewState["ImageUrl"] as String;
				if (value == null)
				{
					return FormatUrl("~/ImageService.ashx");
				}
				return FormatUrl(value);
			}
			set
			{
				ViewState["ImageUrl"] = value;
			}
		}

		[DefaultValue(0), Themeable(false)]
		public Int32 MaxWidth
		{
			get
			{
				Object value = ViewState["MaxWidth"];
				if (null == value)
				{
					return 0;
				}
				return (Int32) value;
			}
			set
			{
				if ( value < 0 )
				{
					ViewState["MaxWidth"] = 0;
				}
				else
				{
					ViewState["MaxWidth"] = value;
				}
			}
		}

		[DefaultValue(0), Themeable(false)]
		public Int32 MaxHeight
		{
			get
			{
				Object value = ViewState["MaxHeight"];
				if (null == value)
				{
					return 0;
				}
				return (Int32)value;
			}
			set
			{
				if (value < 0)
				{
					ViewState["MaxHeight"] = 0;
				}
				else
				{
					ViewState["MaxHeight"] = value;
				}
			}
		}

		[Themeable(false)]
		public Guid ImageUID
		{
			get
			{
				Object value = ViewState["ImageUID"];
				if (null == value)
				{
					return Guid.Empty;
				}
				return (Guid)value;
			}
			set
			{
				ViewState["ImageUID"] = value;
			}
		}

	}
}
