using System.Drawing;
using NUnit.Framework;

namespace Fpi.ImageService.Tests
{
	/// <summary>
	///This is a test class for ImageServiceTest and is intended
	///to contain all ImageServiceTest Unit Tests
	///</summary>
	[TestFixture]
	public class ImageServiceTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///A test for Providers
		///</summary>
		[Test]
		public void Default_Provider_Is_Not_Null()
		{
			Assert.IsNotNull(ImageService.Provider);
		}


		[Test]
		public void Providers_Has_One_Or_More_Providers()
		{
			Assert.IsTrue(ImageService.Providers.Count >= 1);
		}

		[Test]
		public void Has_ConfigSection()
		{
			Assert.IsNotNull(ImageService.ConfigSection);
		}

		[Test]
		public void GetThumbnailDimensions_Maintains_Aspect_Ratio_For_Square_Image()
		{
			// Square Image
			using (Bitmap bitmap = new Bitmap(100, 100))
			{
				Size size = ImageService.GetThumbnailDimensions(10, 10, bitmap);
				Assert.AreEqual(10, size.Width);
				Assert.AreEqual(10, size.Height);
			}
		}

		[Test]
		public void GetThumbnailDimensions_Maintains_Aspect_Ratio_For_Rectangle_Image()
		{
			using (Bitmap bitmap = new Bitmap(100, 200))
			{
				Size size = ImageService.GetThumbnailDimensions(10, 10, bitmap);
				Assert.AreEqual(5, size.Width);
				Assert.AreEqual(10, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(200, 100))
			{
				Size size = ImageService.GetThumbnailDimensions(10, 10, bitmap);
				Assert.AreEqual(10, size.Width);
				Assert.AreEqual(5, size.Height);
			}
		}

		[Test]
		public void GetThumbnailDimensions_Maintains_Aspect_Ratio_When_MaxHeight_Is_Zero()
		{
			using (Bitmap bitmap = new Bitmap(100, 100))
			{
				Size size = ImageService.GetThumbnailDimensions(50, 0, bitmap);
				Assert.AreEqual(50, size.Width);
				Assert.AreEqual(50, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(100, 200))
			{
				Size size = ImageService.GetThumbnailDimensions(50, 0, bitmap);
				Assert.AreEqual(50, size.Width);
				Assert.AreEqual(100, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(200, 100))
			{
				Size size = ImageService.GetThumbnailDimensions(50, 0, bitmap);
				Assert.AreEqual(50, size.Width);
				Assert.AreEqual(25, size.Height);
			}
		}

		[Test]
		public void GetThumbnailDimensions_Maintains_Aspect_Ratio_When_MaxWidth_Is_Zero()
		{
			using (Bitmap bitmap = new Bitmap(100, 100))
			{
				Size size = ImageService.GetThumbnailDimensions(0, 50, bitmap);
				Assert.AreEqual(50, size.Width);
				Assert.AreEqual(50, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(100, 200))
			{
				Size size = ImageService.GetThumbnailDimensions(0, 50, bitmap);
				Assert.AreEqual(25, size.Width);
				Assert.AreEqual(50, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(200, 100))
			{
				Size size = ImageService.GetThumbnailDimensions(0, 50, bitmap);
				Assert.AreEqual(100, size.Width);
				Assert.AreEqual(50, size.Height);
			}
		}

		[Test]
		public void GetThumbnailDimensions_Does_Not_Resize_An_Image_That_Is_Already_Smaller_Than_Requested()
		{
			using (Bitmap bitmap = new Bitmap(50, 50))
			{
				Size size = ImageService.GetThumbnailDimensions(100, 100, bitmap);
				Assert.AreEqual(50, size.Width);
				Assert.AreEqual(50, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(40, 50))
			{
				Size size = ImageService.GetThumbnailDimensions(0, 100, bitmap);
				Assert.AreEqual(40, size.Width);
				Assert.AreEqual(50, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(50, 40))
			{
				Size size = ImageService.GetThumbnailDimensions(0, 100, bitmap);
				Assert.AreEqual(50, size.Width);
				Assert.AreEqual(40, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(40, 50))
			{
				Size size = ImageService.GetThumbnailDimensions(100, 0, bitmap);
				Assert.AreEqual(40, size.Width);
				Assert.AreEqual(50, size.Height);
			}

			using (Bitmap bitmap = new Bitmap(50, 40))
			{
				Size size = ImageService.GetThumbnailDimensions(100, 0, bitmap);
				Assert.AreEqual(50, size.Width);
				Assert.AreEqual(40, size.Height);
			}
		}

	}
}
