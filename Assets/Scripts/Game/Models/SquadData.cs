using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Esta clase es la plantilla dónde se vuelcan
 * los datos de las unidades.
 */

[Serializable]
public class SquadData
{
    public int id;
    public int team;
    public string State;
    public string Type;
    public float Health;
    public Vector2 Position;
    public Vector2 Orientation;
    public float width;
    public float height;
    public bool archerTarget;

    public override string ToString()
    {
        return $"type : {Type},\n" +
            $"health : {Health}\n" +
            $"position : {Position}\n" +
            $"orientation : {Orientation}\n" +
            $"width : {width},\n" +
            $"height : {height}";
    }

}
