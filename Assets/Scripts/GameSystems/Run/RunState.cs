using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState
{
    public const int STARTING_COAL = 21;
    public const int STARTING_BULLETS = 3;

    public static int bullets = STARTING_BULLETS;
    public static int coal = STARTING_COAL;
    public static bool lostHeart = false;
    public static bool harvestedHeart = false;

    public static void Reset()
    {
        bullets = STARTING_BULLETS;
        coal = STARTING_COAL;
        lostHeart = false;
        harvestedHeart = false;
    }
}
