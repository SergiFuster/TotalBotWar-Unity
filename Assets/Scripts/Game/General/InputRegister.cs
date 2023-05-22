using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRegister : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* MOBILE VERSION
        // Verifica si hay alg�n toque en la pantalla
        if (Input.touchCount > 0)
        {
            // Recorre todos los toques en la pantalla
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                // Verifica si el toque acab� de empezar
                if (touch.phase == TouchPhase.Began)
                {
                    // Registra la posici�n del toque
                    Vector2 touchPosition = touch.position;
                    Debug.Log($"Touch on : {touchPosition}");
                    Debug.Log("Touch Position: " + touchPosition);
                }
            }
        }*

        /*PC VERSION */
        if (Input.GetMouseButtonDown(0))
        {

            // Convierte la posici�n del rat�n en coordenadas 2D dentro de la escena
            Vector3 mousePosition = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Data.AddTouch(worldPosition);
        }
    }

}
