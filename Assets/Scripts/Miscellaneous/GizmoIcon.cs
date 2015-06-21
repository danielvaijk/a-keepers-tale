using UnityEngine;
using System.Collections;

public class GizmoIcon : MonoBehaviour
{
    public Texture2D gizmoIcon;

    private void OnDrawGizmos ()
    {
        if (gizmoIcon != null)
            Gizmos.DrawIcon(transform.position, gizmoIcon.name, true);
    }
}