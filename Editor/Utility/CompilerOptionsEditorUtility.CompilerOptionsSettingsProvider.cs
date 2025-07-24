#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    /// <summary>
    /// 编辑器实用程序：编译选项
    /// </summary>
    internal static partial class CompilerOptionsEditorUtility
    {
        /// <summary>
        /// 设置提供器：编译选项
        /// </summary>
        private sealed class CompilerOptionsSettingsProvider : SettingsProvider
        {
            #region 常量
            /// <summary>
            /// 路径
            /// </summary>
            private const string PATH = "MoShan/编译选项";

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
            private static readonly IEnumerable<string> KEYWORDS = new HashSet<string>(new[] { "Is", "Lock", "CompilerOptions" });
            #endregion

            #region 构造方法
            /// <summary>
            /// 构造方法
            /// </summary>
            public CompilerOptionsSettingsProvider() : base(PATH, SCOPES, KEYWORDS)
            {
                // 设置【名称】
                label = "编译选项";
            }
            #endregion

            #region 公开方法
            /// <summary>
            /// 绘制 GUI 时
            /// </summary>
            /// <param name="searchContext">搜索上下文</param>
            public override void OnGUI(string searchContext)
            {
                IsLock = EditorGUILayout.Toggle
                (
                    new GUIContent("锁定程序集重载", "当脚本发生变更时，程序集不进行编译。"),
                    s_IsLock
                );

                IsLockOnPlaying = EditorGUILayout.Toggle
                (
                    new GUIContent("锁定运行时程序集重载", "若编辑器处于运行时，当脚本发生变更时，程序集不进行编译。"),
                    s_IsLockOnPlaying
                );
            }
            #endregion
        }
    }
}
#endif
