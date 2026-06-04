using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 场景名称特性
    /// </summary>
    /// <remarks>
    /// <b>使用：</b>
    /// <br/>
    /// 使用该特性标记的字符串字段将在检视窗口中显示为元素为场景名称的枚举字段。
    /// <para/>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型的特性标签对字段进行标记时，应位于 <see cref="global::MoShan.Unity.EngineExpand"/> 命名空间中的其它继承了 PropertyAttribute 的特性标签之前，否则特性效果可能不会生效。
    /// <br/>
    /// 该类型所在的脚本文件应放置在非 Editor 文件夹下，否则会因为找不到该类型而导致报错。
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// 
    /// [SceneName]
    /// public string Example = string.Empty;
    /// 
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)] // 仅对字段生效，不可继承，单个字段上不允许添加多个该属性
    public class SceneNameAttribute : PropertyAttribute
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public SceneNameAttribute() { }
        #endregion
    }
}
