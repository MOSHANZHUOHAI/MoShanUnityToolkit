using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 实用程序：绘制 GL
    /// </summary>
    public static partial class DrawGLUtility
    {
        /// <summary>
        /// 形状绘制模式
        /// </summary>
        [Serializable]
        public enum DrawShapeMode : int
        {
            /// <summary>
            /// 仅绘制线
            /// </summary>
            Line = 0,
            /// <summary>
            /// 仅绘制填充
            /// </summary>
            Fall = 1,
            /// <summary>
            /// 绘制线和填充
            /// </summary>
            Both = 2,
        }
    }
}
