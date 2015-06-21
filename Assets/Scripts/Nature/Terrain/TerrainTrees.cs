using UnityEngine;
using System.Collections;

public class TerrainTrees : MonoBehaviour
{
    private Terrain terrain;

    private TreeInstance[] originalTrees;

    private void Start ()
    {
        // Get the and set the Terrain Component.
        terrain = GetComponent<Terrain>();

        // Backup the original Trees on this terrain.
        originalTrees = terrain.terrainData.treeInstances;

        // Create a capsule collider for every terrain tree.
        for (int i = 0; i < originalTrees.Length; i++)
        {
            TreeInstance treeInstance = terrain.terrainData.treeInstances[i];

            GameObject treePrefab = terrain.terrainData.treePrototypes[treeInstance.prototypeIndex].prefab;

            Vector3 treePosition = Vector3.Scale(treeInstance.position, terrain.terrainData.size);

            GameObject treeObject = (GameObject)Instantiate(treePrefab, treePosition, Quaternion.identity);

            treeObject.name = treePrefab.name;
            treeObject.transform.parent = terrain.transform;
            treeObject.GetComponent<MeshRenderer>().enabled = false;

            treeObject.SendMessage("SetInstanceIndex", i, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnApplicationQuit ()
    {
        // Restore the trees on the terrain as it was before.
        terrain.terrainData.treeInstances = originalTrees;
    }
}