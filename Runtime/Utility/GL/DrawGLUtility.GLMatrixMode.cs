using System;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 实用程序：绘制 GL
    /// </summary>
    public static partial class DrawGLUtility
    {
        /// <summary>
        /// GL 矩阵模式
        /// </summary>
        [Serializable]
        public enum GLMatrixMode : int
        {
            /// <summary>
            /// 默认
            /// </summary>
            /// <remarks>
            /// 不显示任何内容
            /// </remarks>
            Identity = 0,
            /// <summary>
            /// 正交
            /// </summary>
            /// <remarks>
            /// 屏幕左下角为原点(0, 0)，屏幕右上角为(1, 1)
            /// </remarks>
            Ortho,
            /// <summary>
            /// 屏幕像素
            /// </summary>
            /// <remarks>
            /// <br>单位：像素</br>
            /// <br>屏幕左上角为原点(0, 0)，屏幕右下角为(<see cref="global::UnityEngine.Screen.width">屏幕宽度</see>, <see cref="global::UnityEngine.Screen.height">屏幕高度</see>)</br>
            /// </remarks>
            Pixel,
            /// <summary>
            /// 世界空间
            /// </summary>
            /// <remarks>
            /// 单位：Unity 单位
            /// </remarks>
            World,
        }
    }
}
