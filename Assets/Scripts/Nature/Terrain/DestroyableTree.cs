using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]

public class DestroyableTree : MonoBehaviour
{
    public float treeHealth;

    [HideInInspector]
    public int instanceIndex;

    private void Update ()
    {
        if (treeHealth <= 0f)
        {
            // Locate the terrain.
            Terrain terrain = Terrain.activeTerrain;

            // Get access to all the tree instances inside the terrain.
            List<TreeInstance> trees = new List<TreeInstance>(terrain.terrainData.treeInstances);

            // Sort of "errase" this tree from the terrain using its <instanceIndex> id.
            trees[instanceIndex] = new TreeInstance();

            // Update the current tree instances on the terrain.
            terrain.terrainData.treeInstances = trees.ToArray();

            // Make the Instantiated Tree GameObject visable.
            GetComponent<MeshRenderer>().enabled = true;

            // Access this trees Rigidbody and apply force to it so it falls over.
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(transform.forward * 50, ForceMode.Force);

            GetComponent<NatureResource>().available = true;

            enabled = false;
        }
    }

    private void TakeDamage (float damage)
    {
        treeHealth -= damage;
    }

    private void SetInstanceIndex (int index)
    {
        instanceIndex = index;
    }
}