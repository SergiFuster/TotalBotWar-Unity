using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Esta clase almacena datos que facilitan
 * y hacen más eficiente a datos que se son
 * accedidos muchas veces. Por ejemplo, los 
 * GameObjects que la clase converter tiene
 * que actualizar en cada frame.
 * 
 */
public static class Data
{
    public static Dictionary<int, GameObject> TeamUnits0 = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> TeamUnits1 = new Dictionary<int, GameObject>();
    public static Queue<Vector2> Touches = new Queue<Vector2>();
    public static Vector2 Resolution = new Vector2(1000, 500);
    
    public static void AddSquadTeam0(int id, GameObject squad)
    {
        TeamUnits0.Add(id, squad);

    }
    public static void AddSquadTeam1(int id, GameObject squad)
    {
        TeamUnits1.Add(id, squad);
    }
    public static void AddTouch(Vector2 touch)
    {
        Touches.Enqueue(touch);
    }
}
