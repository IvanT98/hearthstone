using System;

public static class Utilities
{
    private static readonly Random _random = new Random();

    public static int GetRandomListIndex(int listSize)
    {
        int randomIndex = _random.Next(listSize);

        return randomIndex;
    }

    public static int GetRandomNumber(int max)
    {
        return _random.Next(1, max);
    }
    
    public static int GetRandomNumberInRange(int min, int max)
    {
        return _random.Next(min, max);
    }
}
