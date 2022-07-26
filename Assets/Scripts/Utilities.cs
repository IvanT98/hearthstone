using System;

/// <summary>
/// A utility class for storing reusable static methods.
/// </summary>
public static class Utilities
{
    private static readonly Random Random = new();
    private const int MinimumRandomNumber = 1;

    /// <summary>
    /// Calculates a random list index.
    /// </summary>
    /// <param name="listSize">List size.</param>
    /// <returns>A random list index.</returns>
    public static int GetRandomListIndex(int listSize)
    {
        return Random.Next(listSize);;
    }

    /// <summary>
    /// Calculates and returns a random number in range from 1 to maximum.
    ///
    /// The minimum allowed random number is 1.
    /// </summary>
    /// <param name="max">Maximum allowed random number.</param>
    /// <returns>A random number.</returns>
    public static int GetRandomNumber(int max)
    {
        return Random.Next(MinimumRandomNumber, max);
    }
    
    /// <summary>
    /// Calculates and returns a random number in range from minimum to maximum.
    /// </summary>
    /// <param name="min">Minimum allowed random number.</param>
    /// <param name="max">Maximum allowed random number.</param>
    /// <returns>A random number.</returns>
    public static int GetRandomNumberInRange(int min, int max)
    {
        return Random.Next(min, max);
    }
}
