using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 实用程序：IMGUI 绘制
    /// </summary>
    /// <remarks>
    /// 内部一切涉及角度的计算均采用角度制，即输入角度与输出角度的取值范围均为[0°, 360°)，以正右为【0°】，逆时针增长
    /// </remarks>
    public static class DrawGUIUtility
    {
        #region 常量
        /// <summary>
        /// 缩进空间
        /// </summary>
        private const int INDENT_SPACE = 15;

        /// <summary>
        /// 前缀标签宽度
        /// </summary>
        /// <remarks>
        /// 前缀标签的宽度上限
        /// </remarks>
        private const int PREFIX_LABEL_WIDTH = 150;

        /// <summary>
        /// 控件宽度
        /// </summary>
        /// <remarks>
        /// 控件的宽度下限
        /// </remarks>
        private const int CONTROL_WIDTH = 50;
        #endregion

        #region 字段
        /// <summary>
        /// 缩进级别
        /// </summary>
        private static int s_IndentLevel = 0;

        /// <summary>
        /// 左中对齐标签样式
        /// </summary>
        private readonly static GUIStyle s_MiddleLeftLabelStyle;

        /// <summary>
        /// 居中对齐标签样式
        /// </summary>
        private readonly static GUIStyle s_MiddleCenterLabelStyle;
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
                return s_IndentLevel * INDENT_SPACE;
            }
        }

        /// <summary>
        /// 左中对齐标签样式
        /// </summary>
        internal static GUIStyle MiddleLeftLabelStyle
        {
            get
            {
                return s_MiddleLeftLabelStyle;
            }
        }

        /// <summary>
        /// 居中对齐标签样式
        /// </summary>
        internal static GUIStyle MiddleCenterLabelStyle
        {
            get
            {
                return s_MiddleCenterLabelStyle;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static DrawGUIUtility()
        {
            s_MiddleLeftLabelStyle   = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };

            s_MiddleCenterLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【标签】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="label">标签</param>
        public static void DrawLabel(Rect position, string label)
        {
            DrawLabel(position, new GUIContent(label));
        }

        /// <summary>
        /// 绘制【标签】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="label">标签</param>
        public static void DrawLabel(Rect position, GUIContent label)
        {
            GUI.Label(position, label);
        }

        /// <summary>
        /// 绘制【前缀标签】
        /// </summary>
        /// <param name="totalPosition">用于绘制前缀标签和后续控件的绘制位置</param>
        /// <returns>返回绘制前缀标签之后仍可用于后续控件的绘制位置。</returns>
        public static Rect DrawPrefixLabel(Rect totalPosition, GUIContent label)
        {
            // 判断 <【标签】是否为【空】>
            if (label == GUIContent.none)
            {
                return totalPosition;
            }

            // 获取【边界宽度】
            int borderWidth = 4;

            // 获取【标签宽度】
            // 标签宽度 = 总宽度 - 控件宽度 - 缩进空间 - 2 * 边界宽度
            float prefixLabel = totalPosition.width - CONTROL_WIDTH - IndentSpace - 2 * borderWidth;

            // 判断 <【标签宽度】是否小于【0】>，即<是否无法绘制【前缀标签】>
            if (prefixLabel <= 0)
            {
                return totalPosition;
            }

            // 获取【标签位置】
            Rect labelPosition = new Rect
            (
                totalPosition.x + IndentSpace + borderWidth,
                totalPosition.y,
                prefixLabel,
                totalPosition.height
            );

            // 绘制【标签】
            DrawLabel(labelPosition, label);

            return new Rect
            (
                totalPosition.x + PREFIX_LABEL_WIDTH + borderWidth,
                totalPosition.y,
                totalPosition.width - PREFIX_LABEL_WIDTH - 2 * borderWidth,
                totalPosition.height
            );
        }

        /// <summary>
        /// 绘制【切换】
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value">布尔值</param>
        /// <returns>返回用户输入的布尔内容。</returns>
        public static bool DrawToggle(Rect position, bool value)
        {
            return GUI.Toggle(position, value, GUIContent.none);
        }

        /// <summary>
        /// 绘制【文本字段】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="text">文本</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的文本内容。</returns>
        public static string DrawTextField(Rect position, string text, bool isRetrunImmediately = false)
        {
            return DrawGUITextFieldUtility.DrawTextField(position, text, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【浮点数字段】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="value">值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的浮点数内容。</returns>
        public static float DrawFloatField(Rect position, float value, bool isRetrunImmediately = false)
        {
            return DrawGUINumberFieldUtility.DrawNumberField(position, value, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【整型字段】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="value">值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的整型内容。</returns>
        public static int DrawIntField(Rect position, int value, bool isRetrunImmediately = false)
        {
            return DrawGUINumberFieldUtility.DrawIntField(position, value, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【滑动条】
        /// </summary>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="position">位置</param>
        /// <param name="value">滑动条值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的滑动条值。</returns>
        public static float DrawSlider(Rect position, float knobRadius, float value, float min, float max, bool isRoundToInt, bool isRetrunImmediately)
        {
            return DrawGUISliderUtility.DrawSlider(position, knobRadius, value, min, max, false, isRoundToInt, isRetrunImmediately);
        }

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
            return DrawGUISliderUtility.DrawIntSlider(position, knobRadius, value, min, max, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【旋钮】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="angle">角度（角度制），取值范围为[0, 360)</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的旋转角度，取值范围为[0, 360)。</returns>
        public static float DrawKnob(Rect position, float knobRadius, float angle, bool isRoundToInt = false, bool isRetrunImmediately = false)
        {
            return DrawGUIKnobUtility.DrawKnob(position, knobRadius, angle, isRoundToInt, isRetrunImmediately);
        }
        #endregion
    }
}
