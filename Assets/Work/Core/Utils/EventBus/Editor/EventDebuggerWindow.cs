#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Alchemy.Editor;
using Alchemy.Inspector;

namespace Work.Core.Utils.EventBus.Editor
{
    /// <summary>
    /// Event log viewer window built with AlchemyEditorWindow (attribute-driven UI).
    /// </summary>
    public sealed class EventDebuggerWindow : AlchemyEditorWindow
    {
        private const int DefaultMaxLogs = 200;

        [Serializable]
        [HorizontalGroup]
        public sealed class LogItem
        {
            [LabelWidth(44f)]
            [ReadOnly]
            [LabelText("Time")]
            public string time;

            [ReadOnly]
            [HideLabel]
            public string message;

            public LogItem(string time, string message)
            {
                this.time = time;
                this.message = message;
            }
        }

        // Controls
        [FoldoutGroup("Controls")]
        [LabelText("Max Logs")]
        public int maxLogs = DefaultMaxLogs;

        [FoldoutGroup("Controls")]
        [LabelText("Pause")]
        public bool pause;

        [FoldoutGroup("Controls")]
        [LabelText("Show Timestamp")]
        public bool showTimestamp = true;

        // Logs
        [Title("Logs")]
        [ListViewSettings(ShowAlternatingRowBackgrounds = AlternatingRowBackground.All, ShowFoldoutHeader = false)]
        public List<LogItem> logs = new();

        [MenuItem("Window/Event Debugger")]
        public static void ShowWindow()
        {
            var window = GetWindow<EventDebuggerWindow>("Event Debugger");
            window.Show();
        }

        private void OnEnable()
        {
            // Safety: clamp in case persisted data is invalid.
            maxLogs = Mathf.Clamp(maxLogs, 10, 5000);

            EventTrackerManager.OnGlobalLog += AddLog;
        }

        private void OnDisable()
        {
            EventTrackerManager.OnGlobalLog -= AddLog;
        }

        [Button, HorizontalGroup]
        public void Clear()
        {
            logs.Clear();
            Repaint();
        }

        [Button, HorizontalGroup]
        public void CopyAllToClipboard()
        {
            if (logs == null || logs.Count == 0)
            {
                EditorGUIUtility.systemCopyBuffer = string.Empty;
                return;
            }

            var lines = new System.Text.StringBuilder(logs.Count * 64);
            for (int i = 0; i < logs.Count; i++)
            {
                var item = logs[i];
                lines.Append('[').Append(item.time).Append("] ").AppendLine(item.message);
            }

            EditorGUIUtility.systemCopyBuffer = lines.ToString();
        }

        private void AddLog(string log)
        {
            if (pause) return;

            var time = DateTime.Now.ToString("HH:mm:ss");
            var message = showTimestamp ? log : StripLeadingTimestamp(log);

            logs.Add(new LogItem(time, message));

            var cap = Mathf.Clamp(maxLogs, 10, 5000);
            while (logs.Count > cap)
                logs.RemoveAt(0);

            // Forces UI refresh (EditorWindow API).
            Repaint();
        }

        private static string StripLeadingTimestamp(string text)
        {
            // Best-effort: if the string already starts with "[HH:mm:ss] " remove it.
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (text.Length >= 11 && text[0] == '[' && text[9] == ']' && text[10] == ' ')
                return text.Substring(11);
            return text;
        }
    }
}
#endif
