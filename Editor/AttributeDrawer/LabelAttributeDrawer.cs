#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using System;
using System.Reflection;
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
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件需要放置在【Editor】文件夹下；</br>
    /// <br>该类型需要配合【<see cref="LabelAttribute">标签特性</see>】类型使用，且对应类型所在的脚本文件应放置在非【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    [CustomPropertyDrawer(typeof(LabelAttribute))] // 设置自定义属性绘制器对应的类型
    internal sealed class LabelAttributeDrawer : PropertyDrawer
    {
        #region 生命周期方法
        /// <summary>
        /// 绘制 GUI 时
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">绘制属性</param>
        /// <param name="label">绘制属性的标签</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute target = attribute as LabelAttribute;

            // 使用正则表达式判断属性的显示名称是否不匹配模式【Element \d+】，即是否以【Element】作为开头，且后面跟着一个或多个数字
            // 若属性是数组或列表的元素属性，为了在检视面板中正确显示每个元素的标签，则将属性的显示名称直接赋值给标签的文本
            if (!Regex.IsMatch(property.displayName, "Element \\d+"))
            {
                // 判断 <【属性的自定义标签名称】是否不为【空】>
                if (!string.IsNullOrWhiteSpace(target.Name))
                {
                    // 设置【标签】为【自定义标签】
                    label.text = target.Name;
                }
            }

            // 更新【提示】
            label.tooltip = target.Tooltip;

            // 获取【缩进后的字段绘制区域】
            Rect fieldRect = EditorGUI.IndentedRect(position);

            // 判断 <属性类型是否不为【枚举】>
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                // 绘制字段
                EditorGUI.PropertyField(fieldRect, property, label, true);

                return;
            }

            // 替换枚举名称
            // 使用正则表达式判断属性的显示名称是否匹配模式 "Element \d+"，即是否以 "Element " 作为开头，且后面跟着一个或多个数字
            // 若属性是数组或列表的元素属性，为了在检视面板中正确显示每个元素的标签，则将属性的显示名称直接赋值给标签的文本
            if (Regex.IsMatch(property.displayName, "Element \\d+"))
            {
                label.text = property.displayName;
            }

            // 开始【编辑器 GUI 变更检测】
            EditorGUI.BeginChangeCheck();

            // 获取【枚举类型】
            Type type = fieldInfo.FieldType;

            // 获取【枚举所对应的名称】
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

            // 判断 <【序列化属性】是否为【多选枚举】>
            if (IsEnumFlags(property))
            {
                // 绘制【多选枚举字段】
                property.intValue = EditorGUI.MaskField(fieldRect, label, property.intValue, values);

                // 结束【编辑器 GUI 变更检测】
                EditorGUI.EndChangeCheck();
            }
            else
            {
                // 绘制【枚举字段】
                int index = EditorGUI.Popup(fieldRect, label.text, property.enumValueIndex, values);

                // 结束【编辑器 GUI 变更检测】，并判断 <是否发生编辑器 GUI 变更>
                if (EditorGUI.EndChangeCheck() && index != -1)
                {
                    property.enumValueIndex = index;
                }
            }
        }

        /// <summary>
        /// 获取【属性高度】
        /// </summary>
        /// <param name="property">序列化属性</param>
        /// <param name="label">GUI内容</param>
        /// <returns>返回属性在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】的绘制高度</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 判断【是否为标记枚举】
        /// </summary>
        /// <param name="property">序列化属性</param>
        /// <returns>返回输入序列化属性的类型是否为标记枚举类型的判断结果。</returns>
        private bool IsEnumFlags(SerializedProperty property)
        {
            FieldInfo fieldInfo = property.serializedObject.targetObject.GetType().GetField(property.name);

            // 判断 <【字段信息】是否为【空】>
            if (fieldInfo == null)
            {
                return false;
            }

            Type enumType = fieldInfo.FieldType;

            return enumType.IsEnum && Attribute.IsDefined(enumType, typeof(FlagsAttribute));
        }
        #endregion
    }
}
#endif
