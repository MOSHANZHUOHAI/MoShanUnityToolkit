using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 六方向
    /// </summary>
    [Serializable]
    public enum Direction6 : byte
    {
        /// <summary>
        /// 上方
        /// </summary>
        Up    = Direction4.Up,
        /// <summary>
        /// 下方
        /// </summary>
        Down  = Direction4.Down,
        /// <summary>
        /// 左侧
        /// </summary>
        Left  = Direction4.Left,
        /// <summary>
        /// 右侧
        /// </summary>
        Right = Direction4.Right,
        /// <summary>
        /// 前方
        /// </summary>
        Front = 4,
        /// <summary>
        /// 后方
        /// </summary>
        Back  = 5
    }
}
