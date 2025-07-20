using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 按钮特性
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>使用：</b></br>
    /// <br>使用该特性标记的【方法】将在检视窗口底部显示自定义名称的按钮。</br>
    /// </para>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件应放置在非【Editor】文件夹下，否则会因为找不到该类型而导致报错。</br>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// 
    /// [Button("按钮名称")]
    /// public void Example()
    /// {
    ///     // TODO: 在方法中运行的逻辑
    /// }
    /// 
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)] // 仅对方法生效，不可继承，单个方法上不允许添加多个该属性
    public sealed class ButtonAttribute : PropertyAttribute
    {
        #region 字段
        /// <summary>
        /// 名称
        /// </summary>
        public readonly string Name;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public ButtonAttribute() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">需要显示在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】中的自定义按钮名称</param>
        public ButtonAttribute(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        }
        #endregion
    }
}
