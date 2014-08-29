using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;


namespace Fpi.ImageService
{
	public class CachedFileSystemImageProvider : FileSystemImageProvider
	{
		public override Image GetThumbnailImage(Object imageUID, int maxWidth, int maxHeight)
		{
			String filename = GetThumbnailFilename(imageUID, maxWidth, maxHeight);
			if (File.Exists(filename))
				return Image.FromFile(filename);

			using (Image image = base.GetThumbnailImage(imageUID, maxWidth, maxHeight)) {
				if (image == null) {
					return null;
				}
				WriteImageToFileSystem(filename, image, ImageFormat);
			}
			return Image.FromFile(filename);
		}

		public virtual String GetThumbnailFilename(Object imageUID, Int32 width, Int32 height)
		{
			ThrowIfInvalidImageUID(imageUID);

			String key = Regex.Replace(imageUID.ToString(), "[{}-]", String.Empty, RegexOptions.IgnoreCase);
			String extension = ImageService.GetExtension(ImageFormat);
			String directory = GetDirectory(imageUID);

			return Path.Combine(directory, String.Format("{0}-{1}x{2}{3}", key, width, height, extension));
		}

	}
}
