using UnityEngine;
using UnityEditor;

public class PrefabBrush : EditorWindow
{
    GameObject prefab;
    float radius = 3f;
    int countPerClick = 6;
    Vector2 scaleRange = new Vector2(0.85f, 1.3f);

    [MenuItem("Tools/Prefab Brush")]
    static void Open() => GetWindow<PrefabBrush>("Prefab Brush");

    void OnGUI()
    {
        GUILayout.Label("Prefab Paint Brush", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        radius = EditorGUILayout.Slider("Brush Radius", radius, 0.5f, 20f);
        countPerClick = EditorGUILayout.IntSlider("Density", countPerClick, 1, 20);
        scaleRange = EditorGUILayout.Vector2Field("Scale Range", scaleRange);

        GUILayout.Space(10);
        GUILayout.Label("SHIFT + Left Click in Scene to paint", EditorStyles.helpBox);
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (prefab == null) return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                for (int i = 0; i < countPerClick; i++)
                {
                    Vector2 r = Random.insideUnitCircle * radius;
                    Vector3 pos = hit.point + new Vector3(r.x, 0, r.y);

                    var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    Undo.RegisterCreatedObjectUndo(go, "Paint Prefab");

                    go.transform.position = pos;
                    go.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    float s = Random.Range(scaleRange.x, scaleRange.y);
                    go.transform.localScale *= s;
                }
            }

            e.Use();
        }
    }
}
