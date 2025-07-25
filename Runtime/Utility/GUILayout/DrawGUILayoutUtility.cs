using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 实用程序：自动布局 IMGUI 绘制
    /// </summary>
    /// <remarks>
    /// 内部一切涉及角度的计算均采用角度制，即输入角度与输出角度的取值范围均为[0°, 360°)，以正右为【0°】，逆时针增长
    /// </remarks>
    public static class DrawGUILayoutUtility
    {
        #region 常量
        /// <summary>
        /// 字段高度
        /// </summary>
        private const int FIELD_HEIGHT = 20;

        /// <summary>
        /// 边框宽度
        /// </summary>
        private const int BORDER_WIDTH = 4;
        #endregion

        #region 字段
        /// <summary>
        /// 字段宽度
        /// </summary>
        private static int s_FieldWidth = 0;
        #endregion

        #region 属性

        #endregion

        #region 公开方法
        /// <summary>
        /// 获取【控件位置】
        /// </summary>
        /// <param name="height">控件高度</param>
        /// <returns></returns>
        public static Rect GetControlRect(float height)
        {
            return GUILayoutUtility.GetRect(s_FieldWidth, int.MaxValue, height, height);
        }

        /// <summary>
        /// 空格
        /// </summary>
        public static void Space()
        {
            GetControlRect(FIELD_HEIGHT);
        }

        /// <summary>
        /// 空格
        /// </summary>
        /// <param name="height">空格高度</param>
        public static void Space(float height)
        {
            GetControlRect(height > 0 ? height : 0);
        }

        /// <summary>
        /// 绘制【标签】
        /// </summary>
        /// <param name="label">标签</param>
        public static void DrawLabel(GUIContent label)
        {
            // 绘制【标签】
            DrawGUIUtility.DrawLabel
            (
                HandleBroder(GetControlRect(FIELD_HEIGHT)),
                label
            );
        }

        /// <summary>
        /// 绘制【切换】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="value">布尔值</param>
        /// <returns>返回用户输入的布尔内容。</returns>
        public static bool DrawToggle(GUIContent label, bool value)
        {
            // 绘制【切换】
            return DrawGUIUtility.DrawToggle
            (
                DrawPrefixLabel(FIELD_HEIGHT, label),
                value
            );
        }

        /// <summary>
        /// 绘制【文本字段】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="text">文本</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的文本内容。</returns>
        public static string DrawTextField(GUIContent label, string text, bool isRetrunImmediately = false)
        {
            return DrawGUIUtility.DrawTextField
            (
                DrawPrefixLabel(FIELD_HEIGHT, label),
                text,
                isRetrunImmediately
            );
        }

        /// <summary>
        /// 绘制【浮点数字段】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="value">值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的浮点数内容。</returns>
        public static float DrawFloatField(GUIContent label, float value, bool isRetrunImmediately = false)
        {
            return DrawGUIUtility.DrawFloatField
            (
                DrawPrefixLabel(FIELD_HEIGHT, label),
                value,
                isRetrunImmediately
            );
        }

        /// <summary>
        /// 绘制【整型字段】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="value">值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的整型内容。</returns>
        public static int DrawIntField(GUIContent label, int value, bool isRetrunImmediately = false)
        {
            return DrawGUIUtility.DrawIntField
            (
                DrawPrefixLabel(FIELD_HEIGHT, label),
                value,
                isRetrunImmediately
            );
        }

        /// <summary>
        /// 绘制【二维向量字段】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="value">值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的浮点数内容。</returns>
        public static Vector2 DrawVector2Field(GUIContent label, Vector2 value, bool isRetrunImmediately = false)
        {
            // 获取【字段位置】
            Rect fieldPosition = DrawPrefixLabel(FIELD_HEIGHT, label);

            // 获取【向量分量宽度】
            float componentWidth = fieldPosition.width / 2;

            // 获取【向量分量标签尺寸】
            Vector2 componentLabelSize = new Vector2(12, fieldPosition.height);

            // 更新【向量分量宽度】
            componentWidth -= componentLabelSize.x;

            // 更新【字段位置宽度】
            fieldPosition.width = componentWidth;

            // 绘制【标签】
            DrawGUIUtility.DrawLabel(new Rect(fieldPosition.position, componentLabelSize), new GUIContent("X"));

            fieldPosition.x += componentLabelSize.x;

            value.x = DrawGUIUtility.DrawFloatField
            (
                fieldPosition,
                value.x,
                isRetrunImmediately
            );

            fieldPosition.x += componentWidth;

            // 绘制【标签】
            DrawGUIUtility.DrawLabel(new Rect(fieldPosition.position, componentLabelSize), new GUIContent("Y"));

            fieldPosition.x += componentLabelSize.x;

            value.y = DrawGUIUtility.DrawFloatField
            (
                fieldPosition,
                value.y,
                isRetrunImmediately
            );

            return value;
        }

        /// <summary>
        /// 绘制【滑动条】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="value">滑动条值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的滑动条值。</returns>
        public static float DrawSlider(GUIContent label, float value, float min, float max, bool isRoundToInt = false, bool isRetrunImmediately = false)
        {
            return DrawGUIUtility.DrawSlider
            (
                DrawPrefixLabel(FIELD_HEIGHT, label),
                8.0f,
                value,
                min,
                max,
                isRoundToInt,
                isRetrunImmediately
            );
        }

        /// <summary>
        /// 绘制【整型滑动条】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="value">滑动条值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的滑动条值。</returns>
        public static int DrawIntSlider(GUIContent label, int value, int min, int max, bool isRetrunImmediately = false)
        {
            return DrawGUIUtility.DrawIntSlider
            (
                DrawPrefixLabel(FIELD_HEIGHT, label),
                8.0f,
                value,
                min,
                max,
                isRetrunImmediately
            );
        }

        /// <summary>
        /// 绘制【旋钮】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="angle">角度（角度制），取值范围为[0, 360)</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的旋转角度，取值范围为[0, 360)。</returns>
        public static float DrawKnob(GUIContent label, float angle, bool isRoundToInt = false, bool isRetrunImmediately = false)
        {
            // 获取【控件高度】
            int controlHeight = 84;

            return DrawGUIUtility.DrawKnob
            (
                DrawPrefixLabel(controlHeight, label),
                8.0f,
                angle,
                isRoundToInt,
                isRetrunImmediately
            );
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 处理【边框】
        /// </summary>
        /// <param name="position">需要进行边框处理的位置</param>
        /// <returns>返回处理后的位置。</returns>
        private static Rect HandleBroder(Rect position)
        {
            return new Rect
            (
                position.x + BORDER_WIDTH,
                position.y,
                position.width - BORDER_WIDTH * 2,
                position.height
            );
        }

        /// <summary>
        /// 获取【控件】位置
        /// </summary>
        /// <param name="totalPosition">用于绘制前缀标签和后续控件的绘制位置</param>
        /// <param name="prefixLabelPosition">前缀标签的绘制位置</param>
        /// <returns>返回控件的绘制位置。</returns>
        private static Rect GetControlPosition(Rect totalPosition, Rect prefixLabelPosition)
        {
            // 获取【控件】的【X 轴坐标】
            float controlX = Mathf.Min
            (
                prefixLabelPosition.x + prefixLabelPosition.width,
                totalPosition.x + totalPosition.width
            );

            // 获取【控件】的【宽度】
            float controlWidth = Mathf.Max
            (
                totalPosition.x + totalPosition.width - (prefixLabelPosition.x + prefixLabelPosition.width),
                0.0f
            );

            // 获取【控件】的【高度】
            float controlHeight = Mathf.Max
            (
                controlWidth <= 0.0f ? 0.0f : totalPosition.height,
                0.0f
            );

            return new Rect
            (
                controlX,
                totalPosition.y,
                controlWidth,
                controlHeight
            );
        }

        /// <summary>
        /// 绘制【前缀标签】
        /// </summary>
        /// <param name="height">控件高度</param>
        /// <returns>返回绘制前缀标签之后仍可用于后续控件的绘制位置。</returns>
        private static Rect DrawPrefixLabel(float height, GUIContent label)
        {
            // 获取【总位置】
            Rect totalPosition = GetControlRect(height);

            // 获取【前缀标签位置】
            Rect prefixLabelPosition = DrawGUIUtility.GetPrefixLabelPosition(totalPosition);

            // 绘制【前缀标签】
            DrawGUIUtility.DrawLabel(HandleBroder(prefixLabelPosition), label);

            return HandleBroder(GetControlPosition(totalPosition, prefixLabelPosition));
        }
        #endregion
    }
}
