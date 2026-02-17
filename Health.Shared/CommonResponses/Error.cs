using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.CommonResponses
{
    public class Error
    {

        public string Code { get; }
        public string Description { get; }
        public ErrorTypes ErrorTypes { get; set; }

        private Error(string code, string description, ErrorTypes errorTypes)
        {
            Code = code;
            Description = description;
            ErrorTypes = errorTypes;
        }

        // Static Factory Method
        public static Error Failure(
            string Code = "General.Failure",
            string Description = "General Failure Has Occurred")
            => new Error(code: Code, description: Description, ErrorTypes.Failure);

        public static Error Validation(
            string Code = "General.Validation",
            string Description = "Validation Error Has Occurred")
            => new Error(code: Code, description: Description, ErrorTypes.Validation);

        public static Error NotFound(
            string Code = "General.NotFound",
            string Description = "The Requested Resource Was Not Found")
            => new Error(code: Code, description: Description, ErrorTypes.NotFound);

        public static Error Unauthorized(
            string Code = "General.Unauthorized",
            string Description = "You Are Not Authorized To Perform This Action")
            => new Error(code: Code, description: Description, ErrorTypes.Unauthorized);

        public static Error Forbidden(
            string Code = "General.Forbidden",
            string Description = "You Do Not Have Permission To Access This Resource")
            => new Error(code: Code, description: Description, ErrorTypes.Forbidden);

        public static Error InvalidCredentials(
            string Code = "General.InvalidCredentials",
            string Description = "The Provided Credentials Are Invalid")
            => new Error(code: Code, description: Description, ErrorType.InvalidCredentials);


    }
}
