using System;

namespace Fpi.ImageService.Tests
{
	public static class FileSystemProviderProperties
	{
		public const String ProviderName = "FileSystemImageProvider";
		public const String FileSystemPath = "C:\\Temp\\ImageService";
	}

	public static class TestImage
	{
		public const String ImageOneFilename = "C:\\Temp\\ImageService\\Rings.gif";
		public const Int32 ImageOneWidth = 1000;
		public const Int32 ImageOneHeight = 1000;
		public const String ImageTwoFilename = "C:\\Temp\\ImageService\\Shyloh.jpg";
		public const Int32 ImageTwoWidth = 800;
		public const Int32 ImageTwoHeight = 1000;
	}
}
