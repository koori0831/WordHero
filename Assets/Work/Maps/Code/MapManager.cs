using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Work.Maps.Code;

namespace Assets.Work.Maps.Code
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private LevelGridScanner scanner;

        public void Awake()
        {
            scanner.BuildGrid();
        }

        public List<Vector3> GetMovePath(Vector3 startPos, Vector3 targetPos)
        {
            if (scanner.GetMovePath(startPos, targetPos, out var path))
            {
                Debug.Log($"[MapManager] Path Found : {path.Count} points");
                return path;
            }
            return new List<Vector3>();
        }
    }
}