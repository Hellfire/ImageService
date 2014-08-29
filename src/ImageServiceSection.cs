using System.Configuration;

namespace Fpi.ImageService
{
	public class ImageServiceSection : ConfigurationSection
	{
		/// <summary>
		/// Gets the providers.
		/// </summary>
		/// <value>The providers.</value>
		[ConfigurationProperty(ConfigurationSectionName.Providers)]
		public ProviderSettingsCollection Providers
		{
			get
			{
				return (ProviderSettingsCollection)base[ConfigurationSectionName.Providers];
			}
		}

		/// <summary>
		/// Gets or sets the default provider.
		/// </summary>
		/// <value>The default provider.</value>
		[ConfigurationProperty(ConfigurationPropertyName.DefaultProvider, DefaultValue = ImageProviderTypeName.FileSystem)]
		public string DefaultProvider
		{
			get
			{
				return (string)base[ConfigurationPropertyName.DefaultProvider];
			}
			set
			{
				base[ConfigurationPropertyName.DefaultProvider] = value;
			}
		}

	}
}
