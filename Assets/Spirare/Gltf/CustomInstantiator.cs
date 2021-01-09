using GLTFast;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInstantiator : GameObjectInstantiator
{
    public CustomInstantiator(Transform transform) : base(transform)
    {

    }

    public override void AddPrimitive(uint nodeIndex, string meshName, Mesh mesh, Material[] materials, int[] joints = null, bool first = true)
    {
        base.AddPrimitive(nodeIndex, meshName, mesh, materials, joints, first);
        var collider = nodes[nodeIndex].AddComponent<MeshCollider>();
        collider.convex = true;
    }
}

