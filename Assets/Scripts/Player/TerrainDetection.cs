using PixelCrushers.DialogueSystem.Articy.Articy_4_0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetection : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask waterLayer;

    private float rayDistance = 1.5f;

    private PlayerMovement playerMovement;

    private string surface;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * rayDistance, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            if ((groundLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                surface = "ground";
                //Debug.Log("Player is standing on the ground");
            }
            else if ((waterLayer.value & (1 << hit.collider.gameObject.layer)) < 0)
            {
                surface = "water";
                //Debug.Log("Player is standing on water");
            }
            else
            {
                surface = "";
            }
        }
        else
        {
            surface = "";
        }
    }

    void CheckSurface()
    {
        
    }
}
