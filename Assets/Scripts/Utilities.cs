using System;

public static class Utilities
{
    private static readonly Random Random = new();
    private const int MinimumRandomNumber = 1;

    public static int GetRandomListIndex(int listSize)
    {
        return Random.Next(listSize);;
    }

    public static int GetRandomNumber(int max)
    {
        return Random.Next(MinimumRandomNumber, max);
    }
    
    public static int GetRandomNumberInRange(int min, int max)
    {
        return Random.Next(min, max);
    }
}
