using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 停靠状态
    /// </summary>
    [Serializable]
    internal enum DockState : int
    {
        /// <summary>
        /// 已关闭
        /// </summary>
        Closed = -1,
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// 正在拖拽选项卡
        /// </summary>
        DraggingTab,
    }
}
