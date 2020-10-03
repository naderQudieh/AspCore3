using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AppZeroAPI.Middleware
{
    public class NullObjectModelValidator : IObjectModelValidator
    {
        public void Validate(ActionContext actionContext,
            ValidationStateDictionary validationState, string prefix, object model)
        {

        }
    }
    public class FieldError
    {
        public string Field { get; set; }
        public string ErrorMessage { get; set; }
        public List<FieldError> SubErrors { get; set; } = new List<FieldError>();
    }
    public class ValidationResult
    {
        public string Message { get; set; }
        public List<FieldError> Errors { get; set; } = new List<FieldError>();
        public ValidationResult(string errorMessage)
        {
            Message = errorMessage;
        }
    }
    public class ValidationException : Exception
    {
        public ValidationResult ValidationResult { get; set; }

        public ValidationException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
    }
    public class ModelValidator
    {
        
        public static void Validate(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var validationResultIsValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
            if (!validationResultIsValid)
            {
                ValidationResult validationResult = new ValidationResult("One or more validation error should be fixed.");
                foreach (var item in validationResults)
                {
                    FieldError fieldError = new FieldError
                    {
                        Field = item.MemberNames.First(),
                        ErrorMessage = item.ErrorMessage
                    };
                    validationResult.Errors.Add(fieldError);
                }

                throw new ValidationException(validationResult);
            }
        }
    }
}
