using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Cinemachine;

public class StateManager : MonoBehaviour
{
    public enum STATE
    {
        WAITING,
        POSITIONING,
        DELETING,
        PLAYING
    }
    public List<GameObject> Backgrounds= new List<GameObject>();
    public GameObject PositioningInterface;
    public GameObject GameInterface;
    public TextMeshProUGUI Team0Name;
    public TextMeshProUGUI Team1Name;
    public TextMeshProUGUI remainingUnitsText;
    public ToggleGroup UnitsToggleGroup;
    public Material RectangleMaterial;
    public GameObject General;
    public CinemachineVirtualCamera VirtualCamera;
    public Canvas canvas;
    public int MaxUnits;
    [HideInInspector]
    public STATE state = STATE.WAITING;

    private int unitsAmount = 1;
    private List<GameObject> unitsPositioned = new List<GameObject>();
    private Requester requester = new Requester();
    private float minSpriteWidth;
    private GameObject rectangle;
    private Vector3 lastClickDownPos;
    private GameObject unitSelected;
    private const float DRAG_THRESHOLD = 20f;
    private Gesture gesture = Gesture.CLICK;
    enum Gesture
    {
        DRAG,
        CLICK
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        state = STATE.PLAYING;
        StartCoroutine(Temp());
        return;*/
        StartCoroutine(Connection());
    }

    private void AddGeneral()
    {
        SpriteRenderer sr = General.GetComponent<SpriteRenderer>();
        General.transform.position = new Vector3(requester.width/2, requester.height/4, 0);
        float heightRatio = sr.size.y / sr.size.x;
        sr.size = new Vector2(minSpriteWidth, minSpriteWidth * heightRatio);
        unitsPositioned.Add(General);
    }
    IEnumerator Temp()
    {
        // To see game before setup initial positions
        requester.GetInfo();
        yield return requester.Connect();
        StartCoroutine(Game());
    }
    private void Update()
    {
        if (state == STATE.PLAYING) return;
        remainingUnitsText.text = $"Remaining: {MaxUnits-unitsAmount}";
        if (UnitsToggleGroup.ActiveToggles().Count() == 0) state = STATE.DELETING;
        else state = STATE.POSITIONING;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (state == STATE.DELETING && unitSelected != null &&
            Vector3.Distance(mousePosition, lastClickDownPos) > DRAG_THRESHOLD &&
            IsInsideBounds(mousePosition))
        {
            gesture = Gesture.DRAG;
            unitSelected.transform.position = mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            
            if (!IsInsideBounds(mousePosition)) return;
            if (state == STATE.POSITIONING && unitsAmount < MaxUnits)
            {
                CreateUnit(mousePosition);
            }
            else if(state == STATE.DELETING)
            {
                OnDeletingClickDown(mousePosition);
            }
        }

        if(Input.GetMouseButtonUp(0) && state == STATE.DELETING && unitSelected != null)
            OnDeletingClickUp(mousePosition);
    }

    private bool IsInsideBounds(Vector3 pos)
    {
        return (pos.x >= 0 && pos.x <= requester.width && pos.y >= 0 && pos.y <= requester.height / 2);
    }

    private void OnDeletingClickUp(Vector3 mousePos)
    {
        if (gesture == Gesture.CLICK && unitSelected.name != "General")
        {
            unitsPositioned.Remove(unitSelected);
            Destroy(unitSelected);
            unitsAmount--;
        }
        unitSelected = null;
    }

    private void OnDeletingClickDown(Vector3 mousePos)
    {
        lastClickDownPos = mousePos;
        gesture = Gesture.CLICK;
        float minDist = -1;
        foreach(GameObject unit in unitsPositioned)
        {
            if (UnitClicked(unit, mousePos))
            {
                float dist = Vector3.Distance(mousePos, unit.transform.position);
                if (minDist == -1 || dist < minDist)
                {
                    minDist = dist;
                    unitSelected = unit;
                }
            }
        }
    }

    private bool UnitClicked(GameObject unit, Vector3 clickPos)
    {
        SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();
        return Vector3.Distance(unit.transform.position, clickPos) < sr.size.x;
    }
    private void CreateUnit(Vector3 mousePos)
    {
        PositioningToggleInfo info = UnitsToggleGroup.ActiveToggles().First<Toggle>().gameObject.GetComponent<PositioningToggleInfo>();
        GameObject unit = Instantiate(info.prefab);
        unit.name = info.prefab.name;
        unit.transform.position = mousePos;
        // unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, 0);
        SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();
        float heightRatio = sr.size.y / sr.size.x;
        sr.size = new Vector2(minSpriteWidth, minSpriteWidth * heightRatio);
        unitsPositioned.Add(unit);
        unitsAmount++;
    }

    IEnumerator Connection()
    {
        requester.GetInfo();
        //requester.GetDefaultInfo();
        yield return requester.Connect();
        InitialPositioningSetup();
    }

    private void InitialPositioningSetup()
    {
        SetPositioningCameraPosition();
        minSpriteWidth = requester.width / 20;
        AddGeneral();
        CreateRectangle(requester.width, requester.height/2);
        state = STATE.POSITIONING;
    }
    
    void SetPositioningCameraPosition()
    {
        VirtualCamera.m_Lens.OrthographicSize = requester.height / 2.5f;
        Vector3 cameraPos = new Vector3(requester.width / 2, requester.height / 4, VirtualCamera.transform.position.z);
        VirtualCamera.transform.position = cameraPos;
    }

    public void StartGameButtonPressed()
    {
        //Send positions to server here
        List<InitialUnit> unitsToSend = ParseGameObjectToInitialUnit();
        string json = JsonConvert.SerializeObject(unitsToSend);
        Debug.Log(json);
        string message = requester.SendInitialPositions(json);
        Debug.Log(message);
        SetCameraPosition();
        foreach (GameObject unit in unitsPositioned) Destroy(unit);
        unitsPositioned.Clear();
        Destroy(rectangle);
        setInterface();
        createBackground();
        StartCoroutine(Game());
    }

    void createBackground()
    {
        GameObject rndBackground = Backgrounds[Random.Range(0, Backgrounds.Count)];
        rndBackground.GetComponent<SpriteRenderer>().size = new Vector2(requester.width, requester.height);
        GameObject instance = Instantiate(rndBackground);
        instance.transform.position = new Vector3(requester.width / 2, requester.height / 2, 0);

    }
    void setInterface()
    {
        PositioningInterface.SetActive(false);
        GameInterface.SetActive(true);
        Team0Name.text = requester.bot0;
        Team1Name.text = requester.bot1;
    }

    public void SetCameraPosition()
    {

        VirtualCamera.m_Lens.OrthographicSize = requester.height / 1.8f;
        Vector3 cameraPos = new Vector3(requester.width/2, requester.height/2, VirtualCamera.transform.position.z);
        VirtualCamera.transform.position = cameraPos;

    }

    private List<InitialUnit> ParseGameObjectToInitialUnit()
    {
        List<InitialUnit> unitsToSend = new List<InitialUnit>();
        foreach(GameObject unit in unitsPositioned)
        {
            unitsToSend.Add(new InitialUnit(unit.transform.position.x, unit.transform.position.y, unit.name));
        }
        return unitsToSend;
    }

    IEnumerator Game()
    {
        yield return requester.StartGame();
        StartCoroutine(requester.GameLoop());
    }

    private void CreateRectangle(float width, float height)
    {
        rectangle = new GameObject("Rectangle");
        // Crear una nueva malla
        Mesh mesh = new Mesh();

        // Definir los vértices del rectángulo
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(width, 0, 0);
        vertices[2] = new Vector3(width, height, 0);
        vertices[3] = new Vector3(0, height, 0);

        // Definir los bordes del rectángulo
        int[] triangles = new int[8];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 1;
        triangles[3] = 2;
        triangles[4] = 2;
        triangles[5] = 3;
        triangles[6] = 3;
        triangles[7] = 0;

        // Asignar los vértices y triángulos a la malla
        mesh.vertices = vertices;
        mesh.SetIndices(triangles, MeshTopology.Lines, 0);

        // Calcular las normales y los valores UV de la malla
        mesh.RecalculateBounds();

        MeshRenderer meshRenderer = rectangle.AddComponent<MeshRenderer>();
        Material material = RectangleMaterial;
        meshRenderer.material = material;
        MeshFilter meshFilter = rectangle.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    class InitialUnit
    {
        public float x;
        public float y;
        public string type;
        public InitialUnit(float x, float y, string type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}

