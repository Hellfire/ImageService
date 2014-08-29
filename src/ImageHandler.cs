using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Fpi.ImageService
{
	public class ImageHandler : IHttpHandler
	{
		bool IHttpHandler.IsReusable
		{
			get
			{
				return true;
			}
		}

		public virtual void ProcessRequest(HttpContext context)
		{
			Guid imageUID = GetGuid(context, "uid", Guid.Empty);
			Int32 maxWidth = GetInt32(context, "mw", 0);
			Int32 maxHeight = GetInt32(context, "mh", 0);

			if (maxWidth == 0 && maxHeight == 0)
			{
				// Send the original image.
				using (Image image = ImageService.GetImage(imageUID))
				{
					if (image == null)
					{
						throw new HttpException(404, "File Not Found");
					}

					context.Response.ContentType = "image/png";
					WriteImageToStream(context.Response.OutputStream, image, ImageService.Provider.ImageFormat);
				}
			}
			else
			{
				// Send the resized image.
				using (Image image = ImageService.GetThumbnailImage(imageUID, maxWidth, maxHeight))
				{
					if (image == null)
					{
						throw new HttpException(404, "File Not Found");
					}

					context.Response.ContentType = "image/png";
					WriteImageToStream(context.Response.OutputStream, image, ImageService.Provider.ImageFormat);
				}
			}
		}

		private void WriteImageToStream(Stream stream, Image image, ImageFormat imageFormat)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				image.Save(memoryStream, imageFormat);
				memoryStream.WriteTo(stream);
			}
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		Int32 GetInt32(HttpContext context, String name, Int32 defaultValue)
		{
			if (null == context)
				return defaultValue;

			return null == context.Request.QueryString[name] ? defaultValue : Int32.Parse(context.Request.QueryString[name]);
		}

		Guid GetGuid(HttpContext context, String name, Guid defaultValue)
		{
			if (null == context)
				return defaultValue;

			return null == context.Request.QueryString[name] ? defaultValue : new Guid(context.Request.QueryString[name]);
		}

	}
}
