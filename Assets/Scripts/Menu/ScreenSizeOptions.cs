using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeOptions : MonoBehaviour
{
    public static Vector2 option1 = new Vector2(1000, 500);
    public static Vector2 option2 = new Vector2(2000, 1000);
    public static Vector2 option3 = new Vector2(750, 750);
    public static Vector2 option4 = new Vector2(1500, 1500);

    public void size1000x500()
    {
        Data.Resolution = ScreenSizeOptions.option1;
    }

    public void size2000x1000()
    {
        Data.Resolution = ScreenSizeOptions.option2;

    }

    public void size750x750()
    {
        Data.Resolution = ScreenSizeOptions.option3;

    }

    public void size1500x1500()
    {
        Data.Resolution = ScreenSizeOptions.option4;

    }

}
