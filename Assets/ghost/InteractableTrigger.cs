using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractableTrigger : MonoBehaviour
{
    public GameObject[] targetTilemaps; // 关联要显示的Tilemap数组
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateTilemaps();
            isActivated = true;
        }
    }

    private void ActivateTilemaps()
    {
        foreach (GameObject tilemap in targetTilemaps)
        {
            if (tilemap != null)
            {
                tilemap.SetActive(true); // 显示Tilemap
                tilemap.GetComponent<Tilemap>().RefreshAllTiles(); // 刷新Tilemap显示
            }
        }
    }

    // 可选：添加重置功能（若需要机关可重复触发）
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActivated)
        {
            DeactivateTilemaps();
            isActivated = false;
        }
    }

    private void DeactivateTilemaps()
    {
        foreach (GameObject tilemap in targetTilemaps)
        {
            if (tilemap != null)
            {
                tilemap.SetActive(false); // 隐藏Tilemap
            }
        }
    }
}
