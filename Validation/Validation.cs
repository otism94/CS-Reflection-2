using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reflection.Validation
{
    public class UrlOptionalPrefix : ValidationAttribute
    {

    }

    public class MinDimensions : ValidationAttribute
    {
        private readonly int _minDimensions;

        public MinDimensions(int minDimensions)
        {
            _minDimensions = minDimensions;
        }

        protected override ValidationResult IsValid(
            object value, 
            ValidationContext validationContext)
        {
            try
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    using (var image = Image.FromStream(file.OpenReadStream()))
                    {
                        if (image.Width < _minDimensions && image.Height < _minDimensions)
                        {
                            return new ValidationResult(GetErrorMessage());
                        }
                    }
                }
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult("Error validating file. Please try again.");
            }
        }

        public string GetErrorMessage()
        {
            return $"Minimum logo dimensions are {_minDimensions}x{_minDimensions}.";
        }
    }

    public class MaxFileSize : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSize(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            try
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    if (file.Length > _maxFileSize)
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult("Error validating file. Please try again.");
            }
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is {(double)_maxFileSize / 1000000}MB.";
        }
    }

    public class ImageExtensions : ValidationAttribute
    {
        private readonly string[] _validExtensions = { ".png", ".jpg", ".jpeg" };

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            try
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    if (!_validExtensions.Contains(fileExtension))
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult("Error validating file. Please try again.");
            }
            
        }

        public string GetErrorMessage()
        {
            return "Invalid image format. Please upload a .png, .jpg, or .jpeg file.";
        }
    }
}
