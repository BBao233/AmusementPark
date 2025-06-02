using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    private void Start()
    {
        Camera mainCamera = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        transform.localScale = new Vector3(
            cameraWidth / spriteWidth,
            cameraHeight / spriteHeight,
            1f
        );
    }
}
