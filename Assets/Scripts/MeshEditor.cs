using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer), typeof(MeshFilter))]
public class MeshEditor : MonoBehaviour
{
    static T[] SubArray<T>(T[] arr, int startIndex, int length)
    {
        var newArr = new T[length];
        Array.Copy(arr, startIndex, newArr, 0, length);
        return newArr;
    }

    static Mesh ExtractSubmesh(Mesh mesh, int submesh)
    {
        Mesh newMesh = new Mesh();
        SubMeshDescriptor descriptor = mesh.GetSubMesh(submesh);
        newMesh.vertices = SubArray(mesh.vertices, descriptor.firstVertex, descriptor.vertexCount);

        if(mesh.tangents != null && mesh.tangents.Length == mesh.vertices.Length)
        {
            newMesh.tangents = SubArray(mesh.tangents, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.boneWeights != null && mesh.boneWeights.Length == mesh.vertices.Length)
        {
            newMesh.boneWeights = SubArray(mesh.boneWeights, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv != null && mesh.uv.Length == mesh.vertices.Length)
        {
            newMesh.uv = SubArray(mesh.uv, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv2 != null && mesh.uv2.Length == mesh.vertices.Length)
        {
            newMesh.uv2 = SubArray(mesh.uv2, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv3 != null && mesh.uv3.Length == mesh.vertices.Length)
        {
            newMesh.uv3 = SubArray(mesh.uv3, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv4 != null && mesh.uv4.Length == mesh.vertices.Length)
        {
            newMesh.uv4 = SubArray(mesh.uv4, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv5 != null && mesh.uv5.Length == mesh.vertices.Length)
        {
            newMesh.uv5 = SubArray(mesh.uv5, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv6 != null && mesh.uv6.Length == mesh.vertices.Length)
        {
            newMesh.uv6 = SubArray(mesh.uv6, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv7 != null && mesh.uv7.Length == mesh.vertices.Length)
        {
            newMesh.uv7 = SubArray(mesh.uv7, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.uv8 != null && mesh.uv8.Length == mesh.vertices.Length)
        {
            newMesh.uv8 = SubArray(mesh.uv8, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.colors != null && mesh.colors.Length == mesh.vertices.Length)
        {
            newMesh.colors = SubArray(mesh.colors, descriptor.firstVertex, descriptor.vertexCount);
        }

        if(mesh.colors32 != null && mesh.colors32.Length == mesh.vertices.Length)
        {
            newMesh.colors32 = SubArray(mesh.colors32, descriptor.firstVertex, descriptor.vertexCount);
        }

        var triangles = SubArray(mesh.triangles, descriptor.indexStart, descriptor.indexCount);
        for(int i = 0; i < triangles.Length; i++)
        {
            triangles[i] -= descriptor.firstVertex;
        }

        newMesh.triangles = triangles;

        if(mesh.normals != null && mesh.normals.Length == mesh.vertices.Length)
        {
            newMesh.normals = SubArray(mesh.normals, descriptor.firstVertex, descriptor.vertexCount);
        }
        else
        {
            newMesh.RecalculateNormals();
        }

        newMesh.Optimize();
        newMesh.OptimizeIndexBuffers();
        newMesh.RecalculateBounds();
        newMesh.name = mesh.name + $" Submesh {submesh}";
        return newMesh;
    }

    public static void SplitSubMesh(GameObject gameObject)
    {
        GameObject tempObject = Instantiate(gameObject);
        tempObject.name = gameObject.name;
        var meshFilter = tempObject.GetComponent<MeshFilter>();
        var materials = tempObject.GetComponent<MeshRenderer>().sharedMaterials;

        if(meshFilter == null || meshFilter.sharedMesh == null)
        {
            // Debug.LogWarning("No mesh exists on this gameObject");
            Destroy(tempObject);
            return;
        }
        if(meshFilter.sharedMesh.subMeshCount <= 1)
        {
            // Debug.LogWarning("Mesh has <= 1 submesh components. No additional extraction required.");
            Destroy(tempObject);
            return;
        }

        gameObject.GetComponent<MeshFilter>().mesh = ExtractSubmesh(meshFilter.sharedMesh, 0);
        gameObject.GetComponent<MeshRenderer>().materials = new[] { materials[0] };

        for(int i = 1; i < meshFilter.sharedMesh.subMeshCount; i++)
        {
            var child = Instantiate(tempObject, gameObject.transform);
            child.name = $"{tempObject.name} Submesh {i}";
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            child.GetComponent<MeshFilter>().mesh = ExtractSubmesh(meshFilter.sharedMesh, i);
            child.GetComponent<MeshRenderer>().materials = new[] { materials[i] };
        }

        DestroyImmediate(tempObject);
    }
}