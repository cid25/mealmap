﻿namespace Mealmap.Infrastructure;

public class ConcurrentUpdateException : Exception
{
    public ConcurrentUpdateException(string message) : base(message) { }

    public ConcurrentUpdateException(string message, Exception inner) : base(message, inner) { }
}
