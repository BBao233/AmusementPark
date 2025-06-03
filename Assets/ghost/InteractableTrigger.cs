using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractableTrigger : MonoBehaviour
{
    public GameObject[] targetTilemaps; // ����Ҫ��ʾ��Tilemap����
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
                tilemap.SetActive(true); // ��ʾTilemap
                tilemap.GetComponent<Tilemap>().RefreshAllTiles(); // ˢ��Tilemap��ʾ
            }
        }
    }

    // ��ѡ��������ù��ܣ�����Ҫ���ؿ��ظ�������
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
                tilemap.SetActive(false); // ����Tilemap
            }
        }
    }
}
