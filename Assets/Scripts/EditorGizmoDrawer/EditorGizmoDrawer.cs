using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class EditorGizmoDrawer : MonoBehaviour
{
    // --- Customization Fields ---
    public Color gizmoColor = new Color(0.2f, 0.8f, 0.2f, 0.25f);
    public Color labelColor = Color.white;
    
    public MonoBehaviour targetScript; 

    void OnDrawGizmosSelected()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        
        // --- Draw the Box ---
        Gizmos.color = gizmoColor;
        Vector3 center = transform.position + new Vector3(collider.offset.x, collider.offset.y, 0);
        Vector3 size = new Vector3(collider.size.x, collider.size.y, 0.1f);
        Gizmos.DrawCube(center, size);

        // --- Prepare the Label ---
        GUIStyle style = new GUIStyle();
        style.normal.textColor = labelColor;
        style.alignment = TextAnchor.MiddleCenter;

        string customText = "";
        
        if (targetScript != null)
        {
            var field = targetScript.GetType().GetField("zoneName");
            if (field != null)
            {
                customText = (string)field.GetValue(targetScript);
            }
        }
        
        string textToDisplay = string.IsNullOrEmpty(customText) 
            ? this.gameObject.name 
            : $"{this.gameObject.name}: {customText}";
            
        Handles.Label(transform.position, textToDisplay, style);
    }
}