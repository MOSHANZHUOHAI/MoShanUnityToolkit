#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    /// <summary>
    /// 编辑器实用程序：检视窗口编辑器
    /// </summary>
    public static partial class InspectorEditorUtility
    {
        #region 类型
        /// <summary>
        /// 设置提供器：检视窗口编辑器
        /// </summary>
        private sealed class InspectorEditorSettingsProvider : SettingsProvider
        {
            #region 常量
            /// <summary>
            /// 路径
            /// </summary>
            private const string PATH = "MoShan/检视窗口编辑器";

            /// <summary>
            /// 作用域
            /// </summary>
            /// <remarks>
            /// <br><see cref="SettingsScope.User">用户</see> = 首选项窗口</br>
            /// <br><see cref="SettingsScope.Project">项目</see> = 项目设置窗口</br>
            /// </remarks>
            private const SettingsScope SCOPES = SettingsScope.User;

            /// <summary>
            /// 关键词
            /// </summary>
            private static readonly IEnumerable<string> KEYWORDS = new HashSet<string>(new[] { "Is", "Show", "Script", "Target", "Editor" });
            #endregion

            #region 构造方法
            /// <summary>
            /// 构造方法
            /// </summary>
            public InspectorEditorSettingsProvider() : base(PATH, SCOPES, KEYWORDS)
            {
                // 设置【名称】
                label = "检视窗口编辑器";
            }
            #endregion

            #region 公开方法
            /// <summary>
            /// 绘制 GUI 时
            /// </summary>
            /// <param name="searchContext">搜索上下文</param>
            public override void OnGUI(string searchContext)
            {
                base.OnGUI(searchContext);

                IsShowTargetScript = EditorGUILayout.Toggle
                (
                    new GUIContent("显示目标脚本", "显示检视窗口编辑器拓展的【目标类型的脚本】到检视窗口中对应区块的顶部。"),
                    s_IsShowTargetScript
                );

                IsShowEditorScript = EditorGUILayout.Toggle
                (
                    new GUIContent("显示编辑器脚本", "显示检视窗口编辑器拓展的【自身类型的脚本】到检视窗口中对应区块的顶部。"),
                    s_IsShowEditorScript
                );
            }
            #endregion
        }
        #endregion
    }
}
#endif
