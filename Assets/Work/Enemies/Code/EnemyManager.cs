using Assets.Work.Maps.Code;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyManager : MonoBehaviour
    {
        private class PathNode
        {
            public Vector3 targetPos;
            public List<Vector3> path;
        }

        [SerializeField] private MapManager mapManager; 
        [field: SerializeField] public List<GameObject> objects;
        [field: SerializeField] public Transform target;

        private Dictionary<Guid, PathNode> pathNodes = new Dictionary<Guid, PathNode>();

        [Header("MoveManage")]
        public float cohesionWeight = 1.0f;     // 3규칙 가중치
        public float alignmentWeight = 1.0f;
        public float separationWeight = 1.0f;
        public float moveRadiusRange = 5.0f;    // 활동 범위 반지름
        public float boundaryForce = 3.5f;      // 범위 내로 돌아가게 하는 힘
        public float maxSpeed = 2.0f;
        public float neighborDistance = 3.0f;   // 이웃 탐색 범위
        public float maxNeighbors = 50;         // 이웃 탐색 수 제한

        private List<ICrowd> neighdors = new List<ICrowd>();
        public List<ICrowd> Neighdors => neighdors;

        public void Awake()
        {
            foreach (var item in objects)
            {
                neighdors.Add(item.GetComponent<ICrowd>());
            }

            foreach (var item in Neighdors)
            {
                if (item.Transform.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.Init(this);
                }
            }
        }

        public Vector3 GetNextMove(Vector3 startPos, Vector3 targetPos, Guid guid)
        {
            Vector3 nextPos = startPos;

            if (pathNodes.TryGetValue(guid, out PathNode pathNode))
            {
                if (Vector3.Distance(pathNode.targetPos, targetPos) > 0.25f)
                {
                    pathNode = new PathNode
                    {
                        targetPos = targetPos,
                        path = mapManager.GetMovePath(startPos, targetPos)
                    };
                    pathNodes[guid] = pathNode;

                    if (pathNode.path.Count <= 1)
                    {
                        return startPos;
                    }
                    else
                    {
                        nextPos = pathNode.path[1];
                        if (Vector3.Distance(startPos, nextPos) < 0.1f)
                        {
                            pathNode.path.RemoveAt(0);
                            if (pathNode.path.Count <= 1)
                            {
                                return startPos;
                            }
                            nextPos = pathNode.path[1];
                        }
                    }
                }
                else
                {
                    if (pathNode.path.Count <= 1)
                    {
                        return startPos;
                    }
                    else
                    {
                        nextPos = pathNode.path[1];
                        if (Vector3.Distance(startPos, nextPos) < 0.1f)
                        {
                            pathNode.path.RemoveAt(0);
                            if (pathNode.path.Count <= 1)
                            {
                                return startPos;
                            }
                            nextPos = pathNode.path[1];
                        }
                        
                    }
                }
            }
            else
            {
                pathNode = new PathNode
                {
                    targetPos = targetPos,
                    path = mapManager.GetMovePath(startPos, targetPos)
                };
                pathNodes.Add(guid, pathNode);
                nextPos = pathNode.path.Count > 1 ? pathNode.path[1] : startPos;
                Debug.Log($"{nextPos}");
            }

            return nextPos;
        }
    }
}
