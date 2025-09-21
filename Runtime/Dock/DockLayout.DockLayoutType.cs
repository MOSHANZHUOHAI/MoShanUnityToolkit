using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 停靠布局
    /// </summary>
    internal partial class DockLayout
    {
        /// <summary>
        /// 停靠布局类型
        /// </summary>
        [Serializable]
        private enum DockLayoutType : int
        {
            /// <summary>
            /// 固定
            /// </summary>
            /// <remarks>
            /// <para>可用于【根布局】与【子布局】。</para>
            /// <para>
            /// <br>允许从外部变更自身的位置与尺寸；</br>
            /// <br>允许由父级变更自身的位置与尺寸。</br>
            /// </para>
            /// <para>
            /// <br>禁止从内部变更自身的位置与尺寸。</br>
            /// </para>
            /// </remarks>
            Fixed = 0,
            /// <summary>
            /// 浮动
            /// </summary>
            /// <remarks>
            /// <para>可用于【根布局】。</para>
            /// <para>
            /// <br>允许从外部变更自身的位置与尺寸；</br>
            /// <br>允许从内部变更自身的位置与尺寸。</br>
            /// </para>
            /// </remarks>
            Floating = 1,
        }
    }
}
