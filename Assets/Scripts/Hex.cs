using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex: MonoBehaviour
{
    public Vector3Int boardPosition;
    public int height;
    public int distance = int.MaxValue;
    public bool visited = false;
    public bool outline = false;
    private bool lastOutline = false;

    public void Update()
    {
        if (outline != lastOutline)
        {
            if (outline)
                Highlight();
            else
                Unhighlight();
            lastOutline = outline;
        }
    }


    public void Highlight()
    {
        if (GetComponent<Outline>() != null)
            return;

        Outline outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        outline.OutlineWidth = 3.0f;
    }

    public void Unhighlight()
    {
        if (GetComponent<Outline>() == null)
            return;

        Destroy(GetComponent<Outline>());
    }
}
