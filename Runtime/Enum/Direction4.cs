using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 四方向
    /// </summary>
    [Serializable]
    public enum Direction4 : byte
    {
        /// <summary>
        /// 上方
        /// </summary>
        Up    = 0,
        /// <summary>
        /// 下方
        /// </summary>
        Down  = 1,
        /// <summary>
        /// 左侧
        /// </summary>
        Left  = 2,
        /// <summary>
        /// 右侧
        /// </summary>
        Right = 3,
    }
}
