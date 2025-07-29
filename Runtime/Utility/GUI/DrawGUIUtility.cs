using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 实用程序：IMGUI 绘制
    /// </summary>
    /// <remarks>
    /// <para>用于在【OnGUI】生命周期方法期间进行绘制</para>
    /// <para>内部一切涉及角度的计算均采用角度制，即输入角度与输出角度的取值范围均为[0°, 360°)，以正右为【0°】，逆时针增长</para>
    /// </remarks>
    public static class DrawGUIUtility
    {
        #region 字段
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
        public static void DrawLabel(Rect position, GUIContent label)
        {
            GUI.Label(position, label);
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
