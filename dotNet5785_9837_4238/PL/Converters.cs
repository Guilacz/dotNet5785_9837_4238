using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using DalApi;

namespace PL
{
    /// <summary>
    /// Converter class to convert a single value to a Visibility or bool value based on the input value.
    /// </summary>
    public  class ConverterAddUpdate : IValueConverter
    {

        private IEnumerable<DO.Assignment> _assignments;

        // Add this property to the class
        public IEnumerable<DO.Assignment> Assignments
        {
            get => _assignments;
            set => _assignments = value;
        }
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
            if (parameter as string == "Delete" && values is BO.Call call)
            {
                // וודא של `_assignments` לא יהיה `null`
                if (_assignments == null)
                {
                    return Visibility.Collapsed;
                }

                int numberOfAssignment = _assignments.Count(a => a.CallId == call.CallId);

                // התנאי לא לפיה אם קריאה פתוחה ואין הקצאות
                if (call.CallStatus == BO.CallInListStatus.Open && numberOfAssignment == 0)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

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


        //public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (parameter as string == "Delete" && values is BO.Call call)
        //    {
        //        int numberOfAssignment = _assignments.Count(a => a.CallId == call.CallId);
        //        if (_assignments == null)
        //        {
        //            return Visibility.Collapsed;
        //        }
        //        if (call.CallStatus == BO.CallStatus.Open && numberOfAssignment == 0)
        //        {
        //            return Visibility.Visible;
        //        }
        //        return Visibility.Collapsed;
        //    }
        //    if (values is string add && add == "Add")
        //    {
        //        if (targetType == typeof(Visibility))
        //            return Visibility.Collapsed;

        //        if (targetType == typeof(bool))//to id isRead
        //            return false;
        //    }

        //    if (targetType == typeof(Visibility))
        //        return Visibility.Visible;

        //    if (targetType == typeof(bool))
        //        return true;

        //    throw new NotImplementedException();
        //}

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



        //public object ConvertToDelete(object value, Type targetType, object parameter, CultureInfo culture)
        //{



        //    if (value is BO.Call call)
        //    {
        //        //IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

        //        int numberOfAssignment = _assignments.Count(a => a.CallId == call.CallId);

        //        // תנאי: קריאה בסטטוס פתוח והקצאות שוות לאפס
        //        if (call.CallStatus == BO.CallStatus.Open && numberOfAssignment == 0)
        //        {
        //            return Visibility.Visible;
        //        }
        //    }

        //    return Visibility.Collapsed;
        //}
    }






    //public class CallInProgressVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var callInProgress = value as BO.CallInProgress;
    //        // Return Visibility.Visible if there's a call in progress, otherwise Collapsed
    //        return callInProgress != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class CallInProgressVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callInProgress = value as BO.CallInProgress;

            // Si le paramètre est "HideOnCall", renvoie Visibility.Collapsed si l'appel est en cours
            if (parameter != null && parameter.ToString() == "HideOnCall")
            {
                return callInProgress != null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }

            // Sinon, le comportement par défaut : Visible si appel en cours, Collapsed sinon
            return callInProgress != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
