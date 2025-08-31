using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 实用程序：IMGUI 圆形滑动条绘制
    /// </summary>
    /// <remarks>
    /// 内部一切涉及角度的计算均采用角度制，即输入角度与输出角度的取值范围均为[0°, 360°)，以正右为【0°】，逆时针增长
    /// </remarks>
    internal static class DrawGUICircleSliderUtility
    {
        #region 常量
        /// <summary>
        /// 控件哈希值
        /// </summary>
        private static readonly int CONTROL_HASH = nameof(DrawGUICircleSliderUtility).GetHashCode();
        #endregion

        #region 字段
        /// <summary>
        /// 热控件值
        /// </summary>
        private static float s_HotControlValue = 0.0f;
        #endregion

        #region 属性
        /// <summary>
        /// 圆形纹理
        /// </summary>
        private static Texture2D CircleTexture
        {
            get
            {
                return DrawGUIUtility.RectTexture;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static DrawGUICircleSliderUtility() { }
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【角度滑动条】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="angle">角度（角度制），取值范围为[0°, 360°)</param>
        /// <param name="isCounterclockwise">是否逆时针</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的旋转角度，取值范围为[0, 360)。</returns>
        public static float DrawAngleSlider(Rect position, float knobRadius, float angle, bool isCounterclockwise, bool isRoundToInt, bool isRetrunImmediately)
        {
            return DrawCircleSlider(position, knobRadius, angle, 0, 360, isCounterclockwise, isRoundToInt, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【圆形滑动条】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isCounterclockwise">是否逆时针</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的旋转角度，取值范围为[0, 360)。</returns>
        public static float DrawCircleSlider(Rect position, float knobRadius, float value, float min, float max, bool isCounterclockwise, bool isRoundToInt, bool isRetrunImmediately)
        {
            // 判断 <【输入角度】是否为【NaN】>，即<【输入角度】是否无效>
            if (float.IsNaN(min) || float.IsNaN(max))
            {
                try
                {
                    // 抛出异常：参数异常
                    throw new ArgumentException("角度参数为 NaN ！");
                }
                // 捕获异常：参数异常
                catch (ArgumentException otherException)
                {
                    Debug.LogException(otherException);
                }

                value = 0.0f;
            }

            // 重定向【输入值】
            value = Repeat(value, min, max);

            // 获取【绘制区域中心】
            Vector2 center = position.center;

            // 获取【绘制区域尺寸】
            Vector2 size = position.size;

            // 判断 <旋钮半径是否小于等于【0】>，即<是否需要重置【旋钮半径】为预设值>
            if (knobRadius <= 0.0f)
            {
                knobRadius = 10.0f;
            }

            // 限定【旋钮半径】
            knobRadius = Math.Clamp
            (
                knobRadius,
                0.0f,
                Math.Max(Math.Min(size.x, size.y), 0.0f) * 0.5f
            );

            // 设置【背景圆形半径】为【绘制区域尺寸】的【宽】与【高】中的最小值的一半减去【旋钮半径】
            float backgroundRadius = Math.Max(Math.Min(size.x, size.y), 0.0f) * 0.5f - knobRadius;

            // 限定【背景圆形半径】
            backgroundRadius = Math.Max(backgroundRadius, 0.0f);

            #region 获取【背景位置】
            // 获取【背景半尺寸】
            Vector2 halfBackgroundSize = backgroundRadius * Vector2.one;

            // 获取【背景位置】
            Rect backgroundPosition = new Rect
            (
                center - halfBackgroundSize,
                2 * halfBackgroundSize
            );
            #endregion

            // 获取【控件标识】
            int controlId = GUIUtility.GetControlID(CONTROL_HASH, FocusType.Passive, position);

            #region 获取【事件信息】
            // 获取【当前事件】
            Event currentEvent = Event.current;

            // 获取【控件标识】对应的【当前事件类型】
            EventType eventType = currentEvent.GetTypeForControl(controlId);

            // 判断 <【GUI 实用程序】的【当前热控件标识】是否等于【控件标识】>，即<【当前控件】是否拥有焦点>
            bool isHasFocus = GUIUtility.hotControl == controlId;
            #endregion

            #region 获取【旋钮位置】
            // 获取【当前值】
            float currentValue;

            // 判断 <【当前控件】是否拥有焦点>、<【当前事件类型】是否为【拖拽鼠标】>
            if (isHasFocus)
            {
                // 设置【当前值】为【热控件值】
                currentValue = s_HotControlValue;
            }
            else
            {
                // 设置【当前值】为【输入角度】
                currentValue = value;
            }

            // 获取【比例】
            float ratio = min == max ? 0.0f : Mathf.Clamp01((currentValue - min) / (max - min));

            // 转换【当前值】为【弧度制角度】
            float radians = ratio * 2 * Mathf.PI * (isCounterclockwise ? -1 : 1);

            // 获取【旋钮中心】
            Vector2 knobCenter = new Vector2
            (
                center.x + Mathf.Cos(radians) * backgroundRadius,
                center.y - Mathf.Sin(radians) * backgroundRadius
            );

            // 获取【旋钮位置】
            Rect knobPosition = new Rect(knobCenter - knobRadius * Vector2.one, 2 * knobRadius * Vector2.one);
            #endregion

            #region 绘制【控件】
            // 判断 <【当前控件】是否拥有焦点>
            if (isHasFocus)
            {
                // 绘制【背景】描边
                DrawCircle
                (
                    new Rect(backgroundPosition.position - Vector2.one, backgroundPosition.size + 2 * Vector2.one),
                    Color.white,
                    backgroundRadius + 1
                );

                // 绘制【旋钮】描边
                DrawCircle
                (
                    new Rect(knobPosition.position - Vector2.one, knobPosition.size + 2 * Vector2.one),
                    Color.white,
                    knobRadius + 1
                );
            }

            // 绘制【背景】
            DrawCircle
            (
                backgroundPosition,
                Color.black,
                backgroundRadius
            );

            // 绘制【旋钮】
            DrawCircle
            (
                knobPosition,
                Color.gray,
                knobRadius
            );
            #endregion

            #region 获取【返回值】
            // 获取【返回值】
            float result = value;

            switch (eventType)
            {
                // 按下鼠标
                case EventType.MouseDown:
                    // 获取【当前事件】的【鼠标位置】
                    Vector2 mousePosition = currentEvent.mousePosition;

                    // 判断 <【旋钮绘制位置】是否包含【鼠标位置】>
                    if (knobPosition.Contains(mousePosition))
                    {
                        // 获取【鼠标相对旋钮中心的距离】
                        float distanceToKnob = Vector2.Distance(currentEvent.mousePosition, knobCenter);

                        // 判断 <【鼠标到旋钮中心的距离】是否大于【旋钮半径】>，即<鼠标是否不在旋钮范围内>
                        if (distanceToKnob > knobRadius)
                        {
                            break;
                        }

                        // 设置【GUI 实用程序】的【当前热控件标识】为【当前控件标识】
                        GUIUtility.hotControl = controlId;
                        
                        // 初始化【热控件值】
                        s_HotControlValue = value % 360.0f;

                        result = value;
                    }
                    break;

                // 抬起鼠标
                case EventType.MouseUp:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        // 重置【GUI 实用程序】中的【当前热控件标识】
                        GUIUtility.hotControl = 0;

                        UpdateResult();
                    }
                    break;

                // 拖拽鼠标
                case EventType.MouseDrag:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        // 获取【鼠标相对中心的方向】
                        Vector2 direction = currentEvent.mousePosition - center;

                        // 获取【鼠标相对旋钮中心的角度】，即【旋钮角度】
                        float knobAngle;

                        // 判断 <是否逆时针>
                        if (isCounterclockwise)
                        {
                            knobAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        }
                        else
                        {
                            knobAngle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;
                        }

                        // 判断 <【旋钮角度】是否小于【0】>
                        if (knobAngle < 0)
                        {
                            // 标准化【旋钮角度】的取值范围到[0, 360)
                            knobAngle += 360;
                        }

                        // 更新【比例】
                        ratio = Mathf.Clamp01(knobAngle / 360.0f);

                        // 更新【热控件值】
                        s_HotControlValue = ratio * (max - min);

                        // 更新【当前值】为【热控件值】
                        currentValue = s_HotControlValue;

                        // 使用事件
                        currentEvent.Use();

                        // 判断 <是否立即返回结果>
                        if (isRetrunImmediately)
                        {
                            UpdateResult();
                        }
                    }
                    break;

                // 默认
                default:
                    break;
            }
            #endregion

            // 判断 <是否对返回结果进行四舍五入取整>
            if (isRoundToInt)
            {
                result = (int)Math.Round(result);

                currentValue = (int)Math.Round(currentValue);
            }

            #region 绘制【标签】
            // 判断 <是否存在足够位置绘制标签>
            if (backgroundRadius >= 32)
            {
                /*
                 * 经过测试，在使用默认旋钮半径（10 px）与标签样式的默认文本尺寸时，考虑到可显示的最大值为【 359.99°】，在保证可视化效果的情况下，【32 px】为可绘制标签的最小背景半径值。
                 * 即默认情况下，若需要保证可视化效果，绘制区域的最小宽高为【84 px】。
                 */

                // 获取【标签文本】，在文本前添加空格以对冲文本末尾符号【%】的宽度导致的文本偏移
                string labelText = string.Format(" {0}%", (ratio * 100).ToString("F2"));

                // 绘制【标签】以显示【当前角度】
                GUI.Label(position, new GUIContent(labelText), DrawGUIUtility.MiddleCenterLabelStyle);
            }
            #endregion

            return result;

            #region 局部方法
            // 静态局部方法：绘制圆形
            // @position：位置
            // @color：颜色
            // @radius：半径
            static void DrawCircle(Rect position, Color color, float radius)
            {
                GUI.DrawTexture
                (
                    position,
                    CircleTexture,
                    ScaleMode.StretchToFill,
                    true,
                    0,
                    color,
                    0,
                    radius
                );
            }

            // 局部方法：更新返回值
            void UpdateResult()
            {
                // 设置【返回值】为【热控件值】
                result = s_HotControlValue;

                // 判断 <【输入值】是否不等于【返回值】>
                if (value != result)
                {
                    // 通知 GUI 已发生变更
                    GUI.changed = true;
                }
            }
            #endregion
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 重复
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回通过重定向限定取值范围到【最小值】和【最大值】之间的【输入值】。</returns>
        private static float Repeat(float value, float min, float max)
        {
            // 判断 <【最小值】是否等于【最大值】>
            if (min == max)
            {
                return min;
            }

            // 判断 <【最小值】是否大于【最大值】>
            if (min > max)
            {
                float temp = min;
                min        = max;
                max        = temp;
            }

            // 获取【长度】
            float length = Mathf.Abs(max - min);

            // 获取【偏移】
            float offset = min - length * Mathf.FloorToInt(min / length);

            return Mathf.Clamp((value - offset) % length + offset + min, min, max);
        }
        #endregion
    }
}
