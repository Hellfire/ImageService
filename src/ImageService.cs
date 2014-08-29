using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web.Configuration;

namespace Fpi.ImageService
{
	public static class ImageService
	{

		public static ImageServiceSection ConfigSectionSettings;

		#region Provider Specific

		private static readonly object Lock = new object();
		private static ImageProviderCollection _providers;
		private static ImageProvider _defaultProvider;
		private static ImageServiceSection _section;

		/// <summary>
		/// Gets or sets the config section.
		/// </summary>
		/// <value>The config section.</value>
		public static ImageServiceSection ConfigSection
		{
			get
			{
				// Delayed loading
				if (_section == null) {
					_section = ConfigSectionSettings ?? (ImageServiceSection)ConfigurationManager.GetSection(ConfigurationSectionName.ImageService);
				}

				return _section;
			}
			set
			{
				_section = value;
			}
		}

		/// <summary>
		/// Gets or sets the provider.
		/// </summary>
		/// <value>The provider.</value>
		public static ImageProvider Provider
		{
			get
			{
				if (_defaultProvider == null)
					LoadProviders();

				return _defaultProvider;
			}
			set
			{
				_defaultProvider = value;
			}
		}

		/// <summary>
		/// Gets or sets the providers.
		/// </summary>
		/// <value>The providers.</value>
		public static ImageProviderCollection Providers
		{
			get
			{
				if (_providers == null)
					LoadProviders();

				return _providers;
			}
			set
			{
				_providers = value;
			}
		}

		/// <summary>
		/// Gets the provider names.
		/// </summary>
		/// <returns></returns>
		public static string[] GetProviderNames()
		{
			if (Providers == null)
				return new string[] { };

			int providerCount = Providers.Count;
			string[] providerNames = new string[providerCount];

			int i = 0;
			foreach (ImageProvider provider in Providers) {
				providerNames[i] = provider.Name;
				i++;
			}
			return providerNames;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <param name="providerName">Name of the provider.</param>
		/// <returns></returns>
		public static ImageProvider GetInstance(string providerName)
		{
			//ensure load
			LoadProviders();

			//ensure it's instanced
			if (String.IsNullOrEmpty(providerName) || String.IsNullOrEmpty(providerName.Trim()))
				return _defaultProvider;

			ImageProvider provider = _providers[providerName];
			if (provider != null)
				return provider;

			throw new ArgumentException("No provider is defined with the name " + providerName, "providerName");
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns></returns>
		private static ImageProvider GetInstance()
		{
			return GetInstance(null);
		}

		/// <summary>
		/// Loads the providers.
		/// </summary>
		public static void LoadProviders()
		{
			if (_defaultProvider != null) return;
			lock (Lock) {
				if (_defaultProvider != null) return;

				if (ConfigSection == null)
					throw new ConfigurationErrorsException("Can't find the imageService section in your application's config");

				_providers = new ImageProviderCollection();
				foreach (ProviderSettings settings in ConfigSection.Providers) {
					_providers.Add(ProvidersHelper.InstantiateProvider(settings, typeof(ImageProvider)));
				}

				_defaultProvider = _providers[ConfigSection.DefaultProvider];

				if (_defaultProvider == null)
					throw new ProviderException("Unable to load default ImageProvider");
			}
		}

		/// <summary>
		/// Adds the provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public static void AddProvider(ImageProvider provider)
		{
			if (_providers == null)
				_providers = new ImageProviderCollection();
			_providers.Add(provider);
		}

		#endregion

		#region Image Service

		/// <summary>
		/// Adds the image to the repository and returns the UID assigned to the image.
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static Object SaveImage(Image image)
		{
			return GetInstance().SaveImage(image);
		}

		/// <summary>
		/// Adds the image to the repository identified by <i>providerName</i> and returns the UID assigned to the image.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="providerName"></param>
		/// <returns></returns>
		public static Object SaveImage(Image image, String providerName)
		{
			return GetInstance(providerName).SaveImage(image);
		}

		/// <summary>
		/// Adds the image to the repository and returns the UID assigned to the image.
		/// </summary>
		/// <returns></returns>
		public static Object SaveImage(String filename)
		{
			return GetInstance().SaveImage(filename);
		}

		/// <summary>
		/// Adds the image to the repository identified by <i>providerName</i> and returns the UID assigned to the image.
		/// </summary>
		/// <returns></returns>
		public static Object SaveImage(String filename, String providerName)
		{
			return GetInstance(providerName).SaveImage(filename);
		}

		/// <summary>
		/// Saves the image identified by <i>imageUID</i> in the repository.  If the image does not exist, it is added.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		public static void SaveImage(Object imageUID, Image image)
		{
			GetInstance().SaveImage(imageUID, image);
		}

		/// <summary>
		/// Saves the image identified by <i>imageUID</i> in the repository identified by <i>providerName</i>.  If the image does not exist, it is added.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="image"></param>
		/// <param name="providerName"></param>
		/// <returns></returns>
		public static void SaveImage(Object imageUID, Image image, String providerName)
		{
			GetInstance(providerName).SaveImage(imageUID, image);
		}


		/// <summary>
		/// Saves the image identified by <i>imageUID</i> in the repository.  If the image does not exist, it is added.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static void SaveImage(Object imageUID, String filename)
		{
			GetInstance().SaveImage(imageUID, filename);
		}

		/// <summary>
		/// Saves the image identified by <i>imageUID</i> in the repository identified by <i>providerName</i>.  If the image does not exist, it is added.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="filename"></param>
		/// <param name="providerName"></param>
		/// <returns></returns>
		public static void SaveImage(Object imageUID, String filename, String providerName)
		{
			GetInstance(providerName).SaveImage(imageUID, filename);
		}

		/// <summary>
		/// Returns the image identified by <i>imageKey</i> from the repository.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <returns></returns>
		public static Image GetImage(Object imageUID)
		{
			return GetInstance().GetImage(imageUID);
		}

		/// <summary>
		/// Returns the image identified by <i>imageKey</i> from the repository identified by <i>providerName</i>.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="providerName"></param>
		/// <returns></returns>
		public static Image GetImage(Object imageUID, String providerName)
		{
			return GetInstance(providerName).GetImage(imageUID);
		}

		/// <summary>
		/// Returns the image identified by <i>imageUID</i> resized to fit within <i>maxWidth</i> and <i>maxHeight</i> while preserving the aspect ratio.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="maxWidth"></param>
		/// <param name="maxHeight"></param>
		/// <returns></returns>
		public static Image GetThumbnailImage(Object imageUID, Int32 maxWidth, Int32 maxHeight)
		{
			return GetInstance().GetThumbnailImage(imageUID, maxWidth, maxHeight);
		}

		/// <summary>
		/// Returns the image identified by <i>imageUID</i> resized to fit within <i>maxWidth</i> and <i>maxHeight</i> while preserving the aspect ratio.
		/// </summary>
		/// <param name="imageUID">the image identifier</param>
		/// <param name="maxWidth">maximum width</param>
		/// <param name="maxHeight">maximum height</param>
		/// <param name="providerName">the provider to get the image from</param>
		/// <returns></returns>
		public static Image GetThumbnailImage(Object imageUID, Int32 maxWidth, Int32 maxHeight, String providerName)
		{
			return GetInstance(providerName).GetThumbnailImage(imageUID, maxWidth, maxHeight);
		}

		/// <summary>
		/// Deletes the image identified by <i>imageUID</i> from the repository.
		/// </summary>
		/// <param name="imageUID"></param>
		public static void DeleteImage(Object imageUID)
		{
			GetInstance().DeleteImage(imageUID);
		}

		/// <summary>
		/// Deletes the image identified by <i>imageUID</i> from the repository identified by <i>providerName</i>.
		/// </summary>
		/// <param name="imageUID"></param>
		/// <param name="providerName"></param>
		public static void DeleteImage(Object imageUID, String providerName)
		{
			GetInstance(providerName).DeleteImage(imageUID);
		}

		#endregion

		#region Helper Methods

		public static ImageFormat GetImageFormat(String format)
		{
			switch (format.ToLower()) {
				case "bmp":
				case ".bmp":
					return ImageFormat.Bmp;
				case "gif":
				case ".gif":
					return ImageFormat.Gif;
				case "jpg":
				case ".jpg":
				case "jpeg":
				case ".jpeg":
					return ImageFormat.Jpeg;
				case "png":
				case ".png":
					return ImageFormat.Png;
				default:
					throw new ApplicationException(String.Format("Format '{0}' is unsupported", format));
			}
		}

		public static String GetExtension(ImageFormat imageFormat)
		{
			if (ImageFormat.Bmp == imageFormat) {
				return ".bmp";
			}
			if (ImageFormat.Gif == imageFormat) {
				return ".gif";
			}
			if (ImageFormat.Jpeg == imageFormat) {
				return ".jpg";
			}
			if (ImageFormat.Png == imageFormat) {
				return ".png";
			}

			return null;
		}

		public static String GetContentType(String extension)
		{
			return GetContentType(extension, "application/octet-stream");
		}

		public static String GetContentType(String extension, String defaultContentType)
		{
			using (Microsoft.Win32.RegistryKey registry = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension)) {
				if (registry == null) {
					return defaultContentType;
				}

				String contentType = registry.GetValue("Content Type") as String;
				return String.IsNullOrEmpty(contentType) ? defaultContentType : contentType;
			}
		}

		public static Image CreateThumbnailImage(String filename, Int32 maxWidth, Int32 maxHeight)
		{
			using (Image image = Image.FromFile(filename)) {
				Size newSize = GetThumbnailDimensions(maxWidth, maxHeight, image);
				return CreateThumbnailImage(image, newSize);
			}
		}

		public static Image CreateThumbnailImage(String filename, Size maxSize)
		{
			using (Image image = Image.FromFile(filename)) {
				Size newSize = GetThumbnailDimensions(maxSize.Width, maxSize.Height, image);
				return CreateThumbnailImage(image, newSize);
			}
		}

		public static Image CreateThumbnailImage(Image source, Size size)
		{
			return CreateThumbnailImage(source, size.Width, size.Height);
		}

		public static Image CreateThumbnailImage(Image source, Int32 newWidth, Int32 newHeight)
		{
			if (newWidth <= 0) {
				throw new ImageProviderException("newWidth must be greater than zero.", new ArgumentException("Must be greater than zero.", "newWidth"));
			}

			if (newHeight <= 0) {
				throw new ImageProviderException("newHeight must be greater than zero.", new ArgumentException("Must be greater than zero.", "newHeight"));
			}

			Image resized = new Bitmap(newWidth, newHeight);
			using (Graphics graphics = Graphics.FromImage(resized)) {
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.DrawImage(source, 0, 0, newWidth, newHeight);
			}
			return resized;
		}

		public static Size GetThumbnailDimensions(Int32 maxWidth, Int32 maxHeight, Image bitmap)
		{
			return GetThumbnailDimensions(maxWidth, maxHeight, bitmap.Width, bitmap.Height);
		}

		public static Size GetThumbnailDimensions(Int32 maxWidth, Int32 maxHeight, Int32 actualWidth, Int32 actualHeight)
		{
			float width = Convert.ToSingle(actualWidth);
			float height = Convert.ToSingle(actualHeight);
			float maxWidthF = Convert.ToSingle(maxWidth);
			float maxHeightF = Convert.ToSingle(maxHeight);

			// If the source images is already smaller than the target size, or one of maxWidth and maxHeight are zero
			if ((width <= maxWidthF && height <= maxHeightF) || (maxWidthF <= 0 && actualHeight < maxHeight) || (maxHeight <= 0 && actualWidth < maxWidth)) {
				return new Size(actualWidth, actualHeight);
			}

			// Otherwise we check to see if we can shrink it width first.
			float multiplier = maxWidthF / width;
			// If maxWidth is 0 then we use height regardless
			if (maxWidth > 0) {
				// If maxHeight is 0 then we use width regardless
				if ((height * multiplier) <= maxHeightF || maxHeight <= 0) {
					height *= multiplier;
					return new Size(maxWidth, Convert.ToInt32(height));
				}
			}

			// Since we can use max width we use the max height
			multiplier = maxHeightF / height;
			width *= multiplier;
			return new Size(Convert.ToInt32(width), maxHeight);
		}
		#endregion

	}
}
