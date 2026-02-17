using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.CommonResponses
{
    public class Result
    {
        protected readonly List<Error> _errors = [];
        public bool IsSuccess => _errors.Count == 0;
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors => _errors;

        protected Result() { }
        protected Result(Error error)
        {
            _errors.Add(error);
        }
        protected Result(List<Error> errors)
        {
            _errors.AddRange(errors);
        }

        // Static Factory Method 
        public static Result Ok() => new();
        public static Result Fail(Error error) => new(error);
        public static Result Fail(List<Error> errors) => new(errors);
    }

    // Generic Result 
    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        public TValue Value 
            => IsSuccess? _value: throw new InvalidOperationException("You Can Not Access the Value In Case Of Failure Scienario");

        private Result(TValue value) : base()
        {
            _value = value;
        }

        private Result(Error error) : base(error)
        {
            _value = default!;
        }

        private Result(List<Error> errors) : base(errors)
        {
            _value = default!;
        }

        // Factory Method 

        public static Result<TValue> Ok(TValue value) => new(value);
        public new static Result<TValue> Fail(Error error) => new(error);
        public new static Result<TValue> Fail(List<Error> errors) => new(errors);

        // Overloadin Operator (Implicitly)

        public static implicit operator Result<TValue> (TValue value) => Ok(value);
        public static implicit operator Result<TValue> (Error error) => Fail(error);
        public static implicit operator Result<TValue> (List<Error> errors) => Fail(errors);

    }
}
