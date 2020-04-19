using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState
{
    public static int bullets = 3;
    public static int coal = 21;
    public static bool lostHeart;

    public static void Reset()
    {
        bullets = 3;
        coal = 21;
        lostHeart = false;
    }
}
