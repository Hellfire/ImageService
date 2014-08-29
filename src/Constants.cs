using System;

namespace Fpi.ImageService
{
	public class ConfigurationSectionName
	{
		public const string Providers = "providers";
		public const string ImageService = "imageService";
	}

	public class ConfigurationPropertyName
	{
		public const string DefaultProvider = "defaultProvider";
	}

	/// <summary>
	/// Summary for the DataProviderTypeName class
	/// </summary>
	public class ImageProviderTypeName
	{
		public const string FileSystem = "FileSystemImageProvider";
	}

	public class FileSystemConfigurationPropertyName
	{
		public const String FileSystemPath = "fileSystemPath";
		public const String ImageFormat = "imageFormat";
		public const String SendBlankImage = "sendBlankImage";
	}

	public class FileSystemEmbeddedResourceName
	{
		public const String BlankPngImage = "Fpi.ImageService.blank.png";
		public const String BlankGifImage = "Fpi.ImageService.blank.png";
	}
}
