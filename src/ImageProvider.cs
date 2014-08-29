using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fpi.ImageService
{
	public abstract class ImageProvider : ProviderBase, IDisposable
	{
		#region Provider Specific

		public override void Initialize(String name, NameValueCollection config)
		{
			base.Initialize(name, config);

			if (config.Count > 0)
			{
				throw new ConfigurationErrorsException(String.Format("Unrecognized configuration property: {0}", config.GetKey(0)));
			}
		}

		/// <summary>
		/// Applies the config.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="propertyName">Name of the config property to retrieve.</param>
		/// <param name="propertyValue">The parameter that will receive the value value.</param>
		protected void SetConfigOption(NameValueCollection config, String propertyName, ref String propertyValue)
		{
			if (config != null && config[propertyName] != null)
			{
				propertyValue = config[propertyName];
				config.Remove(propertyName);
			}
		}

		/// <summary>
		/// Applies the config.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="propertyName">Name of the config property to retrieve.</param>
		/// <param name="propertyValue">The parameter that will receive the value value.</param>
		protected void SetConfigOption(NameValueCollection config, String propertyName, ref Boolean propertyValue)
		{
			if (config != null && config[propertyName] != null)
			{
				propertyValue = Convert.ToBoolean(config[propertyName]);
				config.Remove(propertyName);
			}
		}

		/// <summary>
		/// Applies the config.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="propertyName">Name of the config property to retrieve.</param>
		/// <param name="propertyValue">The parameter that will receive the value value.</param>
		protected void SetConfigOption(NameValueCollection config, String propertyName, ref ImageFormat propertyValue)
		{
			if (config != null && config[propertyName] != null)
			{
				propertyValue = ImageService.GetImageFormat(config[propertyName]);
				config.Remove(propertyName);
			}
		}

		#endregion

		public abstract ImageFormat ImageFormat
		{
			get;
		}

		/// <summary>
		/// Adds the image to the repository and returns the key assigned to it.
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public abstract Object SaveImage(Image image);

		public abstract object SaveImage(String filename);

		/// <summary>
		/// Updates the image in the repository.  If the image does not already exist it will be created.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		public abstract void SaveImage(Object imageUID, Image image);

		public virtual void SaveImage(Object imageUID, String filename)
		{
			using (Image image = Image.FromFile(filename))
			{
				SaveImage(imageUID, image);
			}
		}

		/// <summary>
		/// Returns the original image identified by <i>imageKey</i>, <i>imageType</i> and <i>imageUID</i>
		/// </summary>
		/// <param name="imageUID"></param>
		/// <returns></returns>
		public abstract Image GetImage(Object imageUID);

		/// <summary>
		/// Returns the image identified by <i>imageUID</i> resized smaller to fit <i>maxWidth</i> and <i>maxHeight</i>.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="maxWidth"></param>
		/// <param name="maxHeight"></param>
		/// <returns></returns>
		/// <remarks>
		/// If the image is already smaller than <i>maxWidth</i> and <i>maxHeight</i> then it will be returned unmodified.
		/// </remarks>
		public virtual Image GetThumbnailImage(Object imageUID, Int32 maxWidth, Int32 maxHeight)
		{
			using (Image image = GetImage(imageUID))
			{
				if (image != null)
				{
					Size newSize = ImageService.GetThumbnailDimensions(maxWidth, maxHeight, image);
					return ImageService.CreateThumbnailImage(image, newSize);
				}
			}
			return null;
		}

		/// <summary>
		/// Deletes the image from the data store.
		/// </summary>
		/// <param name="imageUID"></param>
		public abstract void DeleteImage(Object imageUID);


		#region IDisposable Members

		private Boolean _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				// if this is a dispose call dispose on all state you
				// hold, and take yourself off the Finalization queue.
				if (disposing)
				{

				}

				_disposed = true;
			}
		}

		// finalizer simply calls Dispose(false)
		~ImageProvider()
		{
			Dispose(false);
		}

		#endregion
	}
}
