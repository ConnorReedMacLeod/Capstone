﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public static class LibView {

    // Returns the position of the mouse
    public static Vector3 GetMouseLocation() {
        Vector3 camPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(camPoint.x, camPoint.y, 0.0f);
    }

    public static GameObject GetObjectUnderMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
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
        if (go != null) {
            MonoBehaviour mono = (MonoBehaviour)go.GetComponentInChildren(type);
            return mono;
        } else {
            return null;
        }
    }

    public static void AssignSpritePathToObject(string sSprPath, GameObject go) {
        SpriteRenderer sprren = go.GetComponent<SpriteRenderer>();
        if (sprren != null) {
            AssignSpritePathToSpriteRenderer(sSprPath, sprren);
            return;
        }

        Image img = go.GetComponent<Image>();
        if(img != null) {
            AssignSpritePathToImage(sSprPath, img);
            return;
        }

        Debug.Log("Tried to assign an image to " + go + " but it has no spriterenderer or image component!");
    }

    public static void AssignSpritePathToSpriteRenderer(string sSprPath, SpriteRenderer sprRen) {

        Sprite sprIcon = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        if (sprIcon == null) {
            Debug.LogWarning("Could not find specificed sprite: " + sSprPath);
        }

        sprRen.sprite = sprIcon;

    }

    public static void AssignSpritePathToImage(string sSprPath, Image img) {

        Sprite sprIcon = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        if (sprIcon == null) {
            Debug.LogWarning("Could not find specificed sprite: " + sSprPath);
        }

        img.sprite = sprIcon;

    }


    public static void SetSkillTypeDropDownOptions(Dropdown dropdown, List<SkillType.SkillTypeInfo> lstSkillTypeInfo) {

        SetDropdownOptions(dropdown, lstSkillTypeInfo.Select(info => info.sName));

    }

    public static void SetDropdownOptions(Dropdown dropdown, IEnumerable<string> lstOptions) {
        //Clear out the current list of options
        dropdown.ClearOptions();

        List<Dropdown.OptionData> lstNewOptions;

        lstNewOptions = lstOptions.Select(str => new Dropdown.OptionData(str)).ToList();

        dropdown.AddOptions(lstNewOptions);

    }
}
