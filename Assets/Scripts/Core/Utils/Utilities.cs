using UnityEngine;
public static class Utilities
{
    public static bool RandBool()
    {
        return Random.value < 0.5f;
    }

    public static bool RandBool(float separator)
    {
        return Random.value < (separator % 1);
    }
}