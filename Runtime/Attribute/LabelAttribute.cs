using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 标签特性
    /// </summary>
    /// <remarks>
    /// <b>使用：</b>
    /// <br/>
    /// 使用该特性标记的非集合类型字段（包括枚举元素）将在检视窗口中显示自定义的名称与提示。
    /// <br/>
    /// 使用该特性标记的集合类型字段（数组或列表）中的元素将在检视窗口中显示自定义的名称与提示。
    /// <para/>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型的特性标签对字段进行标记时，应位于 <see cref="global::MoShan.Unity.EngineExpand"/> 命名空间中的其它继承了 PropertyAttribute 的特性标签之前，否则特性效果可能不会生效。
    /// <br/>
    /// 该类型的特性标签对字段进行标记时，应位于其它 Unity 原生特性标签之前，否则特性效果可能不会生效。
    /// <br/>
    /// 该类型所在的脚本文件应放置在非 Editor 文件夹下，否则会因为找不到该类型而导致报错。
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// 
    /// [Label("名称", "提示")]
    /// public string Example = string.Empty;
    /// 
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)] // 仅对字段生效；单个字段上不允许添加多个该属性；不可继承
    public sealed class LabelAttribute : PropertyAttribute
    {
        #region 字段
        /// <summary>
        /// 名称
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 提示
        /// </summary>
        /// <remarks>
        /// 指定当光标悬停在 <see cref="UnityEditor.InspectorWindow">检视窗口</see> 中的该特性修饰的字段上时会显示的工具提示信息。
        /// </remarks>
        public readonly string Tooltip;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">指定需要显示在 <see cref="UnityEditor.InspectorWindow">检视窗口</see> 中的自定义字段名称。</param>
        public LabelAttribute(string name)
        {
            Name    = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
            Tooltip = string.Empty;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">指定需要显示在 <see cref="UnityEditor.InspectorWindow">检视窗口</see> 中的自定义字段名称。</param>
        /// <param name="tooltip">指定当光标悬停在 <see cref="UnityEditor.InspectorWindow">检视窗口</see> 中的对应字段上时，需要显示的自定义提示信息</param>
        public LabelAttribute(string name, string tooltip)
        {
            Name    = string.IsNullOrWhiteSpace(name)    ? string.Empty : name;
            Tooltip = string.IsNullOrWhiteSpace(tooltip) ? string.Empty : tooltip;
        }
        #endregion
    }
}
