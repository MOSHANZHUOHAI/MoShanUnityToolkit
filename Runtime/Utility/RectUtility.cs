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
        /// 转换为【正向位置】
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns>返回【尺寸】转换为正数的【输入位置】。</returns>
        public static Rect ToPositive(this Rect position)
        {
            return new Rect
            (
                position.xMin,
                position.yMin,
                Math.Abs(position.width),
                Math.Abs(position.height)
            );
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="target">目标</param>
        /// <returns>返回【输入位置】是否包含【输入目标】的判断结果。</returns>
        public static bool Contains(this Rect position, Rect target)
        {
            return position.Contains(target.min) && position.Contains(target.max);
        }

        /// <summary>
        /// 剪切
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="border">边界</param>
        /// <returns>返回经过剪切到【输入边界】内的【输入位置】。</returns>
        public static Rect Cut(this Rect position, Rect border)
        {
            position = position.ToPositive();
            border   = border  .ToPositive();

            // 判断 <【输入边界】是否包含【输入位置】的【最小点】>
            if (border.Contains(position.min))
            {
                // 判断 <【输入边界】是否包含【输入位置】的【最大点】>
                if (border.Contains(position.max))
                {
                    return position;
                }
                else
                {
                    return new Rect
                    (
                        position.xMin,
                        position.yMin,
                        Math.Min(position.width , border.xMax - position.xMin),
                        Math.Min(position.height, border.yMax - position.yMin)
                    );
                }
            }
            else
            {
                // 判断 <【输入边界】是否包含【输入位置】的【最大点】>
                if (border.Contains(position.max))
                {
                    return new Rect
                    (
                        border.xMin,
                        border.yMin,
                        Math.Min(position.width , position.xMax - border.xMin),
                        Math.Min(position.height, position.yMax - border.yMin)
                    );
                }
                else
                {
                    position.size = Vector2.zero;

                    return new Rect
                    (
                        Math.Clamp(position.xMin, border.xMin, border.xMax),
                        Math.Clamp(position.yMin, border.yMin, border.yMax),
                        0,
                        0
                    );
                }
            }
        }

        /// <summary>
        /// 限制
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="border">边界</param>
        /// <returns>返回经过限制位置和尺寸到【输入边界】内的【输入位置】。</returns>
        public static Rect Limit(this Rect position, Rect border)
        {
            return position.LimitPosition(border).LimitSize(border);
        }

        /// <summary>
        /// 限制【位置】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="border">边界</param>
        /// <returns>返回经过限制位置到【输入边界】内的【输入位置】。</returns>
        public static Rect LimitPosition(this Rect position, Rect border)
        {
            position = position.ToPositive();
            border   = border  .ToPositive();

            // 判断 <【输入边界】是否包含【输入位置】>
            if (border.Contains(position))
            {
                return position;
            }

            position.position = new Vector2
            (
                Math.Clamp
                (
                    position.x,
                    border.xMin,
                    Math.Max(border.xMin, border.xMax - position.width)
                ),
                Math.Clamp
                (
                    position.y,
                    border.yMin,
                    Math.Max(border.yMin, border.yMax - position.height)
                )
            );

            return position;
        }

        /// <summary>
        /// 限制【尺寸】
        /// </summary>
        /// <remarks>
        /// 建议在调用该方法之前，先调用【<see cref="LimitPosition">限制【位置】</see>】方法，或直接调用【<see cref="Limit">限制</see>】方法。
        /// </remarks>
        /// <param name="position">位置</param>
        /// <param name="border">边界</param>
        /// <returns>返回经过限制尺寸到【输入边界】内的【输入位置】。</returns>
        public static Rect LimitSize(this Rect position, Rect border)
        {
            position = position.ToPositive();
            border   = border  .ToPositive();

            // 判断 <【输入边界】是否包含【输入位置】>
            if (border.Contains(position))
            {
                return position;
            }

            position.size = new Vector2
            (
                Math.Clamp
                (
                    position.width,
                    0,
                    Math.Clamp(border.width, 0, border.xMax - position.xMin)
                ),
                Math.Clamp
                (
                    position.height,
                    0,
                    Math.Clamp(border.height, 0, border.yMax - position.yMin)
                )
            );

            return position;
        }
        #endregion
    }
}
