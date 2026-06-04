using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 实用程序：矩形
    /// </summary>
    public static class RectUtility
    {
        #region 公开方法
        /// <summary>
        /// 转换为正向位置
        /// </summary>
        /// <param name="rect">指定需要转换 <see cref="Rect.size">尺寸</see> 为正数的矩形。</param>
        /// <returns>返回值为 <see cref="Rect.size">尺寸</see> 已转换为正数的 <paramref name="rect"/>。</returns>
        public static Rect ToPositive(this Rect rect)
        {
            return new Rect
            (
                rect.xMin,
                rect.yMin,
                Math.Abs(rect.width),
                Math.Abs(rect.height)
            );
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="rect">指定需要判断是否包含 <paramref name="target"/> 的矩形。</param>
        /// <param name="target">指定需要判断是否包含于 <paramref name="rect"/> 的矩形。</param>
        /// <returns>返回值为 <paramref name="rect"/> 是否包含 <paramref name="target"/> 的判断结果。</returns>
        public static bool Contains(this Rect rect, Rect target)
        {
            return rect.Contains(target.min) && rect.Contains(target.max);
        }

        /// <summary>
        /// 剪切
        /// </summary>
        /// <param name="rect">位置</param>
        /// <param name="border">边界</param>
        /// <returns>返回经过剪切到【输入边界】内的【输入位置】。</returns>
        public static Rect Cut(this Rect rect, Rect border)
        {
            rect   = rect.ToPositive();
            border = border.ToPositive();

            // 判断 <输入边界是否包含输入矩形的最小点>
            if (border.Contains(rect.min))
            {
                // 判断 <输入边界是否包含输入矩形的最大点>
                if (border.Contains(rect.max))
                {
                    return rect;
                }
                else
                {
                    return new Rect
                    (
                        rect.xMin,
                        rect.yMin,
                        Math.Min(rect.width , border.xMax - rect.xMin),
                        Math.Min(rect.height, border.yMax - rect.yMin)
                    );
                }
            }
            else
            {
                // 判断 <输入边界是否包含输入位置的最大点>
                if (border.Contains(rect.max))
                {
                    return new Rect
                    (
                        border.xMin,
                        border.yMin,
                        Math.Min(rect.width , rect.xMax - border.xMin),
                        Math.Min(rect.height, rect.yMax - border.yMin)
                    );
                }
                else
                {
                    rect.size = Vector2.zero;

                    return new Rect
                    (
                        Math.Clamp(rect.xMin, border.xMin, border.xMax),
                        Math.Clamp(rect.yMin, border.yMin, border.yMax),
                        0,
                        0
                    );
                }
            }
        }

        /// <summary>
        /// 限制
        /// </summary>
        /// <param name="rect">指定需要限制位置和大小的矩形。</param>
        /// <param name="border">指定用于限制 <paramref name="rect"/> 的边界。</param>
        /// <returns>返回值为经过限制位置和大小到 <paramref name="border"/> 中的 <paramref name="rect"/>。</returns>
        public static Rect Limit(this Rect rect, Rect border)
        {
            return rect.LimitPosition(border).LimitSize(border);
        }

        /// <summary>
        /// 限制位置
        /// </summary>
        /// <param name="rect">指定需要限制位置的矩形。</param>
        /// <param name="border">指定用于限制 <paramref name="rect"/> 的边界。</param>
        /// <returns>返回值为经过限制位置到 <paramref name="border"/> 中的 <paramref name="rect"/>。</returns>
        public static Rect LimitPosition(this Rect rect, Rect border)
        {
            rect = rect.ToPositive();
            border   = border  .ToPositive();

            // 判断 <【输入边界】是否包含【输入位置】>
            if (border.Contains(rect))
            {
                return rect;
            }

            rect.position = new Vector2
            (
                Math.Clamp
                (
                    rect.x,
                    border.xMin,
                    Math.Max(border.xMin, border.xMax - rect.width)
                ),
                Math.Clamp
                (
                    rect.y,
                    border.yMin,
                    Math.Max(border.yMin, border.yMax - rect.height)
                )
            );

            return rect;
        }

        /// <summary>
        /// 限制尺寸
        /// </summary>
        /// <remarks>
        /// 建议在调用该方法之前，先调用 <see cref="LimitPosition">限制位置</see> 方法，或直接调用 <see cref="Limit">限制</see> 方法。
        /// </remarks>
        /// <param name="rect">指定需要限制大小的矩形。</param>
        /// <param name="border">指定用于限制 <paramref name="rect"/> 的边界。</param>
        /// <returns>返回值为经过限制大小到 <paramref name="border"/> 中的 <paramref name="rect"/>。</returns>
        public static Rect LimitSize(this Rect rect, Rect border)
        {
            rect = rect.ToPositive();
            border   = border  .ToPositive();

            // 判断 <【输入边界】是否包含【输入位置】>
            if (border.Contains(rect))
            {
                return rect;
            }

            rect.size = new Vector2
            (
                Math.Clamp
                (
                    rect.width,
                    0,
                    Math.Clamp(border.width, 0, border.xMax - rect.xMin)
                ),
                Math.Clamp
                (
                    rect.height,
                    0,
                    Math.Clamp(border.height, 0, border.yMax - rect.yMin)
                )
            );

            return rect;
        }
        #endregion
    }
}
