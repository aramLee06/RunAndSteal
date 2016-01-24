using UnityEngine;
using System.Collections;

public class OnePadLayerSorter : MonoBehaviour
{
    public string SortLayerName;

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            return;

        sr.sortingLayerName = SortLayerName;
    }
}
