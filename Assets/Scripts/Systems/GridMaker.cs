using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    public Vector3 grid;
    public Vector3 offset;
    public GameObject pathNodePrefab;
    private Vector3 nextPos;

    //public LightProbeGroup lightGrid;
    
    [ContextMenu("lightgrid")]
    public void makelightprobegroup()
    {
        List<Vector3> positions = new List<Vector3>((int)(grid.x * grid.y * grid.z));
        int nxt=0;

        for (int i = 0; i < positions.Capacity; i++)
        {
            Vector3 newvec = Vector3.zero;

            positions.Add(newvec);
        }

        for (int x = 0; x < grid.x; x++)
        {
            for (int z = 0; z < grid.z; z++)
            {
                for (int y = 0; y < grid.y; y++)
                {
                    nextPos = transform.position;
                    nextPos.x += x * offset.x;
                    nextPos.y += y * offset.y;
                    nextPos.z += z * offset.z;
                    if (!Physics.CheckSphere(nextPos, 0.5f))
                    {
                        positions[nxt] = nextPos;
                        nxt++;
                    }
                }
            }
        }

        for (int i = 0; i < positions.Count; i++)
        {
            if(positions[i] == Vector3.zero)
            {
                positions.Remove(positions[i]);
            }
        }

        //lightGrid.probePositions = positions.ToArray();
    }
}
