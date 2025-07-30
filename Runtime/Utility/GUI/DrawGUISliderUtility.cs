using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 实用程序：IMGUI 滑动条绘制
    /// </summary>
    public static class DrawGUISliderUtility
    {
        #region 常量
        /// <summary>
        /// 控件哈希值
        /// </summary>
        private static readonly int CONTROL_HASH = nameof(DrawGUITextFieldUtility).GetHashCode();
        #endregion

        #region 字段
        /// <summary>
        /// 热滑动条控件值
        /// </summary>
        private static float s_HotControlValue = 0.0f;

        /// <summary>
        /// 圆形纹理
        /// </summary>
        private static Texture2D s_CircleTexture;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static DrawGUISliderUtility()
        {
            // 创建【圆形纹理】
            s_CircleTexture = new Texture2D(1, 1);

            // 设置【圆形纹理】的【颜色】为【白色】
            s_CircleTexture.SetPixel(0, 0, Color.white);

            // 应用设置
            s_CircleTexture.Apply();
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【整型滑动条】
        /// </summary>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="position">位置</param>
        /// <param name="value">滑动条值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的滑动条值。</returns>
        public static int DrawIntSlider(Rect position, float knobRadius, int value, int min, int max, bool isRetrunImmediately)
        {
            return (int)DrawSlider(position, knobRadius, value, min, max, true, true, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【滑动条】
        /// </summary>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="position">位置</param>
        /// <param name="value">滑动条值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isInt">是否为整型</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的滑动条值。</returns>
        public static float DrawSlider(Rect position, float knobRadius, float value, float min, float max, bool isInt, bool isRoundToInt, bool isRetrunImmediately)
        {
            // 判断 <是否为【整型】>
            if (isInt)
            {
                value = (int)value;
                min = (int)min;
                max = (int)max;
            }

            // 判断 <【最小值】是否大于【最大值】>
            if (min > max)
            {
                float temp = min;
                min = max;
                max = temp;
            }

            // 判断 <【输入角度】是否为【NaN】>，即<【输入角度】是否无效>
            if (float.IsNaN(value))
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

                value = min;
            }

            value = Mathf.Clamp(value, min, max);

            // 获取【绘制区域中心】
            Vector2 center = position.center;

            // 获取【绘制区域尺寸】
            Vector2 size = position.size;

            // 判断 <【旋钮半径】是否小于等于【0】>，即<是否需要重置【旋钮半径】为预设值>
            if (knobRadius <= 0.0f)
            {
                knobRadius = 10.0f;
            }

            // 限定【旋钮半径】
            knobRadius = Math.Clamp
            (
                knobRadius,
                0.0f,
                Math.Max(Math.Min(position.width, position.height), 0.0f)
            );

            // 获取【背景尺寸】
            float backgroundWidth = Math.Max(size.x - 2 * knobRadius, 0.0f);
            float backgroundHeight = 6.0f;

            #region 获取【背景位置】
            // 获取【背景位置】
            Rect backgroundPosition = new Rect
            (
                center.x - backgroundWidth * 0.5f,
                center.y - backgroundHeight * 0.5f,
                backgroundWidth,
                backgroundHeight
            );
            #endregion

            // 获取【控件编号】
            int controlId = GUIUtility.GetControlID(CONTROL_HASH, FocusType.Passive, position);

            #region 获取【事件信息】
            // 获取【当前事件】
            Event currentEvent = Event.current;

            // 获取【控件编号】对应的【当前事件类型】
            EventType eventType = currentEvent.GetTypeForControl(controlId);

            // 判断 <【GUI 实用程序】的【当前热控件 ID】是否等于【控件编号】>，即<【当前控件】是否拥有焦点>
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

            // 获取【当前值】在【输入范围】中对应的【比例】
            float ratio = Mathf.Clamp01((currentValue - min) / (max - min));

            // 获取【旋钮中心】
            Vector2 knobCenter = new Vector2
            (
                backgroundPosition.x + ratio * backgroundPosition.width,
                center.y
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
                    new Rect(backgroundPosition.position - Vector2.one,
                    backgroundPosition.size + 2 * Vector2.one),
                    s_CircleTexture,
                    Color.white,
                    5 + 1
                );

                // 绘制【旋钮】描边
                DrawCircle
                (
                    new Rect(knobPosition.position - Vector2.one, knobPosition.size + 2 * Vector2.one),
                    s_CircleTexture,
                    Color.white,
                    knobRadius + 1
                );
            }

            // 绘制【背景】
            DrawCircle
            (
                backgroundPosition,
                s_CircleTexture,
                Color.black,
                5
            );

            // 绘制【旋钮】
            DrawCircle
            (
                knobPosition,
                s_CircleTexture,
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

                        // 设置【GUI 实用程序】的【当前热控件 ID】为【当前控件 ID】
                        GUIUtility.hotControl = controlId;

                        // 初始化【热控件值】
                        s_HotControlValue = value;

                        result = value;
                    }
                    break;

                // 抬起鼠标
                case EventType.MouseUp:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        // 重置【GUI 实用程序】中的【当前热控件 ID】
                        GUIUtility.hotControl = 0;

                        UpdateResult();
                    }
                    break;

                // 拖拽鼠标
                case EventType.MouseDrag:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        // 获取【鼠标相对起点的距离】
                        float distance = currentEvent.mousePosition.x - backgroundPosition.x;

                        // 更新【热控件值】
                        s_HotControlValue = Mathf.Lerp(min, max, Mathf.Clamp01(distance / backgroundWidth));

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
            }

            return result;

            #region 局部方法
            // 静态局部方法：绘制圆形
            // @position：位置
            // @image：图像
            // @color：颜色
            // @radius：半径
            static void DrawCircle(Rect position, Texture2D image, Color color, float radius)
            {
                GUI.DrawTexture
                (
                    position,
                    image,
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
    }
}
