using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibView {

    // Returns the position of the mouse
    public static Vector3 GetMouseLocation() {
        Vector3 camPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(camPoint.x, camPoint.y, 0.0f);
    }

    public static GameObject GetObjectUnderMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit)) {
            // If there's a gameobject under the mouse, return it
            return hit.collider.gameObject;
        }

        // Otherwise, return false
        return null;
    }

    public static float GetAngle(Vector3 v3From, Vector3 v3To) {

        Vector3 v3Delta = v3To - v3From;

        return Mathf.Rad2Deg * Mathf.Atan2(v3Delta.y, v3Delta.x);
    }

    public static MonoBehaviour IsUnderMouse(System.Type type) {
        GameObject go = GetObjectUnderMouse();
        if(go != null) {
            MonoBehaviour mono = (MonoBehaviour)go.GetComponentInChildren(type);
            return mono;
        } else {
            return null;
        }
    }

    public static void AssignSpritePathToObject(string sSprPath, GameObject go) {

        Sprite sprIcon = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        if(sprIcon == null) {
            Debug.LogError("Could not find specificed sprite: " + sSprPath);
        }

        go.GetComponent<SpriteRenderer>().sprite = sprIcon;

    }
}
