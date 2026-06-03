#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using System;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 属性绘制器：只读特性
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型所在的脚本文件需要放置在 Editor 文件夹下；
    /// <br/>
    /// 该类型需要配合 <see cref="ReadOnlyAttribute">只读特性</see> 类型使用，且对应类型所在的脚本文件应放置在非 Editor 文件夹下。
    /// </remarks>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))] // 设置自定义属性绘制器对应的类型
    internal sealed class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        #region 生命周期方法
        /// <summary>
        /// 绘制 GUI 时
        /// </summary>
        /// <param name="position">指定需要绘制的 <paramref name="property"/> 的绘制区域。</param>
        /// <param name="property">指定需要绘制的序列化属性。</param>
        /// <param name="label">指定需要绘制的 <paramref name="property"/> 的 GUI 内容。</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 缓存 GUI 启用状态
            bool cacheEnable = GUI.enabled;

            // 禁用 GUI 交互
            GUI.enabled = false;

            // 绘制属性字段
            EditorGUI.PropertyField(position, property, label, true);

            // 重置 GUI 启用状态
            GUI.enabled = cacheEnable;
        }

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property">指定需要绘制的序列化属性。</param>
        /// <param name="label">指定需要绘制的 <paramref name="property"/> 的 GUI 内容。</param>
        /// <returns>返回值为属性在 <see cref="global::UnityEditor.InspectorWindow">检视窗口</see> 的绘制高度。</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        #endregion
    }
}
#endif
