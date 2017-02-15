using System;

namespace Xamarin.Forms.Xaml
{
	[AcceptEmptyServiceProvider]
	public class NullExtension : IMarkupExtension
	{
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return null;
		}
	}
}