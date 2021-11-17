using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map: MonoBehaviour
{
    private static Dictionary<int, Dictionary<int, Dictionary<int, Hex>>> map = null;
    private static List<Hex> hexes = null;
    private static Hex activeHex;

    public enum Direction { E, SE, SW, W, NW, NE };

    public int nRings = 5;
    public int seed = 0;
    public float hexRadius = 1.0f;

    public GameObject hex1;

    public void Start()
    {
        if (map != null)
            return;

        map = new Dictionary<int, Dictionary<int, Dictionary<int, Hex>>>();
        hexes = new List<Hex>();

        for (int i=-nRings; i<=nRings; i++)
        {
            Dictionary<int, Dictionary<int, Hex>>  map_i = new Dictionary<int, Dictionary<int, Hex>>();
            for (int j = -nRings; j <= nRings; j++)
            {
                Dictionary<int, Hex> map_j = new Dictionary<int, Hex>();
                for (int k = -nRings; k <= nRings; k++)
                {
                    if (i + j + k != 0)
                    {
                        map_j.Add(k, null);
                        continue;
                    }

                    GameObject go = GameObject.Instantiate(hex1, transform);
                    Hex hex = go.AddComponent<Hex>();
                    hex.boardPosition = new Vector3Int(i, j, k);
                    hex.height = 1;
                    go.transform.localPosition = new Vector3((i * hexRadius) + (j * 0.5f * hexRadius), 0.0f, j * 1.5f * hexRadius / Mathf.Sqrt(3));

                    map_j.Add(k, hex);
                    hexes.Add(hex);
                }
                map_i.Add(j, map_j);
            }
            map.Add(i, map_i);
        }

        activeHex = map[0][0][0];
        SetDistance();

        HighlightByDistance(1, 3);
    }

    public Vector3Int NeighborPosition(Hex hex, Direction dir)
    {
        switch(dir)
        {
            case Direction.E:
                return hex.boardPosition + new Vector3Int(0, -1, 1);
            case Direction.SE:
                return hex.boardPosition + new Vector3Int(1, -1, 0);
            case Direction.SW:
                return hex.boardPosition + new Vector3Int(1, 0, -1);
            case Direction.W:
                return hex.boardPosition + new Vector3Int(0, 1, -1);
            case Direction.NW:
                return hex.boardPosition + new Vector3Int(-1, 1, 0);
            case Direction.NE:
                return hex.boardPosition + new Vector3Int(-1, 0, 1);
        }

        return hex.boardPosition; 
    }

    public bool PositionOnBoard(Vector3Int pos)
    {
        if (pos.x < -nRings || pos.x > nRings)
            return false;
        if (pos.y < -nRings || pos.y > nRings)
            return false;
        if (pos.z < -nRings || pos.z > nRings)
            return false;
        return true;
    }


    public List<Hex> Neighbors(Hex hex)
    {
        List<Hex> neighbors = new List<Hex>();

        foreach (Direction dir in Direction.GetValues(typeof(Direction)))
        {
            Vector3Int pos = NeighborPosition(hex, dir);

            if (PositionOnBoard(pos))
            {
                Hex neighbor = map[pos.x][pos.y][pos.z];
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private void SetDistanceRecursive(Hex hex)
    {
        if (hex.visited)
            return;

        hex.visited = true;

        foreach (Hex neighbor in Neighbors(hex))
        {
            neighbor.distance = Math.Min(neighbor.distance, hex.distance + 1);
        }
        foreach (Hex neighbor in Neighbors(hex))
        {
            SetDistanceRecursive(neighbor);
        }
    }

    public void SetDistance()
    {
        foreach (Hex hex in hexes)
        {
            hex.distance = int.MaxValue;
            hex.visited = false;
        }

        activeHex.distance = 0;
        SetDistanceRecursive(activeHex);
    }

    public void HighlightByDistance(int maxDistance, int maxJump)
    {
        foreach (Hex hex in hexes)
        {
            if (hex.distance <= maxDistance)
            {
                hex.Highlight();
            }
        }
    }
}
