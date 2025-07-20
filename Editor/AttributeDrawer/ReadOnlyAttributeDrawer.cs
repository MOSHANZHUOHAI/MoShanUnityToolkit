#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 属性绘制器：只读特性
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件需要放置在【Editor】文件夹下；</br>
    /// <br>该类型需要配合【<see cref="ReadOnlyAttribute">只读特性</see>】类型使用，且对应类型所在的脚本文件应放置在非【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))] // 设置自定义属性绘制器对应的类型
    internal sealed class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        #region 生命周期方法
        /// <summary>
        /// 绘制 GUI 时
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">绘制属性</param>
        /// <param name="label">绘制属性的 GUI 内容</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 禁用 GUI 交互
            GUI.enabled = false;

            // 绘制【属性字段】
            EditorGUI.PropertyField(position, property, label, true);

            // 禁用 GUI 交互
            GUI.enabled = true;
        }

        /// <summary>
        /// 获取【属性高度】
        /// </summary>
        /// <param name="property">序列化属性</param>
        /// <param name="label">绘制属性的 GUI 内容</param>
        /// <returns>返回属性在【<see cref="global::UnityEditor.InspectorWindow">检视窗口</see>】的绘制高度</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        #endregion
    }
}
#endif
