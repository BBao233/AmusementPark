using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class background : MonoBehaviour
{
    private Transform camTF;
    private Vector3 lastFrameCameraPos;
    private float texunitSizeX;
    private float texunitSizeY;

    [SerializeField] private Vector2 parallaxFactor;
    [SerializeField] private bool EnableX;
    [SerializeField] private bool EnableY;

    private void Start()
    {
        camTF = Camera.main.transform;
        lastFrameCameraPos = camTF.position;
        Sprite sprite = this.GetComponent<SpriteRenderer>().sprite;
        texunitSizeX = sprite.texture.width/sprite.pixelsPerUnit;
        texunitSizeY = sprite.texture.height / sprite.pixelsPerUnit;
    }

    private void Update()
    {
        Vector2 deltaMovement = camTF.position - lastFrameCameraPos;
        transform.position = transform.position + new Vector3(deltaMovement.x * parallaxFactor.x, deltaMovement.y * parallaxFactor.y);
        lastFrameCameraPos = camTF.position;

        if(EnableX)
        {
            if (Mathf.Abs(camTF.position.x - transform.position.x) >= texunitSizeX)
            {
                float offsetX = camTF.position.x - transform.position.x;
                transform.position = new Vector3(camTF.position.x + offsetX, transform.position.y);
            }
        }

        if (EnableY)
        {
            if (Mathf.Abs(camTF.position.y - transform.position.y) >= texunitSizeY)
            {
                float offsetY = camTF.position.y - transform.position.y;
                transform.position = new Vector3(transform.position.x, camTF.position.y + offsetY);
            }
        }

        }
}
