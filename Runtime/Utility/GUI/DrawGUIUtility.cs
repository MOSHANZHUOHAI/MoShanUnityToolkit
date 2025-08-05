using System;
using System.Collections.Generic;
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
        /// 组深度
        /// </summary>
        /// <remarks>
        /// 当前已记录的开启且未结束的 GUI 组的总数
        /// </remarks>
        private static int s_GroupDepth;

        /// <summary>
        /// 矩形纹理
        /// </summary>
        private static Texture2D s_RectTexture;

        /// <summary>
        /// 左中对齐标签样式
        /// </summary>
        private static readonly GUIStyle s_MiddleLeftLabelStyle;

        /// <summary>
        /// 居中对齐标签样式
        /// </summary>
        private static readonly GUIStyle s_MiddleCenterLabelStyle;

        /// <summary>
        /// 栈：颜色变更记录
        /// </summary>
        private static readonly Stack<Color> s_ColorRecords = new Stack<Color>();
        #endregion

        #region 属性
        /// <summary>
        /// 组深度
        /// </summary>
        /// <remarks>
        /// 当前已记录的开启且未结束的 GUI 组的总数
        /// </remarks>
        public static int GroupDepth
        {
            get
            {
                return s_GroupDepth;
            }
        }

        /// <summary>
        /// 颜色深度
        /// </summary>
        /// <remarks>
        /// 当前已记录的颜色变更的总数
        /// </remarks>
        public static int ColorDepth
        {
            get
            {
                return s_ColorRecords.Count;
            }
        }

        /// <summary>
        /// 矩形纹理
        /// </summary>
        internal static Texture2D RectTexture
        {
            get
            {
                // 判断 <【矩形纹理】是否为【空】>
                if (s_RectTexture == null)
                {
                    // 创建【矩形纹理】
                    s_RectTexture = new Texture2D(1, 1);

                    // 设置【矩形纹理】的【颜色】为【白色】
                    s_RectTexture.SetPixel(0, 0, Color.white);

                    // 应用设置
                    s_RectTexture.Apply();
                }

                return s_RectTexture;
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

        #region 组
        /// <summary>
        /// 开始【组】
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns>返回开始组后，当前已记录的【组】的总数。</returns>
        public static int BeginGroup(Rect position)
        {
            GUI.BeginGroup(position);

            return ++s_GroupDepth;
        }

        /// <summary>
        /// 开始【组】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="style">样式</param>
        /// <returns>返回开始组后，当前已记录的【组】的总数。</returns>
        public static int BeginGroup(Rect position, GUIStyle style)
        {
            GUI.BeginGroup(position, style);

            return ++s_GroupDepth;
        }

        /// <summary>
        /// 结束【组】
        /// </summary>
        /// <returns>返回结束组后，当前已记录的【组】的总数。</returns>
        public static int EndGroup()
        {
            // 判断 <【组深度】是否小于等于【0】>，即<是否无需结束【组】>
            if (s_GroupDepth <= 0)
            {
                return 0;
            }

            GUI.EndGroup();

            return --s_GroupDepth;
        }

        /// <summary>
        /// 结束【所有组】
        /// </summary>
        public static void EndAllGroup()
        {
            // 判断 <【组深度】是否小于等于【0】>，即<是否无需结束【组】>
            if (s_GroupDepth <= 0)
            {
                return;
            }

            while (s_GroupDepth > 0)
            {
                EndGroup();
            }
        }
        #endregion

        #region 变更【颜色】
        /// <summary>
        /// 开始【颜色变更】
        /// </summary>
        /// <returns>返回开始颜色变更后，当前已记录的【GUI 颜色】变更的总数。</returns>
        public static int BeginColorChange()
        {
            // 记录当前【GUI颜色】
            s_ColorRecords.Push(GUI.color);

            return s_ColorRecords.Count;
        }

        /// <summary>
        /// 开始【颜色变更】
        /// </summary>
        /// <param name="newColor">需要变更的新【GUI 颜色】</param>
        /// <returns>返回开始颜色变更后，当前已记录的【GUI 颜色】变更的总数。</returns>
        public static int BeginColorChange(Color newColor)
        {
            // 记录当前【GUI颜色】
            s_ColorRecords.Push(GUI.color);

            // 更新【GUI颜色】
            GUI.color = newColor;

            return s_ColorRecords.Count;
        }

        /// <summary>
        /// 结束【颜色变更】
        /// </summary>
        /// <returns>返回结束颜色变更后，当前仍记录的【GUI 颜色】变更的总数。</returns>
        public static int EndColorChange()
        {
            // 判断 <【颜色变更记录栈】是否为【空】>
            if (s_ColorRecords.Count == 0)
            {
                return 0;
            }

            // 恢复【GUI颜色】为顶层的记录颜色
            GUI.color = s_ColorRecords.Pop();

            return s_ColorRecords.Count;
        }

        /// <summary>
        /// 结束【所有颜色变更】
        /// </summary>
        public static void EndAllColorChange()
        {
            // 判断 <【颜色变更记录栈】是否为【空】>
            if (s_ColorRecords.Count == 0)
            {
                return;
            }

            // While 循环以恢复【GUI颜色】为最底层的记录颜色
            while (s_ColorRecords.Count > 0)
            {
                // 恢复【GUI颜色】为当前循环轮次对应的颜色
                GUI.color = s_ColorRecords.Pop();
            }
        }

        /// <summary>
        /// 变更【颜色】
        /// </summary>
        /// <param name="newColor">需要变更的新【GUI 颜色】</param>
        public static void ChangeColor(Color newColor)
        {
            GUI.color = newColor;
        }
        #endregion

        #region 绘制【控件】
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
        /// 绘制【颜色字段】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="value">值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的颜色。</returns>
        public static Color DrawColorField(Rect position, Color value, bool isRetrunImmediately = false)
        {
            return DrawGUIColorFieldUtility.DrawColorField(position, value, isRetrunImmediately);
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
        public static float DrawCircleSlider(Rect position, float knobRadius, float value, float min, float max, bool isCounterclockwise = false, bool isRoundToInt = false, bool isRetrunImmediately = false)
        {
            return DrawGUICircleSliderUtility.DrawCircleSlider(position, knobRadius, value, min, max, isCounterclockwise, isRoundToInt, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【拖拽线】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="direction">方向</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户输入的拖拽线位置。</returns>
        public static Rect DrawDragLine(Rect position, Direction2 direction, out Vector2 distance)
        {
            return DrawGUIDragAreaUtility.DrawDragLine(position, direction, out distance);
        }

        /// <summary>
        /// 绘制【拖拽区域】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户输入的拖拽区域位置。</returns>
        public static Rect DrawDragArea(Rect position, out Vector2 distance)
        {
            return DrawGUIDragAreaUtility.DrawDragArea(position, out distance);
        }
        #endregion

        #endregion
    }
}
