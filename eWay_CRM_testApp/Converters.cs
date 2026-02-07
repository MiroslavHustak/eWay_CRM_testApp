/*
MIT License

Copyright(c) 2021 Bent Tranberg

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Windows.Data;
using System.Globalization;

namespace eWay_CRM_testApp
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue)) return true;
            if (Equals(value, FalseValue)) return false;
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(Uri))]
    public sealed class PhotoPathToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine($"Convert called with value = {value}");

            if (value == null)
            {
                Debug.WriteLine("PhotoPathToUriConverter: value is null");
                return null;
            }

            string fileName = null;

            // Handle both plain string and wrapped PhotoPath type
            if (value is string str)
            {
                fileName = str;
                Debug.WriteLine($"PhotoPathToUriConverter: Received string: {fileName}");
            }
            else
            {
                // Try to get the underlying value from F# wrapped type
                // F# single-case unions have an Item property
                var itemProperty = value.GetType().GetProperty("Item");
                if (itemProperty != null)
                {
                    var itemValue = itemProperty.GetValue(value);
                    if (itemValue is string itemStr)
                    {
                        fileName = itemStr;
                        Debug.WriteLine($"PhotoPathToUriConverter: Extracted from PhotoPath: {fileName}");
                    }
                }
                else
                {
                    // Fallback: try ToString()
                    fileName = value.ToString();
                    Debug.WriteLine($"PhotoPathToUriConverter: Using ToString(): {fileName}");
                }
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine("PhotoPathToUriConverter: fileName is empty after extraction");
                return null;
            }

            // Resolve the Photos folder next to the executable
            string exeDir = AppContext.BaseDirectory;
            string photosDir = Path.Combine(exeDir, "Photos");
            string fullPath = Path.Combine(photosDir, fileName);

            /*
            Debug.WriteLine($"PhotoPathToUriConverter: BaseDirectory: {exeDir}");
            Debug.WriteLine($"PhotoPathToUriConverter: Photos directory: {photosDir}");
            Debug.WriteLine($"PhotoPathToUriConverter: Full path: {fullPath}");
            Debug.WriteLine($"PhotoPathToUriConverter: File exists: {File.Exists(fullPath)}");
            */

            if (!File.Exists(fullPath))
            {
                // fallback to placeholder if file not found
                fullPath = Path.Combine(photosDir, "placeholder1.jpg");
                Debug.WriteLine($"PhotoPathToUriConverter: Using placeholder: {fullPath}");

                if (!File.Exists(fullPath))
                {
                    Debug.WriteLine("PhotoPathToUriConverter: Placeholder not found either!");
                    return null;
                }
            }

            var uri = new Uri(fullPath, UriKind.Absolute);
            Debug.WriteLine($"PhotoPathToUriConverter: Returning URI: {uri}");
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}