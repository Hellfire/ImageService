using System;
using System.Runtime.Serialization;

namespace Fpi.ImageService
{
	public class ImageProviderException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageProviderException"/> class.
		/// </summary>
		public ImageProviderException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageProviderException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ImageProviderException(String message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageProviderException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
		public ImageProviderException(String message, Exception inner)
			: base(message, inner)
		{
		}

		protected ImageProviderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
