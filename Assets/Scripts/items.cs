using System;
using System.Collections;
using System.Collections.Generic;

public class items
{
    static Dictionary<string, float> item = new Dictionary<string, float>
    {
        {"Log", 5 },
        {"Wooden Plank", 2},
        {"Fiber", 1 },
        {"Stone", 1},
        {"Flint", 1},
        {"Oil", .2f},
        {"Water", .2f},
        {"Ammo", .2f},
        {"Scrap", .5f}
    };

    public static float valueOf(string getString)
    {
        if (item.TryGetValue(getString, out float val))
        {
            return val;
        } else
        {
            return 0;
        }
    }
}
