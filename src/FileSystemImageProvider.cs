using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Fpi.ImageService
{
	public class FileSystemImageProvider : ImageProvider
	{
		#region Provider Configuration and Initialization

		private String _fileSystemPath = String.Empty;
		private ImageFormat _imageFormat = ImageFormat.Png;
		private Boolean _sendBlankImage;
		private Image _blankImage;
		private Stream _blankImageStream;

		public override void Initialize(String name, NameValueCollection config)
		{		
			// Do the local config first and then call base.Intialize().

			SetConfigOption(config, FileSystemConfigurationPropertyName.ImageFormat, ref _imageFormat);
			SetConfigOption(config, FileSystemConfigurationPropertyName.FileSystemPath, ref _fileSystemPath);
			SetConfigOption(config, FileSystemConfigurationPropertyName.SendBlankImage, ref _sendBlankImage);

			base.Initialize(name, config);

			ValidateFileSystemPath();
		}

		private void ValidateFileSystemPath()
		{
			// Ensure the FileSystemPath was defined
			if (String.IsNullOrEmpty(FileSystemPath))
			{
				throw new ConfigurationErrorsException("FileSystemPath is a required configuration property for the FileSystemImageProvider.");
			}

			// Ensure the defined FileSystemPath exists
			if (!Directory.Exists(FileSystemPath))
			{
				throw new ImageProviderException("The configured FileSystemPath does not exist.");
			}

			// Ensure we have Read/Write on the defined FileSystemPath.
			String testFilename = Path.Combine(FileSystemPath, "test.tmp");
			try
			{
				using (StreamWriter streamWriter = File.CreateText(testFilename))
				{
					streamWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffff"));
				}
			}
			catch (Exception ex)
			{
				throw new ImageProviderException("Cannot write to the configured FileSystemPath.", ex);
			}
			finally
			{
				// Clean up the temporary file
				if (File.Exists(testFilename))
				{
					File.Delete(testFilename);
				}
			}
		}

		#endregion

		#region Properties

		public String FileSystemPath
		{
			get
			{
				return _fileSystemPath;
			}
		}

		public override ImageFormat ImageFormat
		{
			get
			{
				return _imageFormat;
			}
		}

		#endregion

		public override object SaveImage(Image image)
		{
			Guid guid = Guid.NewGuid();
			String filename = GetOriginalFilename(guid, ImageFormat);
			WriteImageToFileSystem(filename, image, ImageFormat);
			return guid;
		}

		public override object SaveImage(String filename)
		{
			using (Image image = Image.FromFile(filename))
			{
				return SaveImage(image);
			}
		}

		public override void SaveImage(Object imageUID, Image image)
		{
			String filename = GetOriginalFilename(imageUID, ImageFormat);
			WriteImageToFileSystem(filename, image, ImageFormat);
		}

		protected void WriteImageToFileSystem(String filename, Image image, ImageFormat imageFormat)
		{
			CreateDirectory(Path.GetDirectoryName(filename));
			using (FileStream outputStream = new FileStream(filename, FileMode.Create))
			{
				image.Save(outputStream, imageFormat);
				outputStream.Close();
			}
		}

		public virtual Image GetBlankImage()
		{
			return GetBlankImage(ImageFormat);
		}

		public virtual Image GetBlankImage(ImageFormat imageFormat)
		{
			if (_blankImageStream == null)
			{
				String resourceName = FileSystemEmbeddedResourceName.BlankGifImage;
				if (imageFormat == ImageFormat.Png)
				{
					resourceName = FileSystemEmbeddedResourceName.BlankPngImage;
				}

				Assembly assembly = Assembly.GetExecutingAssembly();

				// NOTE: When an Image is created from a Stream we must keep the stream open for the lifetime of the Image.
				//       Since we reuse the blank image over and over, we must be sure to dispose it and it's underlying
				//       stream when this instance is disposed.  Further, because we keep the image around, the underlying 
				//       stream must support seeking.
				_blankImageStream = assembly.GetManifestResourceStream(resourceName);
				if (_blankImageStream == null)
				{
					return null;
				}
			}
			return Image.FromStream(_blankImageStream, false, true);
		}
	
		public override Image GetImage(object imageUID)
		{
			String filename = GetOriginalFilename(imageUID, ImageFormat);
			if (File.Exists(filename))
			{
				return Image.FromFile(filename);
			}
			return _sendBlankImage ? GetBlankImage() : null;
		}

		public override void DeleteImage(object imageUID)
		{
			String filename = GetOriginalFilename(imageUID, ImageFormat);
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}			
		}

		public virtual String GetDirectory(Object imageUID)
		{
			ThrowIfInvalidImageUID(imageUID);

			String key = Regex.Replace(imageUID.ToString(), "[{}-]", String.Empty, RegexOptions.IgnoreCase);
			String root = key.Substring(0, 1);
			String sub = key.Substring(0, 4);

			return Path.Combine(FileSystemPath, String.Format("{0}\\{1}\\", root, sub));
		}

		public virtual String GetOriginalFilename(Object imageUID, ImageFormat imageFormat)
		{
			ThrowIfInvalidImageUID(imageUID);

			String key = Regex.Replace(imageUID.ToString(), "[{}-]", String.Empty, RegexOptions.IgnoreCase);
			String extension = ImageService.GetExtension(imageFormat);
			String directory = GetDirectory(imageUID);

			return Path.Combine(directory, String.Concat(key, extension));
		}

		protected void ThrowIfInvalidImageUID(Object imageUID)
		{
			if (imageUID == null)
			{
				throw new ArgumentNullException("imageUID");
			}
			if (!(imageUID is Guid))
			{
				throw new ArgumentException("Invalid imageUID.  Expected a Guid.", "imageUID");
			}
		}

		private void CreateDirectory(String path)
		{
			// TODO: Add checking to ensure that relative paths are not created. Example: ../ImageService 
			// TODO: Test to see if this works with UNC paths. Example: \\ImageServer\ImageService

			if (Directory.Exists(path)) return;

			String parent = Path.GetDirectoryName(path);
			if (!Directory.Exists(parent))
			{
				CreateDirectory(parent);
			}
			Directory.CreateDirectory(path);
		}

		#region IDisposable Members

		private Boolean _disposed;

		public FileSystemImageProvider()
		{
			_sendBlankImage = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// free any managed disposable resources you own
					if (_blankImage != null)
					{
						_blankImage.Dispose();
						_blankImage = null;
					}

					if (_blankImageStream != null)
					{
						_blankImageStream.Dispose();
						_blankImageStream = null;
					}

					_disposed = true;
				}

				// perform any custom clean-up operations such as flushing the stream
			}

			base.Dispose(disposing);
		}

		#endregion
	
	}
}
