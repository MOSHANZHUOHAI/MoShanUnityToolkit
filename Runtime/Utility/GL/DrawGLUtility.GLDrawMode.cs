using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 实用程序：绘制 GL
    /// </summary>
    public static partial class DrawGLUtility
    {
        /// <summary>
        /// GL 绘制模式
        /// </summary>
        [Serializable]
        public enum GLDrawMode : int
        {
            /// <summary>
            /// 无
            /// </summary>
            None          = 0,
            /// <summary>
            /// 线 = 1
            /// </summary>
            /// <remarks>
            /// <br>基于每对顶点绘制线条，【2】个顶点视为【1】对。</br>
            /// <br>如果经过【4】个顶点，A、B、C 和 D，则绘制【2】条线：</br>
            /// <br>一条在 A 和 B 之间；</br>
            /// <br>一条在 C 和 D 之间。</br>
            /// </remarks>
            Line          = GL.LINES,
            /// <summary>
            /// 线条带 = 2
            /// </summary>
            /// <remarks>
            /// <br>从开头到末尾，绘制经过每个顶点的线条。</br>
            /// <br>如果经过【3】个顶点，A、B 和 C，则绘制【2】条线：</br>
            /// <br>一条在 A 和 B 之间；</br>
            /// <br>一条在 B 和 C 之间。</br>
            /// </remarks>
            LineStrip     = GL.LINE_STRIP,
            /// <summary>
            /// 三角形 = 4
            /// </summary>
            /// <remarks>
            /// <br>基于每组顶点绘制三角形，【3】个顶点视为【1】组。</br>
            /// <br>如果经过【3】个顶点，则绘制【1】个三角形，其中每一个顶点成为该三角形的一个角。</br>
            /// <br>如果经过【6】个顶点，则绘制【2】个三角形。</br>
            /// </remarks>
            Triangle      = GL.TRIANGLES,
            /// <summary>
            /// 三角形条带 = 5
            /// </summary>
            /// <remarks>
            /// <br>从开头到末尾，在经过的每个顶点之间绘制三角形。</br>
            /// <br>如果经过【5】个顶点，A、B、C、D 和 E，则绘制【3】个三角形。在前【3】个顶点之间绘制第一个三角形。所有后续三角形均使用之前的【2】个顶点，再加下一个额外顶点。。</br>
            /// <br>在该示例中，绘制的三个三角形将为 A、B、C，然后是 B、C、D，最后是 C、D、E。</br>
            /// </remarks>
            TriangleStrip = GL.TRIANGLE_STRIP,
            /// <summary>
            /// 四边形 = 7
            /// </summary>
            /// <remarks>
            /// <br>基于每组顶点绘制三角形，【4】个顶点视为【1】组。</br>
            /// <br>如果经过【4】个顶点，则绘制【1】个四边形，其中每一个顶点成为该四边形的一个角。</br>
            /// <br>如果经过【8】个顶点，则绘制【2】个四边形。</br>
            /// </remarks>
            Quad          = GL.QUADS,
        }
    }
}
