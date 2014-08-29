using System.Collections.Specialized;
using NUnit.Framework;

namespace Fpi.ImageService.Tests
{
	[TestFixture]
	public class CachedFileSystemImageProviderTest : ImageProviderTest
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
			CachedFileSystemImageProvider provider = new CachedFileSystemImageProvider();
			Assert.IsNotNull(provider);

			provider.Initialize(FileSystemProviderProperties.ProviderName, config);

			return provider;
		}

		[Test]
		public void Can_Create_CachedFileSystemImageProvider()
		{
			CachedFileSystemImageProvider provider = new CachedFileSystemImageProvider();
			Assert.IsNotNull(provider);
		}

	}
}
