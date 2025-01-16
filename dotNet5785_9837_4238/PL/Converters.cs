using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using DalApi;
using DO;
using System.Windows.Media;
using System.Diagnostics.Eventing.Reader;

namespace PL
{


    /// <summary>
    /// converter for the delete button in the call screen
    /// </summary>
    public class ConverterDeleteAssignment : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is int NumberOfAssignment)
            {
                return NumberOfAssignment == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// converter for the delete button in the volunteer screen
    /// </summary>
    public class ConverterDeleteVolunteer : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value is int volId)
                return s_bl.Volunteer.sumOfCalls(volId) == 0 ? Visibility.Visible : Visibility.Collapsed;
            return false; 

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    class ConvertCallTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callType = (BO.CallType)value;

            switch (callType)
            {
                case BO.CallType.Math_Primary:
                    return Brushes.Blue;
                case BO.CallType.Math_Middle:
                    return Brushes.Cyan;
                case BO.CallType.Math_High:
                    return Brushes.DarkBlue;
                case BO.CallType.English_Primary:
                    return Brushes.LightGreen;
                case BO.CallType.English_Middle:
                    return Brushes.Lime;
                case BO.CallType.English_High:
                    return Brushes.Green;
                case BO.CallType.Grammary_Primary:
                    return Brushes.Pink;
                case BO.CallType.Grammary_Middle:
                    return Brushes.Purple;
                case BO.CallType.Grammary_High:
                    return Brushes.Magenta;
                default:
                    return Brushes.White; // default color if none match
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }












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
        /// 
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
          

            if (values is string add && add == "Add")
            {
                if (targetType == typeof(Visibility))
                    return Visibility.Collapsed;

                if (targetType == typeof(bool))
                    return false;
            }

            // אם הסטטוס הוא עדכון, היכולת תישאר גלויה
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



    public class CallInProgressVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callInProgress = value as BO.CallInProgress;

            if (parameter != null && parameter.ToString() == "HideOnCall")
            {
                return callInProgress != null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }

            return callInProgress != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

 
    /// <summary>
    /// converter to check according to the status of the call if we can change the value or not
    /// </summary>
    public class CallStatusToStateConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                
                if (value is string callStatus)
                {
                // we sent the parameter according to if it is IsReadOnly, IsEnabled
                string state = parameter as string;

                    // Logic for IsEnabled
                    if (state == "IsEnabled")
                    {
                        return callStatus == "Open" || callStatus == "OpenAtRisk";
                    }

                    // Logiב IsReadOnly
                    if (state == "IsReadOnly")
                    {
                        return !(callStatus == "Open" || callStatus == "OpenAtRisk");
                    }
                }

            return false; 
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    /// <summary>
    /// converter to check according to the status of the call if we can change the value or not
    /// </summary>
    public class ConverterToChangeMaxTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is string callStatus)
            {
               
                    return callStatus == "Open" || callStatus == "OpenAtRisk" || callStatus == "InCare" || callStatus == "InCareAtRisk";
                
            }

            return false;  
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }




}
