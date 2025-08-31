using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 三方向
    /// </summary>
    [Serializable]
    public enum Direction3 : byte
    {
        /// <summary>
        /// 水平
        /// </summary>
        Horizontal = Direction2.Horizontal,
        /// <summary>
        /// 垂直
        /// </summary>
        Vertical   = Direction2.Vertical,
        /// <summary>
        /// 纵深
        /// </summary>
        Depth      = 2,
    }
}
