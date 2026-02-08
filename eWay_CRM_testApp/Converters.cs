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

            string exeDir = AppContext.BaseDirectory;
            string fullPath;

            // Check if path already includes a directory (like "Resources/...")
            if (fileName.Contains("/") || fileName.Contains("\\"))
            {
                // Path includes directory, resolve relative to exe
                fullPath = Path.Combine(exeDir, fileName.Replace('/', Path.DirectorySeparatorChar));
                Debug.WriteLine($"PhotoPathToUriConverter: Path with directory: {fullPath}");
            }
            else
            {
                // Just a filename, try Photos directory first
                string photosDir = Path.Combine(exeDir, "Photos");
                fullPath = Path.Combine(photosDir, fileName);
                Debug.WriteLine($"PhotoPathToUriConverter: Trying Photos directory: {fullPath}");

                // If not found in Photos, try Resources as fallback
                if (!File.Exists(fullPath))
                {
                    fullPath = Path.Combine(exeDir, "Resources", fileName);
                    Debug.WriteLine($"PhotoPathToUriConverter: File not in Photos, trying Resources: {fullPath}");
                }
            }

            Debug.WriteLine($"PhotoPathToUriConverter: File exists: {File.Exists(fullPath)}");

            if (!File.Exists(fullPath))
            {
                // Ultimate fallback to Resources placeholder
                fullPath = Path.Combine(exeDir, "Resources", "placeholder2.jpg");
                Debug.WriteLine($"PhotoPathToUriConverter: Using fallback placeholder: {fullPath}");
                if (!File.Exists(fullPath))
                {
                    Debug.WriteLine("PhotoPathToUriConverter: Fallback placeholder not found either!");
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