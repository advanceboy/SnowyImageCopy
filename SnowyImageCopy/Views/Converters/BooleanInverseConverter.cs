﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SnowyImageCopy.Views.Converters
{
	/// <summary>
	/// Inverse Boolean.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(bool))]
	public class BooleanInverseConverter : IValueConverter
	{
		/// <summary>
		/// Inverse Boolean.
		/// </summary>
		/// <param name="value">Source Boolean</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">Condition Boolean or Boolean string (optional, case-insensitive)</param>
		/// <param name="culture"></param>
		/// <returns>Inversed Boolean except if condition Boolean is given and does not match source Boolean.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertBase(value, parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertBase(value, parameter);
		}

		private static object ConvertBase(object value, object parameter)
		{
			if (!(value is bool))
				return DependencyProperty.UnsetValue;

			var sourceValue = (bool)value;

			var condition = FindBoolean(parameter);
			if (condition.HasValue && (condition.Value != sourceValue))
				return Binding.DoNothing; // DependencyProperty.UnsetValue will not work well.

			return !sourceValue;
		}

		private static bool? FindBoolean(object source)
		{
			if (source is bool)
				return (bool)source;

			if (bool.TrueString.Equals(source as string, StringComparison.OrdinalIgnoreCase))
				return true;

			if (bool.FalseString.Equals(source as string, StringComparison.OrdinalIgnoreCase))
				return false;

			return null;
		}
	}
}