using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cinemachine;
/*
 * Esta clase sirve para transformar
 * los json recibidos en otras estrucutras
 * o actualizar las estructuras existentes 
 * con los nuevos datos.
 * 
*/
public class Converter : MonoBehaviour
{
    public GameObject ContainerTeam0;
    public GameObject ContainerTeam1;
    public GameObject SquadPrefab;

    public void UpdateSquadsFromJson(string gameStateJson)
    {
        dynamic datos = JsonConvert.DeserializeObject(gameStateJson);

        foreach (var unit in datos["team_0"])
        {
            SquadData data = JsonConvert.DeserializeObject<SquadData>(unit.Value.ToString());
            data.team = 0;
            Data.TeamUnits0[data.id].GetComponent<Squad>().SqData = data;
        }

        foreach (var unit in datos["team_1"])
        {
            SquadData data = JsonConvert.DeserializeObject<SquadData>(unit.Value.ToString());
            data.team = 1;
            Data.TeamUnits1[data.id].GetComponent<Squad>().SqData = data;
        }

    }

    public string VectorToJson(Vector2 touch)
    {
        return JsonUtility.ToJson(touch);
    }
    public void CreateSquadsFromJson(string firstGameStateJson)
    {
        /*
        // Usaremos un archivo de manera temporal
        string rutaArchivo = Path.Combine(Application.dataPath, "JSON/primer_game_state.json");
        firstGameStateJson = File.ReadAllText(rutaArchivo);*/
        dynamic datos = JsonConvert.DeserializeObject(firstGameStateJson);


        int id = 0;
        foreach(var unit in datos["team_0"])
        {
            SquadData data = JsonConvert.DeserializeObject<SquadData>(unit.Value.ToString());
            data.team = 0;
            SquadPrefab.GetComponent<Squad>().SqData = data;
            GameObject squad = Instantiate(SquadPrefab, parent:ContainerTeam0.transform);
            Data.AddSquadTeam0(id, squad);
            SetName(id++, squad, data);
        }

        id = 0;
        foreach (var unit in datos["team_1"])
        {
            SquadData data = JsonConvert.DeserializeObject<SquadData>(unit.Value.ToString());
            data.team = 1;
            SquadPrefab.GetComponent<Squad>().SqData = data;
            GameObject squad = Instantiate(SquadPrefab, parent:ContainerTeam1.transform);
            Data.AddSquadTeam1(id, squad);
            SetName(id++, squad, data);
        }
        
    }

    private void SetName(int id, GameObject squad, SquadData data)
    {
        squad.name = $"{id} - {data.Type}";
    }
}
