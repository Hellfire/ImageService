using System;
using System.Collections.Specialized;
using System.Drawing;
using NUnit.Framework;

namespace Fpi.ImageService.Tests
{
	/// <summary>
	/// Summary description for ImageProviderTest
	/// </summary>
	[TestFixture]
	public abstract class ImageProviderTest
	{
		protected abstract ImageProvider GetProvider();
		protected abstract ImageProvider GetProvider(NameValueCollection config);
		protected abstract NameValueCollection GetProviderConfiguration();

		[Test]
		public void RoundTrip()
		{
			ImageProvider provider = GetProvider();
			Object uid = provider.SaveImage(TestImage.ImageOneFilename);

			using (Image image = provider.GetImage(uid))
			{
				Assert.IsNotNull(image);
			}

			provider.DeleteImage(uid);

			using (Image image = provider.GetImage(uid))
			{
				Assert.IsNull(image);
			}
		}

		[Test]
		public void GetThumbnailImage()
		{
			Size newSize = new Size(200, 200);

			ImageProvider provider = GetProvider();

			Object uid = provider.SaveImage(TestImage.ImageOneFilename);

			using (Image image = provider.GetThumbnailImage(uid, newSize.Width, newSize.Height))
			{
				Assert.IsNotNull(image, "Resized image was not returned.");
				Assert.IsTrue(image.Width < TestImage.ImageOneWidth);
				Assert.IsTrue(image.Height < TestImage.ImageOneHeight);
				Assert.IsTrue(image.Width <= newSize.Width, "Resized image width is greater than expected.");
				Assert.IsTrue(image.Height <= newSize.Height, "Resized image height is greater than expected.");
			}
		}

		[Test]
		public void GetImage_Returns_Null_When_Image_Does_Not_Exist()
		{
			ImageProvider provider = GetProvider();

			Guid uid = Guid.NewGuid();

			using (Image image = provider.GetImage(uid))
			{
				Assert.IsNull(image);
			}
		}

		[Test]
		public void GetThumbnailImage_Returns_Null_When_Image_Does_Not_Exist()
		{
			ImageProvider provider = GetProvider();

			Guid uid = Guid.NewGuid();

			using (Image image = provider.GetThumbnailImage(uid, 100, 100))
			{
				Assert.IsNull(image);
			}
		}

	}
}
