using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static Squad;

/*
 * Esta es la clase que controla el control
 * de las unidades, en ella tendremos un objeto
 * del tipo SquadData dónde se almacenará
 * toda la información necesaria para actualizar
 * los atributos del GameObject que contiene este
 * script
 */

public class Squad : MonoBehaviour
{
    /*
     * Struct that relates a string
     * with a game object, this is for 
     * set the soldiers up.
     */
    [Serializable]
    public struct TypePrefab
    {
        public string type;
        public GameObject prefab;
    }

    public Color TeamColor0;
    public Color TeamColor1;
    public SquadData SqData;
    public List<TypePrefab> SoldiersPrefabs;

    private SpriteRenderer sr;
    [Header("------------- HEALTH BAR -------------")]
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Gradient healthGradient;
    private const int HELATHBAR_OFFSET = 5;
    private List<GameObject> unitInstances = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        canvas.transform.position = new Vector3(0, SqData.height / 2 + HELATHBAR_OFFSET, 0);
        SetColor();
        sr.size = new Vector2(SqData.width, SqData.height);
        SetupUnits();
        transform.position = SqData.Position;
        healthSlider.maxValue = SqData.Health;
    }

    private void SetupUnits()
    {
        foreach(TypePrefab typePrefab in SoldiersPrefabs)
        {
            if (typePrefab.type == SqData.Type)
            {
                /* Necesitamos asignar un tamaño a las unidades 
                 * que nos permita verlas bien dependiendo del tamaño del mapa.
                 * Como esto depende de la cantidad de detalle de los sprites
                 * y es muy complicado determinar una fórmula que funcione en
                 * cualquier caso será necesario hacerlo a prueba error.
                 */
                SpriteRenderer unitSr = typePrefab.prefab.GetComponent<SpriteRenderer>();
                float scale = unitSr.size.y / unitSr.size.x;
                /* De momento vamos a utilizar unas dimensiones de pantalla hardocodeadas porque 
                 * aún no se ha hecho la adaptación a las opciones de cambiar dimensiones (he decidido hacerlo así hace poco)
                 * y es difícil acceder a estos parámetros desde este script, en su momento
                 * estos valores deberían ser estáticos y por lo tanto accesibles desde cualquier
                 * sitio.
                 */
                float sizeX = 1000 / 50; //Hardcoded yet
                Vector2 unitSize = new Vector2(sizeX, sizeX * scale);
                unitSr.size = unitSize;

                List<Vector2> positions = DistributeUnits(unitSr.size, sr.size);
                foreach(Vector2 pos in positions)
                {
                    GameObject instance = Instantiate(typePrefab.prefab, transform);
                    instance.transform.position = pos;
                    unitInstances.Add(instance);
                }
                break;
            }
        }
    }

    private List<Vector2> DistributeUnits(Vector2 unitSize, Vector2 areaSize)
    {
        List<Vector2> positions = new List<Vector2>();
        int unitsOnX = (int)(areaSize.x / (unitSize.x));
        int unitsOnY = (int)(areaSize.y / (unitSize.x));
        float xOffset = areaSize.x - (unitSize.x * unitsOnX);
        float yOffset = areaSize.y - (unitSize.y * unitsOnY);
        for(float x = -areaSize.x/2+xOffset/2+unitSize.x/2; x <= areaSize.x/2-xOffset/2-unitSize.x/2; x += unitSize.x)
        {
            for(float y = -areaSize.y/2+unitSize.y/2; y <= areaSize.y/2; y += unitSize.y)
            {
                positions.Add(new Vector2(x, y)); 
            }
        }
        return positions;
    }

    private void SetColor()
    {
        if (SqData.team == 0) sr.color = TeamColor0;
        else sr.color = TeamColor1;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProperties();
    }

    private void UpdateProperties()
    {
        transform.position = SqData.Position;
        healthBar.color = healthGradient.Evaluate(healthSlider.value / healthSlider.maxValue);
        healthSlider.value = SqData.Health;
        SetUnitInstanceAnimation();
    }

    private void SetUnitInstanceAnimation()
    {
        foreach(GameObject unit in unitInstances)
        {
            Animator unitInstanceAnimator = unit.GetComponent<Animator>();
            switch (SqData.State)
            {
                case "IDLE":
                    unitInstanceAnimator.SetFloat("Speed", 0);
                    unitInstanceAnimator.SetBool("Attacking", false);
                    break;
                case "MOVING":
                    unitInstanceAnimator.SetFloat("Speed", 1);
                    unitInstanceAnimator.SetBool("Attacking", false);
                    break;
                case "DEAD":
                    unitInstanceAnimator.SetBool("dead", true);
                    break;
                default:
                    unitInstanceAnimator.SetBool("Attacking", true); 
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (SqData.Health > 0 && SqData.archerTarget)
            Gizmos.DrawWireSphere(transform.position, 50);
    }
}
