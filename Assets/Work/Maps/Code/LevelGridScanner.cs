using UnityEngine;
using System.Collections.Generic;
using System;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Work.Maps.Code
{

    public class LevelGridScanner : MonoBehaviour
    {
        [Header("Grid Settings")]
        public float cellSize = 1f;

        [Header("Obstacle Settings")]
        public LayerMask obstacleLayer;

        [Header("Debug")]
        public bool drawGizmos = true;

        // Grid data
        public bool[,] walkable;
        public int width;
        public int height;

        Bounds levelBounds;

        static readonly Vector2Int[] directions =
{
    // 직선
    new Vector2Int( 0,  1),
    new Vector2Int( 0, -1),
    new Vector2Int( 1,  0),
    new Vector2Int(-1,  0),

    // 대각선
    new Vector2Int( 1,  1),
    new Vector2Int( 1, -1),
    new Vector2Int(-1,  1),
    new Vector2Int(-1, -1),
};


        public void BuildGrid()
        {
            levelBounds = CalculateLevelBounds();

            width = Mathf.CeilToInt(levelBounds.size.x / cellSize);
            height = Mathf.CeilToInt(levelBounds.size.z / cellSize);

            walkable = new bool[width, height];

            Vector3 origin = levelBounds.min;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 center = new Vector3(
                        origin.x + x * cellSize + cellSize * 0.5f,
                        levelBounds.min.y,
                        origin.z + z * cellSize + cellSize * 0.5f
                    );

                    walkable[x, z] = IsWalkable(center);
                }
            }

            Debug.Log($"[LevelGridBFS] Grid Built : {width} x {height}");
        }

        bool IsWalkable(Vector3 position)
        {
            Vector3 half = new Vector3(
                cellSize * 0.45f,
                4f,
                cellSize * 0.45f
            );

            return !Physics.CheckBox(
                position,
                half,
                Quaternion.identity,
                obstacleLayer
            );
        }

        public bool GetMovePath(
            Vector3 currentWorld,
            Vector3 targetWorld,
            out List<Vector3> movePath)
        {
            movePath = new List<Vector3>();

            if (walkable == null)
                return false;

            Vector2Int start = WorldToGrid(currentWorld);
            Vector2Int target = WorldToGrid(targetWorld);

            Debug.Log("범위안에 없음 시발 왜 없지 ");
            if (!IsInBounds(start) || !IsInBounds(target))
                return false;
            Debug.Log("InBound");

            if (!walkable[target.x, target.y])
                return false;
            Debug.Log("타겟이 맵위에 없음");

            List<Vector2Int> path = FindPathBFS(start, target);

            if (path == null || path.Count < 2)
                return false;
            Debug.Log("패스가 안나옴");

            movePath = GridToWorldList(path);
            return true;
        }

        private List<Vector3> GridToWorldList(List<Vector2Int> path)
        {
            List<Vector3> worldPath = new();
            foreach (var gridPos in path)
            {
                worldPath.Add(GridToWorld(gridPos));
            }
            return worldPath;
        }

        List<Vector2Int> FindPathBFS(Vector2Int start, Vector2Int target)
        {
            Queue<Vector2Int> queue = new();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new();
            HashSet<Vector2Int> visited = new();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (current == target)
                    return ReconstructPath(cameFrom, start, target);

                foreach (var dir in directions)
                {
                    Vector2Int next = current + dir;

                    if (!IsInBounds(next)) continue;
                    if (!walkable[next.x, next.y]) continue;
                    if (visited.Contains(next)) continue;

                    if (!CanMoveDiagonal(current, next)) continue;

                    visited.Add(next);
                    cameFrom[next] = current;
                    queue.Enqueue(next);
                }
            }

            return null;
        }

        bool CanMoveDiagonal(Vector2Int from, Vector2Int to)
        {
            Vector2Int dir = to - from;

            if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) != 2)
                return true;

            Vector2Int check1 = new(from.x + dir.x, from.y);
            Vector2Int check2 = new(from.x, from.y + dir.y);

            if (!IsInBounds(check1) || !IsInBounds(check2))
                return false;

            return walkable[check1.x, check1.y] &&
                   walkable[check2.x, check2.y];
        }

        List<Vector2Int> ReconstructPath(
            Dictionary<Vector2Int, Vector2Int> cameFrom,
            Vector2Int start,
            Vector2Int target)
        {
            List<Vector2Int> path = new();
            Vector2Int current = target;

            while (current != start)
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 local = worldPos - levelBounds.min;

            int x = Mathf.FloorToInt(local.x / cellSize);
            int z = Mathf.FloorToInt(local.z / cellSize);

            return new Vector2Int(x, z);
        }

        Vector3 GridToWorld(Vector2Int gridPos)
        {
            return levelBounds.min +
                   new Vector3(
                       gridPos.x * cellSize + cellSize * 0.5f,
                       0f,
                       gridPos.y * cellSize + cellSize * 0.5f
                   );
        }

        bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 &&
                   pos.x < width && pos.y < height;
        }

        Bounds CalculateLevelBounds()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            return bounds;
        }

        void OnDrawGizmosSelected()
        {
            if (!drawGizmos || walkable == null) return;

            Vector3 origin = levelBounds.min;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Gizmos.color = walkable[x, z] ? Color.green : Color.red;

                    Gizmos.DrawWireCube(
                        new Vector3(
                            origin.x + x * cellSize + cellSize * 0.5f,
                            origin.y,
                            origin.z + z * cellSize + cellSize * 0.5f
                        ),
                        new Vector3(cellSize, 0.1f, cellSize)
                    );
                }
            }
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(LevelGridScanner))]
        class LevelGridBFSEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                LevelGridScanner grid = (LevelGridScanner)target;

                GUILayout.Space(10);
                if (GUILayout.Button("Build Level Grid"))
                {
                    grid.BuildGrid();
                    EditorUtility.SetDirty(grid);
                }
            }
        }
#endif
    }
}