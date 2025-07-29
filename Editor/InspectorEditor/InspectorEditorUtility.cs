#if UNITY_EDITOR
using System;
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
        #region 常量
        /// <summary>
        /// 编辑器首选项键：是否显示目标脚本
        /// </summary>
        private const string IS_DISPLAY_TARGET_SCRIPT_EDITOR_PREFS_KEY = nameof(TransformInspectorEditor) + "." + nameof(s_IsDisplayTargetScript);

        /// <summary>
        /// 编辑器首选项键：是否显示目标脚本
        /// </summary>
        private const string IS_DISPLAY_EDITOR_SCRIPT_EDITOR_PREFS_KEY = nameof(TransformInspectorEditor) + "." + nameof(s_IsDisplayEditorScript);
        #endregion

        #region 字段
        /// <summary>
        /// 是否显示目标脚本
        /// </summary>
        private static bool s_IsDisplayTargetScript = true;

        /// <summary>
        /// 是否显示编辑器脚本
        /// </summary>
        private static bool s_IsDisplayEditorScript = false;
        #endregion

        #region 属性
        /// <summary>
        /// 是否显示目标脚本
        /// </summary>
        public static bool IsDisplayTargetScript
        {
            get
            {
                return s_IsDisplayTargetScript;
            }
            set
            {
                // 判断 <【是否显示目标脚本】是否等于【输入值】>
                if (s_IsDisplayTargetScript == value)
                {
                    return;
                }

                s_IsDisplayTargetScript = value;

                // 将【是否显示目标脚本】存入【编辑器首选项】
                EditorPrefs.SetBool(IS_DISPLAY_TARGET_SCRIPT_EDITOR_PREFS_KEY, value);
            }
        }

        /// <summary>
        /// 是否显示编辑器脚本
        /// </summary>
        public static bool IsDisplayEditorScript
        {
            get
            {
                return s_IsDisplayEditorScript;
            }
            set
            {
                // 判断 <【是否显示编辑器脚本】是否等于【输入值】>
                if (s_IsDisplayEditorScript == value)
                {
                    return;
                }

                s_IsDisplayEditorScript = value;

                // 将【是否显示编辑器脚本】存入【编辑器首选项】
                EditorPrefs.SetBool(IS_DISPLAY_EDITOR_SCRIPT_EDITOR_PREFS_KEY, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static InspectorEditorUtility()
        {
            // 从【编辑器首选项】中读取【是否显示目标脚本】
            s_IsDisplayTargetScript = EditorPrefs.GetBool(IS_DISPLAY_TARGET_SCRIPT_EDITOR_PREFS_KEY, true);

            // 从【编辑器首选项】中读取【是否显示编辑器脚本】
            s_IsDisplayEditorScript = EditorPrefs.GetBool(IS_DISPLAY_EDITOR_SCRIPT_EDITOR_PREFS_KEY, false);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 创建【设置提供器】
        /// </summary>
        /// <returns>返回添加条目到【首选项窗口】或【项目设置窗口】所需的设置提供器。</returns>
        [SettingsProvider] // 标记该方法可创建【设置提供器】以添加条目到【首选项窗口】或【项目设置窗口】，修饰方法必须为静态类型下的静态方法
        private static SettingsProvider CreateSettingsProvider()
        {
            return new InspectorEditorSettingsProvider();
        }
        #endregion
    }
}
#endif
