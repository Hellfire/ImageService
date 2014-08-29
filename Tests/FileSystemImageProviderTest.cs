using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Fpi.ImageService.Tests
{
	[TestFixture]
	public class FileSystemImageProviderTest : ImageProviderTest
	{
		protected override NameValueCollection GetProviderConfiguration()
		{
			NameValueCollection config = new NameValueCollection();
			config.Add(FileSystemConfigurationPropertyName.FileSystemPath, FileSystemProviderProperties.FileSystemPath);
			return config;
		}

		protected override ImageProvider GetProvider()
		{
			return GetProvider(GetProviderConfiguration());
		}

		protected override ImageProvider GetProvider(NameValueCollection config)
		{
			FileSystemImageProvider provider = new FileSystemImageProvider();
			Assert.IsNotNull(provider);

			provider.Initialize(FileSystemProviderProperties.ProviderName, config);

			return provider;
		}

		[Test]
		public void Can_Create_FileSystemImageProvider()
		{
			FileSystemImageProvider provider = new FileSystemImageProvider();
			Assert.IsNotNull(provider);
		}

		[Test]
		[ExpectedException(typeof(ImageProviderException))]
		public void Initialize_Fails_When_FileSystemPath_Does_Not_Exist()
		{
			String fileSystemPath = Path.Combine(Path.GetTempPath(), Regex.Replace(Guid.NewGuid().ToString(), "[{}-]", String.Empty));
			NameValueCollection config = new NameValueCollection();
			config.Add(FileSystemConfigurationPropertyName.FileSystemPath, fileSystemPath);
			ImageProvider provider = GetProvider(config);
		}

		[Test]
		public void Initialize_Succeeds_When_FileSystemPath_Exists()
		{
			Assert.IsTrue(Directory.Exists(FileSystemProviderProperties.FileSystemPath), "Did you remember to create the path used for these unit tests?");

			ImageProvider provider = GetProvider();
			Assert.AreEqual(FileSystemProviderProperties.ProviderName, provider.Name);
		}

		[Test]
		public void GetImage_Does_Not_Return_Null_When_SendBlankImage_Is_True_And_Image_Does_Not_Exist()
		{
			NameValueCollection config = GetProviderConfiguration();
			config.Add(FileSystemConfigurationPropertyName.SendBlankImage, "true");

			ImageProvider provider = GetProvider(config);

			Guid uid = Guid.NewGuid();

			using (Image image = provider.GetImage(uid))
			{
				Assert.IsNotNull(image, "Image was not returned.");
			}
		}

	}
}
