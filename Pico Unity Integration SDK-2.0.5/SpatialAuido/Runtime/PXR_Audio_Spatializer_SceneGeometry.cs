//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using PXR_Audio.Spatializer;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PXR_Audio_Spatializer_SceneMaterial))]
public partial class PXR_Audio_Spatializer_SceneGeometry : MonoBehaviour
{
    private int geometryId = -1;
    public int GeometryId
    {
        get => geometryId;
    }

    private PXR_Audio_Spatializer_SceneMaterial material;
    public PXR_Audio_Spatializer_SceneMaterial Material
    {
        get
        {
            if (material == null)
                material = GetComponent<PXR_Audio_Spatializer_SceneMaterial>();
            return material;
        }
    }

    [SerializeField]
    private bool includeChildren = false;

    [SerializeField] private bool ignoreStatic = false;
    [SerializeField] private bool visualizeMeshInEditor = false;

    private PXR_Audio_Spatializer_Context context;
    private PXR_Audio_Spatializer_Context Context
    {
        get
        {
            if (context == null)
                context = FindObjectOfType<PXR_Audio_Spatializer_Context>();
            return context;
        }
    }
    
    /// <summary>
    /// Submit this geometry and it's material into spatializer engine context
    /// </summary>
    /// <param name="ctx">The native address of initialized spatializer context</param>
    /// <returns>Result of mesh submission</returns>
    public PXR_Audio.Spatializer.Result SubmitToContext(IntPtr ctx /***/)
    {
        //  Fetch vertices and indices buffer from mesh filters (optionally in children's meshes as well)
        List<MeshFilter> meshList = new List<MeshFilter>();
        TraverseMeshHierarchy(this.gameObject, includeChildren, meshList, ignoreStatic);

        
        int verticesCount = 0;
        int indicesCount = 0;
        foreach (MeshFilter meshFilter in meshList)
            AccumulateFlattenedMeshSize(meshFilter.sharedMesh, ref verticesCount, ref indicesCount);
        float[] vertices = new float[verticesCount * 3];
        int[] indices = new int[indicesCount];
        FlattenMeshList(meshList, vertices, indices);

        var ret = PXR_Audio.Spatializer.Api.SubmitMeshAndMaterialFactor(ctx, vertices, verticesCount, indices,
            indicesCount / 3, Material.absorption, Material.scattering, Material.transmission, ref geometryId);
        if (ret != Result.Success)
        {
            Debug.LogError("Failed to submit audio mesh: " + gameObject.name + ", Error code is: " + ret);
        }

        return ret;
    }

    private static void TraverseMeshHierarchy(GameObject obj, bool includeChildren, List<MeshFilter> meshList, bool ignoreStatic)
    {
        if (!obj.activeInHierarchy)
            return;

        MeshFilter[] meshes                 = obj.GetComponents<MeshFilter>();
        
        // Gather the meshes.
        foreach (MeshFilter meshFilter in meshes)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
                continue;
            
            if (ignoreStatic && !mesh.isReadable)
            {
                Debug.LogWarning("Mesh: " + meshFilter.gameObject.name + " not readable, cannot be static.", meshFilter.gameObject);
                continue;
            }
            
            meshList.Add(meshFilter);
        }

        // Traverse to the child objects.
        if (includeChildren)
        {
            foreach (Transform child in obj.transform)
            {
                // skip children which have their own component
                if (child.GetComponent<PXR_Audio_Spatializer_SceneGeometry>() == null) 
                    TraverseMeshHierarchy(child.gameObject, includeChildren, meshList, ignoreStatic);
            }
        }
    }

    private static void FlattenMeshList(List<MeshFilter> meshList,  float[] vertices, int[] indices)
    {
        int verticesIdx = 0;
        List<int> tempIndices = new List<int>();
        int vertexOffset = 0;
        int indexOffset = 0;
        
        foreach (MeshFilter meshFilter in meshList)
        {
            Matrix4x4 meshFilterToWorldMatrix = meshFilter.gameObject.transform.localToWorldMatrix;

            //  Add vertices of this mesh
            foreach (Vector3 vertex in meshFilter.sharedMesh.vertices)
            {
                Vector3 vertexWorldSpace = meshFilterToWorldMatrix.MultiplyPoint3x4(vertex);
                vertices[verticesIdx++] = vertexWorldSpace.x;
                vertices[verticesIdx++] = vertexWorldSpace.y;
                vertices[verticesIdx++] = -vertexWorldSpace.z;
            }

            //  Add indices of all submeshes
            int subMeshCount = meshFilter.sharedMesh.subMeshCount;
            for (int subMeshIdx = 0; subMeshIdx < subMeshCount; ++subMeshIdx)
            {
                MeshTopology subMeshTopology = meshFilter.sharedMesh.GetTopology(subMeshIdx);
                tempIndices.Clear();
                meshFilter.sharedMesh.GetIndices(tempIndices, subMeshIdx);

                if (subMeshTopology != MeshTopology.Triangles && subMeshTopology != MeshTopology.Quads)
                    continue;

                int subMeshIndexCount = tempIndices.Count;

                if (subMeshTopology == MeshTopology.Triangles)
                {
                    // Copy and adjust the indices.
                    for (int j = 0; j < subMeshIndexCount; j += 3)
                    {
                        indices[indexOffset + j + 2] = tempIndices[j    ] + vertexOffset;
                        indices[indexOffset + j + 1] = tempIndices[j + 1] + vertexOffset;
                        indices[indexOffset + j    ] = tempIndices[j + 2] + vertexOffset;
                    }

                    indexOffset += subMeshIndexCount;
                }
                else  // subMeshTopology == MeshTopology.Quads
                {
                    // Copy and adjust the indices.
                    for (int j = 0, k = 0; j < subMeshIndexCount; j += 4, k += 6)
                    {
                        indices[indexOffset + k + 2] = tempIndices[j    ] + vertexOffset;
                        indices[indexOffset + k + 1] = tempIndices[j + 1] + vertexOffset;
                        indices[indexOffset + k    ] = tempIndices[j + 2] + vertexOffset;
                        indices[indexOffset + k + 5] = tempIndices[j + 2] + vertexOffset;
                        indices[indexOffset + k + 4] = tempIndices[j + 3] + vertexOffset;
                        indices[indexOffset + k + 3] = tempIndices[j    ] + vertexOffset;
                    }
                    indexOffset += subMeshIndexCount / 2 * 3;
                }
            }

            vertexOffset = verticesIdx / 3;
        }
    }
    
    private static void AccumulateFlattenedMeshSize
        (Mesh mesh, ref int totalVertexCount, ref int totalIndexCount)
    {
        totalVertexCount += mesh.vertexCount;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            MeshTopology topology = mesh.GetTopology(i);
            if (topology == MeshTopology.Triangles || topology == MeshTopology.Quads)
            {
                uint meshIndexCount = mesh.GetIndexCount(i);
                totalIndexCount += (int)meshIndexCount;

                //  We'd like to de-compose quads to 2 triangles,
                //  so we'd need 2 more spaces for the second triangle
                if (topology == MeshTopology.Quads)
                    totalIndexCount += 2;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (visualizeMeshInEditor)
        {
            List<MeshFilter> meshList = new List<MeshFilter>();
            TraverseMeshHierarchy(this.gameObject, includeChildren, meshList, ignoreStatic);
            foreach (var mf in meshList)
            {
                Gizmos.DrawWireMesh(mf.sharedMesh, mf.transform.position, mf.transform.rotation, mf.transform.lossyScale);
            }
        }
    }
}
