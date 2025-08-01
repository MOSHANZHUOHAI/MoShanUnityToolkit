using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

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

        /// <summary>
        /// 缩进宽度
        /// </summary>
        private const int INDENT_WIDTH = 15;

        /// <summary>
        /// 前缀标签宽度下限
        /// </summary>
        private const int MIN_PREFIX_LABEL_WIDTH = 150;
        #endregion

        #region 字段
        /// <summary>
        /// 缩进级别
        /// </summary>
        private static int s_IndentLevel = 0;
        #endregion

        #region 属性
        /// <summary>
        /// 缩进级别
        /// </summary>
        public static int IndentLevel
        {
            get
            {
                return s_IndentLevel;
            }
            set
            {
                int newValue = value > 0 ? value : 0;

                // 判断 <【缩进级别】是否等于【输入值】>
                if (s_IndentLevel == value)
                {
                    return;
                }

                s_IndentLevel = value;
            }
        }

        /// <summary>
        /// 缩进空间
        /// </summary>
        public static int IndentSpace
        {
            get
            {
                return s_IndentLevel * INDENT_WIDTH;
            }
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 获取【控件位置】
        /// </summary>
        /// <param name="height">控件高度</param>
        /// <returns></returns>
        public static Rect GetControlRect(float height)
        {
            return GUILayoutUtility.GetRect(0.0f, int.MaxValue, height, height);
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
            float componentWidth = fieldPosition.width * 0.5f;

            // 获取【向量分量标签尺寸】
            Vector2 componentLabelSize = new Vector2(16.0f, fieldPosition.height);

            // 更新【向量分量宽度】
            componentWidth -= componentLabelSize.x;

            // 更新【字段位置宽度】
            fieldPosition.width = componentWidth;

            // 绘制【X 轴标签】
            DrawGUIUtility.DrawLabel(new Rect(fieldPosition.position, componentLabelSize), new GUIContent("X"));

            fieldPosition.x += componentLabelSize.x;

            value.x = DrawGUIUtility.DrawFloatField
            (
                fieldPosition,
                value.x,
                isRetrunImmediately
            );

            fieldPosition.x += componentWidth;

            // 绘制【Y 轴标签】
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
        /// 绘制【圆形滑动条】
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isCounterclockwise">是否逆时针</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的旋转角度，取值范围为[0, 360)。</returns>
        public static float DrawCircleSlider(GUIContent label, float value, float min, float max, bool isCounterclockwise = false, bool isRoundToInt = false, bool isRetrunImmediately = false)
        {
            // 获取【控件高度】
            int controlHeight = 84;

            return DrawGUIUtility.DrawCircleSlider
            (
                DrawPrefixLabel(controlHeight, label),
                8.0f,
                value,
                min,
                max,
                isCounterclockwise,
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
        /// 获取【前缀标签位置】
        /// </summary>
        /// <param name="totalPosition">用于绘制前缀标签和后续控件的绘制位置</param>
        /// <param name="label">标签</param>
        /// <returns>返回前缀标签的绘制位置。</returns>
        public static Rect GetPrefixLabelPosition(Rect totalPosition, GUIContent label)
        {
            // 判断 <【标签】是否为【空】>
            if (label == null || label == GUIContent.none)
            {
                totalPosition.size = Vector2.zero;

                return totalPosition;
            }

            // 判断 <【前缀标签宽度上限】是否大于【输入位置宽度】>，即<是否无法绘制【控件】>
            if (MIN_PREFIX_LABEL_WIDTH >= totalPosition.width)
            {
                totalPosition.x = Mathf.Min(totalPosition.x + IndentSpace, totalPosition.x + totalPosition.width);

                totalPosition.width = IndentSpace >= totalPosition.width ? 0.0f : totalPosition.width - IndentSpace;

                // 判断 <【前缀标签宽度】是否小于等于【0】>，即<是否无法绘制【前缀标签】>
                if (totalPosition.width <= 0.0f)
                {
                    totalPosition.height = 0.0f;
                }

                return totalPosition;
            }

            // 获取【前缀标签宽度】
            float prefixLabelWidth;

            // 判断 <【前缀标签宽度上限】是否大于【输入位置宽度的一半】>
            if (MIN_PREFIX_LABEL_WIDTH >= totalPosition.width * 0.5f)
            {
                prefixLabelWidth = MIN_PREFIX_LABEL_WIDTH;
            }
            else
            {
                prefixLabelWidth = totalPosition.width * 0.5f;
            }

            // 获取【标签 X 轴坐标】
            totalPosition.x = Mathf.Min(totalPosition.x + IndentSpace, prefixLabelWidth);

            // 获取【标签宽度】
            // 前缀标签宽度 = 总宽度 / 2 - 缩进空间
            totalPosition.width = Mathf.Max(prefixLabelWidth - IndentSpace, 0.0f);

            // 判断 <【前缀标签宽度】是否小于等于【0】>，即<是否无法绘制【前缀标签】>
            if (totalPosition.width <= 0.0f)
            {
                totalPosition.height = 0.0f;
            }

            return totalPosition;
        }

        /// <summary>
        /// 获取【控件位置】
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
            Rect prefixLabelPosition = GetPrefixLabelPosition(totalPosition, label);

            // 绘制【前缀标签】
            DrawGUIUtility.DrawLabel(HandleBroder(prefixLabelPosition), label);

            return HandleBroder(GetControlPosition(totalPosition, prefixLabelPosition));
        }
        #endregion
    }
}
