using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 场景名称特性
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>使用：</b></br>
    /// <br>使用该特性标记的【字符串字段】将在检视窗口中显示为元素为场景名称的枚举字段。</br>
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
