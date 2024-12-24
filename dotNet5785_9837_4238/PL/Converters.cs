using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Converter class to convert a single value to a Visibility or bool value based on the input value.
    /// </summary>
    public  class ConverterAddUpdate : IValueConverter
    {
        /// <summary>
        /// Converts a single value to a Visibility  or bool value.
        /// </summary>
        /// <param name="values">The value to be converted.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <param name="parameter">An optional parameter for the conversion.</param>
        /// <param name="culture">The culture to be used in the converter.</param>
        /// <returns>A converted value based on the input value and target type.</returns>
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is string add && add == "Add")
            {
                if (targetType == typeof(Visibility))
                    return Visibility.Collapsed;
           
                if (targetType == typeof(bool))//to id isRead
                    return false;
            }

            if (targetType == typeof(Visibility))
                return Visibility.Visible;

            if (targetType == typeof(bool))
                return true;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a value back to the original value.
        /// </summary>
        /// <param name="value">The value produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">An optional parameter for the conversion.</param>
        /// <param name="culture">The culture to be used in the converter.</param>
        /// <returns>The original value.</returns>
        /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
