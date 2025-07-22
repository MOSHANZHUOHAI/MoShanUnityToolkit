#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using System;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Path = global::System.IO.Path;
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 属性绘制器：场景名称特性
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件需要放置在【Editor】文件夹下；</br>
    /// <br>该类型需要配合【<see cref="SceneNameAttribute">场景名称特性</see>】类型使用，且对应类型所在的脚本文件应放置在非【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    [CustomPropertyDrawer(typeof(SceneNameAttribute))] // 设置自定义属性绘制器对应的类型
    public class SceneNameAttributeDrawer : PropertyDrawer
    {
        #region 静态私有方法
        /// <summary>
        /// 获取【构建设置中的所有场景名称】
        /// </summary>
        /// <returns>若【构建设置】中存在场景，返回【构建设置中的所有场景的名称】；否则，返回【空数组】。</returns>
        private static string[] GetBuildSettingsSceneNames()
        {
            // 判断 <【构建设置】中是否不存在任何场景资产>
            if (EditorBuildSettings.scenes.Length == 0)
            {
                // 返回【空数组】
                return new string[0];
            }

            // 获取【构建设置】中的【所有场景】
            EditorBuildSettingsScene[] editorBuildSettingScenes = EditorBuildSettings.scenes;

            // 创建【场景名称数组】
            string[] sceneNames = new string[editorBuildSettingScenes.Length + 1];

            // 设置【场景名称数组】的【首个元素】为【无】
            sceneNames[0] = "<None>";

            // 循环以记录【构建设置中的所有场景名称】到【场景名称数组】
            for (int i = 1; i <= editorBuildSettingScenes.Length; i++)
            {
                // 获取【不附带文件扩展名的文件名称】，即【场景名称】
                sceneNames[i] = Path.GetFileNameWithoutExtension(editorBuildSettingScenes[i - 1].path);
            }

            return sceneNames;
        }
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 绘制 GUI 时
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">绘制属性</param>
        /// <param name="label">绘制属性的标签</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 判断 <【属性类型】是否不为【字符串】>
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "标记字段类型非【字符串 string】，请选择正确的字段。", MessageType.Error);

                return;
            }

            // 开始【绘制属性】
            label = EditorGUI.BeginProperty(position, label, property);

            // 获取【当前场景名称】
            string currentSceneName = property.stringValue;

            // 获取【构建设置中的所有场景名称】
            string[] sceneNames = GetBuildSettingsSceneNames();

            // 判断 <【构建设置】中是否不存在任何场景>
            if (sceneNames.Length == 0)
            {
                property.stringValue = string.Empty;

                EditorGUI.Popup(position, label.text, 0, sceneNames);
            }

            // 开始【GUI 变更检测】
            EditorGUI.BeginChangeCheck();

            // 获取【索引】
            int index = 0;

            // 循环以查找【构建设置中的所有场景名称】中是否存在对应【当前场景名称】的元素
            for (int i = 1; i < sceneNames.Length; ++i)
            {
                // 判断 <是否存在对应的【场景名称】>
                if (sceneNames[i].Equals(currentSceneName))
                {
                    index = i;

                    break;
                }
            }

            index = EditorGUI.Popup(position, label.text, index, sceneNames);

            // 结束【GUI 变更检测】，并判断 <是否未发生 GUI 变更>
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            // 判断 <【索引】是否为【0】>
            if (index == 0)
            {
                property.stringValue = string.Empty;
            }
            else
            {
                property.stringValue = sceneNames[index];
            }

            // 结束【绘制属性】
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// 获取【属性高度】
        /// </summary>
        /// <param name="property">序列化属性</param>
        /// <param name="label">GUI内容</param>
        /// <returns>返回属性在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】的绘制高度</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        #endregion
    }
}
#endif
