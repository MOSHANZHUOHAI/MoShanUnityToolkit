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
        /// 绘制【旋钮】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="knobRadius">旋钮半径，取值范围为[0, +∞)</param>
        /// <param name="angle">角度（角度制），取值范围为[0, 360)</param>
        /// <param name="isRoundToInt">是否对返回结果进行四舍五入取整</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回旋钮的旋转角度，取值范围为[0, 360)。</returns>
        public static float DrawKnob(Rect position, float knobRadius, float angle, bool isRoundToInt = false, bool isRetrunImmediately = false)
        {
            return DrawGUIKnobUtility.DrawKnob(position, knobRadius, angle, isRoundToInt, isRetrunImmediately);
        }
        #endregion
    }
}
