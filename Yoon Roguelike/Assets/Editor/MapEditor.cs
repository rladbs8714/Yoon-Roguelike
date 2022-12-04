using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapEditor : Editor
{
    float mapGeneratorCallBackInterval;
    /*
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator;

        if (mapGeneratorCallBackInterval >= 0.1f)
        {
            map.GenerateMap();
            mapGeneratorCallBackInterval = 0f;
        }
        mapGeneratorCallBackInterval += Time.deltaTime;
    }
    */
}
