using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ChangeLadderTrigger : Editor
{
    public const float radiusCheckSphere = 0.05f;

    private static LayerMask ladderLayer;

    private static Mesh mesh;
    private static Vector3[] vertices;
    private static Vector3 meshCenter;
    private static Vector3 meshSize;

    private static Vector3 LowerVertex;
    private static List<Vector3> lowerVerticies;
    private static Vector3 southLowerVertex;
    private static Vector3 northLowerVertex;

    private static int index;

    [MenuItem("T-Soft/Utilities/Generate Ladder triggers")]
    public static void GenerateLadderTriggers()
    {
        var currentObject = Selection.activeGameObject;

        GameObject[] ladderTriggers = new GameObject[currentObject.transform.childCount];
        for (int i = 0; i < ladderTriggers.Length; i++)
        {
            ladderTriggers[i] = currentObject.transform.GetChild(i).gameObject;
        }

        if(currentObject)
        {
            if (ladderTriggers.Length > 0)
            {
                GameObject parentGO = new GameObject(currentObject.name + "_Generated");
                parentGO.layer = LayerMask.NameToLayer("Ladder");

                for (int i = 0; i < ladderTriggers.Length; i++)
                {
                    GetComponents(ladderTriggers[i]);

                    CalculateMeshSize(ladderTriggers[i]);
                    CalculateLowerVerticiesOfLadderTrigger(ladderTriggers[i]);

                    CheckSphereAtLowerVertecies(ladderTriggers[i]).transform.SetParent(parentGO.transform, true);
                }

                PrefabUtility.SaveAsPrefabAsset(parentGO, "Assets/Prefabs/LadderTriggers/" + parentGO.name + ".prefab");
                DestroyImmediate(parentGO);

                Debug.Log("Ladder triggers succesfully generated");
            }
        }
        else
        {
            Debug.LogError("Select game object with ladder triggers");
        }
    }

    public static void GetComponents(GameObject ladderTrigger)
    {
        ladderLayer = LayerMask.GetMask("Ladder");

        mesh = ladderTrigger.GetComponent<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;

        meshCenter = mesh.bounds.center;

        lowerVerticies = new List<Vector3>();
    }

    public static void CalculateMeshSize(GameObject ladderTrigger)
    {
        Vector3 tempMagnitude = Vector3.zero;

        for (int i = 0; i < vertices.Length; i++)
        {
            if ((vertices[0] - vertices[i]).magnitude > tempMagnitude.magnitude)
            {
                tempMagnitude = vertices[0] - vertices[i];
                index = i;
            }
        }

        meshSize = tempMagnitude;
    }

    public static void CalculateLowerVerticiesOfLadderTrigger(GameObject ladderTrigger)
    {
        LowerVertex = vertices[0];

        // Поиск минимальной точки по y
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y < LowerVertex.y)
            {
                LowerVertex = vertices[i];
            }
        }

        // Запись точек, находящихся на нижней грани, в лист 
        for (int i = 0; i < vertices.Length; i++)
        {
            if ((Mathf.Round(vertices[i].y * 10) * 0.1f) == (Mathf.Round(LowerVertex.y * 10) * 0.1f))
            {
                lowerVerticies.Add(vertices[i]);
            }
        }

        // Удаление дубликатов из списка
        lowerVerticies = lowerVerticies.Distinct().ToList();

        // Поиск наименьшего нижнего ребра
        if ((lowerVerticies[0] - lowerVerticies[1]).magnitude < (lowerVerticies[0] - lowerVerticies[2]).magnitude &&
            (lowerVerticies[0] - lowerVerticies[1]).magnitude < (lowerVerticies[0] - lowerVerticies[3]).magnitude)
        {
            southLowerVertex = lowerVerticies[0];
            northLowerVertex = lowerVerticies[1];
        }
        if ((lowerVerticies[0] - lowerVerticies[2]).magnitude < (lowerVerticies[0] - lowerVerticies[1]).magnitude &&
         (lowerVerticies[0] - lowerVerticies[2]).magnitude < (lowerVerticies[0] - lowerVerticies[3]).magnitude)
        {
            southLowerVertex = lowerVerticies[0];
            northLowerVertex = lowerVerticies[2];
        }
        if ((lowerVerticies[0] - lowerVerticies[3]).magnitude < (lowerVerticies[0] - lowerVerticies[2]).magnitude &&
         (lowerVerticies[0] - lowerVerticies[3]).magnitude < (lowerVerticies[0] - lowerVerticies[1]).magnitude)
        {
            southLowerVertex = lowerVerticies[0];
            northLowerVertex = lowerVerticies[3];
        }
    }

    public static GameObject CheckSphereAtLowerVertecies(GameObject ladderTrigger)
    {
        bool checkNorth = Physics.CheckSphere(ladderTrigger.transform.rotation * northLowerVertex + ladderTrigger.transform.position,
            radiusCheckSphere * 2, ladderLayer.value);
        //bool checkSouth = Physics.CheckSphere(ladderTrigger.transform.rotation * southLowerVertex + ladderTrigger.transform.position,
        //    radiusCheckSphere * 2, ladderLayer.value);

        return CreateNewCollider(checkNorth, northLowerVertex, southLowerVertex, ladderTrigger);
    }

    public static GameObject CreateNewCollider(bool checkNorthSphere, Vector3 northLowerVertex, Vector3 southLowerVertex, GameObject ladderTrigger)
    {
        GameObject newGO = new GameObject(ladderTrigger.name + "_Generated");
        newGO.layer = LayerMask.NameToLayer("Ladder");
        BoxCollider newCollider = newGO.AddComponent<BoxCollider>();

        var forward = checkNorthSphere ? northLowerVertex - southLowerVertex : southLowerVertex - northLowerVertex;

        // Поворот вектора meshSize на угол между нормалью и transform.forward
        if (vertices[0].z + ladderTrigger.transform.position.z < vertices[index].z + ladderTrigger.transform.position.z)
        {
            meshSize = Quaternion.Euler(0, -Vector3.Angle(Vector3.forward, forward), 0) * (vertices[index] - vertices[0]);
        }
        else
        {
            meshSize = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, forward), 0) * (vertices[index] - vertices[0]);
        }

        newCollider.center = meshCenter;
        newCollider.size = meshSize;
        newCollider.isTrigger = true;

        if (checkNorthSphere)
        {
            newGO.transform.forward = ladderTrigger.transform.rotation * forward.normalized;
        }
        if (!checkNorthSphere)
        {
            newGO.transform.forward = ladderTrigger.transform.rotation * forward.normalized;
        }

        newGO.transform.position = ladderTrigger.transform.position;

        return newGO;
    }

    //public void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.rotation * northLowerVertex + transform.position, radiusCheckSphere * 2);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.rotation * southLowerVertex + transform.position, radiusCheckSphere * 2);
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(vertices[0], vertices[index]);
    //}
}