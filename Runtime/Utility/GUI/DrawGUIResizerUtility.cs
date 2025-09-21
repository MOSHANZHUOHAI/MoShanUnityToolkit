using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 实用程序：IMGUI 尺寸调整边框绘制
    /// </summary>
    public static partial class DrawGUIResizeBorderUtility
    {
        #region 常量
        /// <summary>
        /// 尺寸调整手柄尺寸下限
        /// </summary>
        private const int MIN_RESIZE_HANDLE_SIZE = 1;

        /// <summary>
        /// 双倍尺寸调整手柄尺寸上限
        /// </summary>
        private const int DOUBLE_MIN_RESIZE_HANDLE_SIZE = 2 * MIN_RESIZE_HANDLE_SIZE;
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【尺寸调整边界】
        /// </summary>
        /// <param name="windowPosition">窗口位置</param>
        /// <param name="resizeHandleSize">
        /// <para>尺寸调整手柄尺寸</para>
        /// <para>
        /// <br>X 轴</br>
        /// <br>绘制位置内的【左右两侧边缘】的【尺寸调整手柄】的【宽度】</br>
        /// <br>取值范围为[1, 绘制位置宽度 / 2]</br>
        /// </para>
        /// <para>
        /// <br>Y 轴</br>
        /// <br>绘制位置内的【上下两侧边缘】的【尺寸调整手柄】的【高度】</br>
        /// <br>取值范围为[1, 绘制位置高度 / 2]</br>
        /// </para>
        /// </param>
        /// <param name="minSize">尺寸下限，若某分量小于等于【0】，则视为该分量对应的轴向上不作尺寸限制</param>
        /// <param name="maxSize">尺寸上限，若某分量小于等于【0】或尺寸下限中的对应分量，则视为该分量对应的轴向上不作尺寸限制</param>
        /// <param name="border">边界，绘制区域不得超出该范围，若【边界】的【尺寸】的任一分量为【0】则视为无效</param>
        /// <returns>返回经过用户调整尺寸的输入位置。</returns>
        public static Rect DrawResizeBorder(Rect windowPosition, Vector2 resizeHandleSize, Vector2 minSize, Vector2 maxSize, Rect border)
        {
            // 获取【正向位置】
            windowPosition = windowPosition.ToPositive();

            // 判断 <【输入位置】的【尺寸】是否小于【尺寸下限】>，即<是否无法绘制尺寸调整区域>
            if (windowPosition.width < DOUBLE_MIN_RESIZE_HANDLE_SIZE || windowPosition.height < DOUBLE_MIN_RESIZE_HANDLE_SIZE)
            {
                return windowPosition;
            }

            #region 获取【有效参数】
            border = border.ToPositive();

            // 限制【尺寸下限】不得小于【0】
            minSize = new Vector2
            (
                minSize.x > 0.0f ? minSize.x : 0.0f,
                minSize.y > 0.0f ? minSize.y : 0.0f
            );

            // 限制【尺寸上限】不得小于【尺寸下限】
            maxSize = new Vector2
            (
                maxSize.x > minSize.x ? maxSize.x : float.MaxValue,
                maxSize.y > minSize.y ? maxSize.y : float.MaxValue
            );

            // 限制【尺寸调整手柄】的【尺寸】不得超过【绘制位置】的【尺寸】的一半
            resizeHandleSize = new Vector2
            (
                Math.Clamp(resizeHandleSize.x, MIN_RESIZE_HANDLE_SIZE, windowPosition.width  * 0.5f),
                Math.Clamp(resizeHandleSize.y, MIN_RESIZE_HANDLE_SIZE, windowPosition.height * 0.5f)
            );
            #endregion

            // 判断 <【输入边界】的【尺寸】是否不为【0】>，即<【边界】是否有效>
            bool isValidBorder = border.width != 0.0f && border.height != 0.0f;

            // 判断 <【输入边界】是否有效>
            if (isValidBorder)
            {
                windowPosition = windowPosition.Limit(border);
            }
            else
            {
                border = new Rect
                (
                    float.PositiveInfinity,
                    float.PositiveInfinity,
                    float.NegativeInfinity,
                    float.NegativeInfinity
                );
            }

            #region 绘制【尺寸调整手柄】

            #region 左
            // 绘制【左侧尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMin,
                    windowPosition.yMin + resizeHandleSize.y,
                    resizeHandleSize.x,
                    windowPosition.height - 2 * resizeHandleSize.y
                ),
                ResizeDirections.Left
            );
            #endregion

            #region 右
            // 绘制【右侧尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMax - resizeHandleSize.x,
                    windowPosition.yMin + resizeHandleSize.y,
                    resizeHandleSize.x,
                    windowPosition.height - 2 * resizeHandleSize.y
                ),
                ResizeDirections.Right
            );
            #endregion

            #region 上
            // 绘制【上方尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMin + resizeHandleSize.x,
                    windowPosition.yMin,
                    windowPosition.width - 2 * resizeHandleSize.x,
                    resizeHandleSize.y
                ),
                ResizeDirections.Up
            );
            #endregion

            #region 下
            // 绘制【下方尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMin + resizeHandleSize.x,
                    windowPosition.yMax - resizeHandleSize.y,
                    windowPosition.width - 2 * resizeHandleSize.x,
                    resizeHandleSize.y
                ),
                ResizeDirections.Down
            );
            #endregion

            #region 左上
            // 绘制【左上尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMin,
                    windowPosition.yMin,
                    resizeHandleSize.x,
                    resizeHandleSize.y
                ),
                ResizeDirections.Left | ResizeDirections.Up
            );
            #endregion

            #region 右上
            // 绘制【右上尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMax - resizeHandleSize.x,
                    windowPosition.yMin,
                    resizeHandleSize.x,
                    resizeHandleSize.y
                ),
                ResizeDirections.Right | ResizeDirections.Up
            );
            #endregion

            #region 右下
            // 绘制【右下尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMax - resizeHandleSize.x,
                    windowPosition.yMax - resizeHandleSize.y,
                    resizeHandleSize.x,
                    resizeHandleSize.y
                ),
                ResizeDirections.Right | ResizeDirections.Down
            );
            #endregion

            #region 左下
            // 绘制【左下尺寸调整手柄】
            HelpDrawResizeHandle
            (
                new Rect
                (
                    windowPosition.xMin,
                    windowPosition.yMax - resizeHandleSize.y,
                    resizeHandleSize.x,
                    resizeHandleSize.y
                ),
                ResizeDirections.Left | ResizeDirections.Down
            );
            #endregion

            #endregion

            // 判断 <【输入边界】是否有效>
            if (isValidBorder)
            {
                windowPosition = windowPosition.Cut(border);
            }

            return windowPosition;

            #region 局部方法
            /// 局部方法：辅助绘制尺寸调整手柄
            /// @position ：尺寸调整手柄位置
            /// @direction：尺寸调整方向
            void HelpDrawResizeHandle(Rect position, ResizeDirections direction)
            {
                DrawResizeHandle(position, direction, minSize, maxSize, border, ref windowPosition);
            }
            #endregion
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 绘制【尺寸调整手柄】
        /// </summary>
        /// <param name="resuzeHandlePosition">尺寸调整手柄位置</param>
        /// <param name="direction">尺寸调整方向</param>
        /// <param name="minSize">尺寸下限，取值范围为[0, +∞]</param>
        /// <param name="maxSize">尺寸上限，取值范围为[尺寸下限, +∞]</param>
        /// <param name="border">边界</param>
        /// <param name="windowPosition">窗口位置</param>
        /// <returns>返回是否调整输入位置的判断结果。</returns>
        private static bool DrawResizeHandle(Rect resuzeHandlePosition, ResizeDirections direction, Vector2 minSize, Vector2 maxSize, Rect border, ref Rect windowPosition)
        {
            // 绘制【拖拽区域】，并判断 <是否未进行拖拽>、<【拖拽距离】是否为【0】>
            if (!DrawGUIUtility.DrawDragArea(resuzeHandlePosition, out Vector2 distance) || distance == Vector2.zero)
            {
                return false;
            }

            // 判断 <【尺寸调整方向】是否包含【左侧】>，即<是否调整左侧边缘>
            if (direction.IsHasDirection(ResizeDirections.Left))
            {
                float last = windowPosition.xMin;

                windowPosition.xMin = Mathf.Clamp
                (
                    windowPosition.xMin + distance.x,
                    Math.Max(border.xMin        , Math.Max(windowPosition.xMax - maxSize.x, float.MinValue)),
                    Math.Min(windowPosition.xMax, Math.Max(windowPosition.xMax - minSize.x, float.MinValue))
                );

                Debug.Log($"{last}|{windowPosition.xMin}");
            }
            // 判断 <【尺寸调整方向】是否包含【右侧】>，即<是否调整右侧边缘>
            else if (direction.IsHasDirection(ResizeDirections.Right))
            {
                windowPosition.xMax = Mathf.Clamp
                (
                    windowPosition.xMax + distance.x,
                    Math.Max(windowPosition.xMin, Math.Min(windowPosition.xMin + minSize.x, float.MaxValue)),
                    Math.Min(border.xMax        , Math.Min(windowPosition.xMin + maxSize.x, float.MaxValue))
                );
            }

            // 判断 <【尺寸调整方向】是否包含【上方】>，即<是否调整上方边缘>
            if (direction.IsHasDirection(ResizeDirections.Up))
            {
                windowPosition.yMin = Mathf.Clamp
                (
                    windowPosition.yMin + distance.y,
                    Math.Max(border.yMin        , Math.Max(windowPosition.yMax - maxSize.y, float.MinValue)),
                    Math.Min(windowPosition.yMax, Math.Max(windowPosition.yMax - minSize.y, float.MinValue))
                );
            }
            // 判断 <【尺寸调整方向】是否包含【下方】>，即<是否调整下方边缘>
            else if (direction.IsHasDirection(ResizeDirections.Down))
            {
                windowPosition.yMax = Mathf.Clamp
                (
                    windowPosition.yMax + distance.y,
                    Math.Max(windowPosition.yMin, Math.Min(windowPosition.yMin + minSize.y, float.MaxValue)),
                    Math.Min(border.yMax        , Math.Min(windowPosition.yMin + maxSize.y, float.MaxValue))
                );
            }

            return true;
        }

        /// <summary>
        /// 是否包含【方向】
        /// </summary>
        /// <param name="directions">所有方向</param>
        /// <param name="direction">方向</param>
        /// <returns>返回【输入所有方向】是否包含【输入方向】的判断结果。</returns>
        private static bool IsHasDirection(this ResizeDirections directions, ResizeDirections direction)
        {
            // 等价于【directions.HasFlag(direction)】;
            return (directions & direction) != 0;
        }
        #endregion
    }
}
