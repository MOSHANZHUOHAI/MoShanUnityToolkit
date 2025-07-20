using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 标签特性
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>使用：</b></br>
    /// <br>使用该特性标记的【非集合类型字段（包括枚举元素）】将在检视窗口中显示自定义的名称与提示。</br>
    /// <br>使用该特性标记的【集合类型字段（数组或列表）】中的元素将在检视窗口中显示自定义的名称与提示。</br>
    /// </para>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型的特性标签对字段进行标记时，应位于其它 Unity 原生特性标签之前，否则特性效果可能不会生效。</br>
    /// <br>该类型所在的脚本文件应放置在非【Editor】文件夹下，否则会因为找不到该类型而导致报错。</br>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// 
    /// [Label("名称", "提示")]
    /// public string Example = string.Empty;
    /// 
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)] // 仅对字段生效，不可继承，单个字段上不允许添加多个该属性
    public sealed class LabelAttribute : PropertyAttribute
    {
        #region 字段
        /// <summary>
        /// 名称（只读）
        /// </summary>
        public readonly string m_Name;

        /// <summary>
        /// 提示（只读）
        /// </summary>
        public readonly string m_Tooltip;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">需要显示在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】中的自定义字段名称</param>
        public LabelAttribute(string name)
        {
            m_Name    = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
            m_Tooltip = string.Empty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">需要显示在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】中的自定义字段名称</param>
        /// <param name="tooltip">当光标悬停在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】中的对应字段上时，需要显示的自定义提示信息</param>
        public LabelAttribute(string name, string tooltip)
        {
            m_Name    = string.IsNullOrWhiteSpace(name)    ? string.Empty : name;
            m_Tooltip = string.IsNullOrWhiteSpace(tooltip) ? string.Empty : tooltip;
        }
        #endregion
    }
}
