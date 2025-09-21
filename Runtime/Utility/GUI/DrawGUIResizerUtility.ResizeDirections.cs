using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 实用程序：IMGUI 尺寸调整边框绘制
    /// </summary>
    public static partial class DrawGUIResizeBorderUtility
    {
        /// <summary>
        /// 尺寸调整方向
        /// </summary>
        [Flags]
        [Serializable]
        private enum ResizeDirections : byte
        {
            /// <summary>
            /// 上方
            /// </summary>
            /// <remarks>
            /// = 1
            /// </remarks>
            Up    = 1 << 0, // 1
            /// <summary>
            /// 下方
            /// </summary>
            /// <remarks>
            /// = 2
            /// </remarks>
            Down  = 1 << 1, // 2
            /// <summary>
            /// 左侧
            /// </summary>
            /// <remarks>
            /// = 4
            /// </remarks>
            Left  = 1 << 2, // 4
            /// <summary>
            /// 右侧
            /// </summary>
            /// <remarks>
            /// = 8
            /// </remarks>
            Right = 1 << 3, // 8
        }
    }
}
