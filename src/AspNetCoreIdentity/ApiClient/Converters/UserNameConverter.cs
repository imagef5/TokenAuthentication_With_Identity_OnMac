using System;
using System.Globalization;
using ApiClient.Models;
using Xamarin.Forms;

namespace ApiClient.Converters
{
	/// <summary>
	/// 사용자 이름 변환 Conver class
	/// </summary>
	public class UserNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var name = value as UserName;

			if(name == null)
				throw new InvalidOperationException("The target must be a UserName");


			return $"{name.Title}. {name.First} {name.Last}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		} 
	}
}
