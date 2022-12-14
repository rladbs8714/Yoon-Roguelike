using System;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleAtrray<T> (T[] array, int seed)
    {
        Random prng = new Random(seed); // System Random

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
