#if UNITY_EDITOR
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Rect         = global::UnityEngine.Rect;
    using ColorUtility = global::UnityEngine.ColorUtility;

    /// <summary>
    /// 属性绘制器：<see cref="UnityEngine.Color">颜色</see>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件需要放置在【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    [CustomPropertyDrawer(typeof(Color))] // 设置自定义属性绘制器对应的类型
    internal sealed class ColorDrawer : PropertyDrawer
    {
        #region 常量
        /// <summary>
        /// 间隔
        /// </summary>
        private const float SPACING = 5.0f;

        /// <summary>
        /// 【十六进制颜色】字段宽度
        /// </summary>
        private const float HEX_FIELD_WIDTH = 60.0f;
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 绘制 GUI 时
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">绘制属性</param>
        /// <param name="label">绘制属性的 GUI 内容</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 开始【绘制属性】
            label = EditorGUI.BeginProperty(position, label, property);

            // 绘制【前缀标签】，并设置控件不接收键盘焦点
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // 获取【缩进等级】
            int indent = EditorGUI.indentLevel;

            // 重置【缩进等级】
            EditorGUI.indentLevel = 0;

            #region 绘制内容
            // 获取【颜色字段宽度】
            float colorFieldWidth = position.width - HEX_FIELD_WIDTH - SPACING;

            // 绘制【颜色字段】
            Color newColor = EditorGUI.ColorField(new Rect(position.x, position.y, colorFieldWidth, position.height), property.colorValue);

            // 判断 <新值是否不等于原值>
            if (!newColor.Equals(property.colorValue))
            {
                property.colorValue = newColor;
            }

            // 绘制文本字段【十六进制颜色字符串】
            string hex = EditorGUI.TextField
            (
                new Rect(position.x + colorFieldWidth + SPACING, position.y, HEX_FIELD_WIDTH, position.height),
                ColorUtility.ToHtmlStringRGB(property.colorValue)
            );

            // 判断 <尝试转换【十六进制颜色字符串】为【RGB 颜色】是否成功>
            if (ColorUtility.TryParseHtmlString(hex, out newColor))
            {
                // 判断 <新值是否不等于原值>
                if (!newColor.Equals(property.colorValue))
                {
                    property.colorValue = newColor;
                }
            }
            #endregion

            // 恢复【缩进等级】
            EditorGUI.indentLevel = indent;

            // 结束【绘制属性】
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// 获取【属性高度】
        /// </summary>
        /// <param name="property">序列化属性</param>
        /// <param name="label">绘制属性的 GUI 内容</param>
        /// <returns>返回属性在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】的绘制高度</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        #endregion
    }
}
#endif
