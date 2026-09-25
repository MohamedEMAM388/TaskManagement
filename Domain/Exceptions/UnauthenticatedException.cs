namespace Domain.Exceptions;

// Thrown when an operation requires an authenticated user but none is present.

public class UnauthenticatedException()
    : Exception("This action requires an authenticated user.");