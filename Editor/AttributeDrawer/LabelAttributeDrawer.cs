#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 属性绘制器：标签特性
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型所在的脚本文件需要放置在 Editor 文件夹下；
    /// <br/>
    /// 该类型需要配合 <see cref="LabelAttribute">标签特性</see> 类型使用，且对应类型所在的脚本文件应放置在非 Editor 文件夹下。
    /// </remarks>
    [CustomPropertyDrawer(typeof(LabelAttribute))] // 设置自定义属性绘制器对应的类型
    internal sealed class LabelAttributeDrawer : PropertyDrawer
    {
        #region 静态私有方法
        /// <summary>
        /// 判断是否为标记枚举
        /// </summary>
        /// <param name="property">指定需要判断类型是否为标记枚举类型的序列化属性。</param>
        /// <returns>返回值为 <paramref name="property"/> 的类型是否为标记枚举类型的判断结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEnumFlags(SerializedProperty property)
        {
            // 获取输入序列化属性的字段信息
            FieldInfo fieldInfo = property.serializedObject.targetObject.GetType().GetField(property.name);

            // 判断 <字段信息是否为空值>
            if (fieldInfo == null)
            {
                return false;
            }

            // 判断 <字段的类型是否为枚举类型>、<字段的类型是否实现了标记特性 FlagsAttribute>
            return fieldInfo.FieldType.IsEnum && Attribute.IsDefined(fieldInfo.FieldType, typeof(FlagsAttribute));
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
            LabelAttribute target = attribute as LabelAttribute;

            // 使用正则表达式判断属性的显示名称是否不匹配模式 "Element \d+"，即是否以 "Element " 作为开头，且后面跟着一个或多个数字
            // 如果属性是数组或列表的元素属性，为了在检视面板中正确显示每个元素的标签，将属性的显示名称直接赋值给标签的文本
            if (!Regex.IsMatch(property.displayName, "Element \\d+"))
            {
                // 判断 <属性的自定义标签名称是否不为空值>
                if (!string.IsNullOrWhiteSpace(target.Name))
                {
                    // 设置标签为自定义标签
                    label.text = target.Name;
                }
            }

            // 更新提示
            label.tooltip = target.Tooltip;

            // 获取缩进后的字段绘制区域
            Rect fieldRect = EditorGUI.IndentedRect(position);

            // 判断 <属性类型是否不为枚举>
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                // 绘制字段
                EditorGUI.PropertyField(fieldRect, property, label, true);

                return;
            }

            // 替换枚举名称
            // 使用正则表达式判断属性的显示名称是否匹配模式 "Element \d+"，即是否以 "Element " 作为开头，且后面跟着一个或多个数字
            // 如果属性是数组或列表的元素属性，为了在检视面板中正确显示每个元素的标签，将属性的显示名称直接赋值给标签的文本
            if (Regex.IsMatch(property.displayName, "Element \\d+"))
            {
                label.text = property.displayName;
            }

            // 开始编辑器 GUI 变更检测
            EditorGUI.BeginChangeCheck();

            // 获取枚举类型
            Type type = fieldInfo.FieldType;

            // 获取枚举所对应的名称
            string[] names  = property.enumNames;
            string[] values = new string[names.Length];

            // While 循环以获取集合中的元素类型
            while (type.IsArray)
            {
                type = type.GetElementType();
            }

            // 循环以获取枚举所对应的名称
            for (int i = 0; i < names.Length; i++)
            {
                // 获取类型下的所有字段
                FieldInfo info = type.GetField(names[i]);

                LabelAttribute[] attributes = (LabelAttribute[])info.GetCustomAttributes(typeof(LabelAttribute), false);

                // 若不存在自定义标签特性标记，则保留原有枚举元素名称
                values[i] = attributes.Length == 0 ? names[i] : attributes[0].Name;
            }

            // 判断 <序列化属性是否为多选枚举>
            if (IsEnumFlags(property))
            {
                // 绘制多选枚举字段
                property.intValue = EditorGUI.MaskField(fieldRect, label, property.intValue, values);

                // 结束编辑器 GUI 变更检测
                EditorGUI.EndChangeCheck();
            }
            else
            {
                // 绘制枚举字段
                int index = EditorGUI.Popup(fieldRect, label.text, property.enumValueIndex, values);

                // 结束编辑器 GUI 变更检测，并判断 <是否发生编辑器 GUI 变更>
                if (EditorGUI.EndChangeCheck() && index != -1)
                {
                    property.enumValueIndex = index;
                }
            }
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
