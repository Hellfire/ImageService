using System;
using System.Web;

namespace Fpi.ImageService
{

	public class CachedImageHandler : ImageHandler
	{
		public override void ProcessRequest(HttpContext context)
		{
			base.ProcessRequest(context);

			HttpCachePolicy cachePolicy = context.Response.Cache;
			cachePolicy.SetCacheability(HttpCacheability.Public);
			cachePolicy.VaryByParams["uid"] = true;
			cachePolicy.VaryByParams["mw"] = true;
			cachePolicy.VaryByParams["mh"] = true;
			cachePolicy.SetOmitVaryStar(true);
			cachePolicy.SetExpires(DateTime.Now + TimeSpan.FromDays(1));
			cachePolicy.SetValidUntilExpires(true);
			cachePolicy.SetLastModified(DateTime.Now);

		}
	}
}

