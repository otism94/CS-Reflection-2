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

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is {(double)_maxFileSize / 1000000}MB.";
        }
    }
}
