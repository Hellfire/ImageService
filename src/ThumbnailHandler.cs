using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Fpi.ImageService
{
	public class ThumbnailHandler : IHttpHandler
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
			String imageURL = GetString(context, "u", String.Empty);
			Int32 maxWidth = GetInt32(context, "w", 0);
			Int32 maxHeight = GetInt32(context, "h", 0);

			String filename = context.Server.MapPath(imageURL);
			String contentType = ImageService.GetContentType(Path.GetExtension(filename));
			ImageFormat imageFormat = ImageService.GetImageFormat(Path.GetExtension(filename));

			if (maxWidth == 0 && maxHeight == 0)
			{
				// Send the original image.
				if (File.Exists(filename))
				{
					context.Response.ContentType = contentType;
					context.Response.WriteFile(filename);
				}
				else
				{
					throw new HttpException(404, "File Not Found");
				}
			}
			else
			{
				// Send the resized image.
				using (Image image = ImageService.CreateThumbnailImage(filename, maxWidth, maxHeight))
				{
					if (image == null)
					{
						throw new HttpException(404, "File Not Found");
					}

					context.Response.ContentType = contentType;
					WriteImageToStream(context.Response.OutputStream, image, imageFormat);
				}
			}

			HttpCachePolicy cachePolicy = context.Response.Cache;
			cachePolicy.SetCacheability(HttpCacheability.Public);
			cachePolicy.VaryByParams["u"] = true;
			cachePolicy.VaryByParams["w"] = true;
			cachePolicy.VaryByParams["k"] = true;
			cachePolicy.SetOmitVaryStar(true);
			cachePolicy.SetExpires(DateTime.Now + TimeSpan.FromDays(1));
			cachePolicy.SetValidUntilExpires(true);
			cachePolicy.SetLastModified(File.GetLastWriteTime(filename));

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

		String GetString(HttpContext context, String name, String defaultValue)
		{
			if (null == context)
				return defaultValue;

			return context.Request.QueryString[name] ?? defaultValue;
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
