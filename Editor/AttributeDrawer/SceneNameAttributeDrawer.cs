#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Path = global::System.IO.Path;

    /// <summary>
    /// 属性绘制器：场景名称特性
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型所在的脚本文件需要放置在 Editor 文件夹下；
    /// <br/>
    /// 该类型需要配合 <see cref="SceneNameAttribute">场景名称特性</see> 类型使用，且对应类型所在的脚本文件应放置在非 Editor 文件夹下。
    /// </remarks>
    [CustomPropertyDrawer(typeof(SceneNameAttribute))] // 设置自定义属性绘制器对应的类型
    internal sealed class SceneNameAttributeDrawer : PropertyDrawer
    {
        #region 静态私有方法
        /// <summary>
        /// 获取构建设置中的所有场景名称
        /// </summary>
        /// <returns>
        /// 如果 <see cref="EditorBuildSettings.scenes">构建设置</see> 中存在场景，返回值为构建设置中的所有场景的名称；
        /// <br/>
        /// 如果 <see cref="EditorBuildSettings.scenes">构建设置</see> 中不存在场景，返回值为 <see cref="Array.Empty{T}">空数组</see>。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string[] GetBuildSettingsSceneNames()
        {
            // 判断 <构建设置中是否不存在任何场景资产>
            if (EditorBuildSettings.scenes.Length == 0)
            {
                // 返回空数组
                return new string[0];
            }

            // 获取构建设置中的所有场景
            EditorBuildSettingsScene[] editorBuildSettingScenes = EditorBuildSettings.scenes;

            // 创建场景名称数组
            string[] sceneNames = new string[editorBuildSettingScenes.Length + 1];

            // 设置场景名称数组的首个元素为无
            sceneNames[0] = "<None>";

            // 循环以记录构建设置中的所有场景名称到场景名称数组
            for (int i = 1; i <= editorBuildSettingScenes.Length; i++)
            {
                // 获取不附带文件扩展名的文件名称，即场景名称
                sceneNames[i] = Path.GetFileNameWithoutExtension(editorBuildSettingScenes[i - 1].path);
            }

            return sceneNames;
        }
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 绘制 GUI 时
        /// </summary>
        /// <param name="position">指定需要绘制的 <paramref name="property"/> 的绘制区域。</param>
        /// <param name="property">指定需要绘制的序列化属性。</param>
        /// <param name="label">指定需要绘制的 <paramref name="property"/> 的 GUI 内容。</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 判断 <属性类型是否不为字符串>
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "标记字段的类型不是字符串 string，请选择正确的字段。", MessageType.Error);

                return;
            }

            // 开始绘制属性
            label = EditorGUI.BeginProperty(position, label, property);

            // 获取当前场景名称
            string currentSceneName = property.stringValue;

            // 获取构建设置中的所有场景名称
            string[] sceneNames = GetBuildSettingsSceneNames();

            // 判断 <构建设置中是否不存在任何场景>
            if (sceneNames.Length == 0)
            {
                property.stringValue = string.Empty;

                EditorGUI.Popup(position, label.text, 0, sceneNames);
            }

            // 开始 GUI 变更检测
            EditorGUI.BeginChangeCheck();

            // 获取索引
            int index = 0;

            // 循环以查找构建设置中的所有场景名称中是否存在对应当前场景名称的元素
            for (int i = 1; i < sceneNames.Length; ++i)
            {
                // 判断 <是否存在对应的场景名称>
                if (sceneNames[i].Equals(currentSceneName))
                {
                    index = i;

                    break;
                }
            }

            index = EditorGUI.Popup(position, label.text, index, sceneNames);

            // 结束 GUI 变更检测，并判断 <是否未发生 GUI 变更>
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            // 判断 <索引是否为 0>
            if (index == 0)
            {
                property.stringValue = string.Empty;
            }
            else
            {
                property.stringValue = sceneNames[index];
            }

            // 结束绘制属性
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property">指定需要绘制的序列化属性。</param>
        /// <param name="label">指定需要绘制的 <paramref name="property"/> 的 GUI 内容。</param>
        /// <returns>返回值为属性在 <see cref="global::UnityEditor.InspectorWindow">检视窗口</see> 的绘制高度。</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        #endregion
    }
}
#endif
