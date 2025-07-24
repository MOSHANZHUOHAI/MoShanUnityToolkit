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
        #region 字段
        /// <summary>
        /// 是否显示目标脚本
        /// </summary>
        private static bool s_IsShowTargetScript = true;

        /// <summary>
        /// 是否显示编辑器脚本
        /// </summary>
        private static bool s_IsShowEditorScript = false;
        #endregion

        #region 属性
        /// <summary>
        /// 是否显示目标脚本
        /// </summary>
        public static bool IsShowTargetScript
        {
            get
            {
                return s_IsShowTargetScript;
            }
            set
            {
                // 判断 <【是否显示目标脚本】是否等于【输入值】>
                if (s_IsShowTargetScript == value)
                {
                    return;
                }

                s_IsShowTargetScript = value;

                EditorPrefs.SetBool($"{nameof(InspectorEditorUtility)}.{nameof(s_IsShowTargetScript)}", value);
            }
        }

        /// <summary>
        /// 是否显示编辑器脚本
        /// </summary>
        public static bool IsShowEditorScript
        {
            get
            {
                return IsShowEditorScript;
            }
            set
            {
                // 判断 <【是否显示编辑器脚本】是否等于【输入值】>
                if (s_IsShowEditorScript == value)
                {
                    return;
                }

                s_IsShowEditorScript = value;

                EditorPrefs.SetBool($"{nameof(InspectorEditorUtility)}.{nameof(s_IsShowTargetScript)}", value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static InspectorEditorUtility()
        {
            s_IsShowTargetScript = EditorPrefs.GetBool($"{nameof(InspectorEditorUtility)}.{nameof(s_IsShowTargetScript)}", true);
            s_IsShowEditorScript = EditorPrefs.GetBool($"{nameof(InspectorEditorUtility)}.{nameof(s_IsShowEditorScript)}", false);
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
