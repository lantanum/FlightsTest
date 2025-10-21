﻿namespace Flights.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public string? Error { get; init; }
        public T? Value { get; init; }
        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
        public static Result<T> Fail(string error) => new() { IsSuccess = false, Error = error };
    }
}