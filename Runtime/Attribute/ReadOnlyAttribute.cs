using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 只读特性
    /// </summary>
    /// <remarks>
    /// <b>使用：</b>
    /// <br/>
    /// 使用该特性标记的字段将禁止在检视窗口中进行编辑。
    /// <para/>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型的特性标签对字段进行标记时，应位于<see cref="global::MoShan.Unity.EngineExpand"/> 命名空间中的其它继承了 PropertyAttribute 的特性标签之前，否则特性效果可能不会生效。
    /// <br/>
    /// 该类型的特性标签对字段进行标记时，应位于其它 Unity 原生特性标签之前，否则特性效果可能不会生效。
    /// <br/>
    /// 该类型所在的脚本文件应放置在非【Editor】文件夹下，否则会因为找不到该类型而导致报错。
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// 
    /// [ReadOnly]
    /// public string Example = string.Empty;
    /// 
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)] // 仅对字段生效；单个字段上不允许添加多个该属性；不可继承
    public sealed class ReadOnlyAttribute : PropertyAttribute
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public ReadOnlyAttribute() { }
        #endregion
    }
}
