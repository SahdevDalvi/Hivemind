using UnityEngine;
using System.Collections.Generic;

public class ActivityZone : MonoBehaviour
{
    public string zoneType; // work, rest, food, social
    public int maxCapacity = 10;

    private List<NPCBrain> insideNPCs = new List<NPCBrain>();

    public bool HasSpace()
    {
        return insideNPCs.Count < maxCapacity;
    }

    public Vector3 GetFreeSpot()
    {
        float radius = 4f;

        for (int i = 0; i < 20; i++)
        {
            Vector2 rand = Random.insideUnitCircle * radius;
            Vector3 pos = transform.position + new Vector3(rand.x, 0, rand.y);

            if (Vector3.Distance(pos, transform.position) < radius)
                return pos;
        }

        return transform.position;
    }

    public void Enter(NPCBrain npc)
    {
        if (!insideNPCs.Contains(npc))
            insideNPCs.Add(npc);
    }

    public void Exit(NPCBrain npc)
    {
        if (insideNPCs.Contains(npc))
            insideNPCs.Remove(npc);
    }
}
