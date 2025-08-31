using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 实用程序：IMGUI 颜色字段绘制
    /// </summary>
    public static class DrawGUIColorFieldUtility
    {
        #region 常量
        /// <summary>
        /// 控件哈希值
        /// </summary>
        private static readonly int CONTROL_HASH = nameof(DrawGUIColorFieldUtility).GetHashCode();
        #endregion

        #region 字段
        /// <summary>
        /// 是否正在编辑
        /// </summary>
        private static bool s_IsEditoring = false;

        /// <summary>
        /// 热控件 ID
        /// </summary>
        private static int s_HotControlId = 0;

        /// <summary>
        /// 热控件值
        /// </summary>
        private static Color s_HotControlValue = Color.clear;
        #endregion

        #region 属性
        /// <summary>
        /// 矩形纹理
        /// </summary>
        private static Texture2D RectTexture
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
        static DrawGUIColorFieldUtility() { }
        #endregion

        #region 私有方法
        /// <summary>
        /// 绘制【颜色字段】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="value">值</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的颜色。</returns>
        public static Color DrawColorField(Rect position, Color value, bool isRetrunImmediately)
        {
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

            #region 绘制【控件】
            // 绘制【背景】
            GUI.DrawTexture(position, RectTexture, ScaleMode.StretchToFill, true, 0.0f, Color.black, 0.0f, 0.0f);

            // 绘制【颜色】
            GUI.DrawTexture(position, RectTexture, ScaleMode.StretchToFill, true, 0.0f, value, 0.0f, 0.0f);
            #endregion

            #region 获取【返回值】
            // 获取【返回值】
            Color result = value;

            switch (eventType)
            {
                // 按下鼠标
                case EventType.MouseDown:
                    // 获取【当前事件】的【鼠标位置】
                    Vector2 mousePosition = currentEvent.mousePosition;

                    // 判断 <【位置】是否包含【鼠标位置】>
                    if (position.Contains(mousePosition))
                    {
                        // 设置【GUI 实用程序】的【当前热控件标识】为【当前控件标识】
                        GUIUtility.hotControl = controlId;

                        s_HotControlId = controlId;

                        // 初始化【热控件值】
                        s_HotControlValue = value;

                        result = value;

                        // TODO: 开启窗口
                        // TODO: 设置窗口颜色

                        Debug.Log("开启颜色窗口");
                    }
                    break;

                // 默认
                default:
                    break;
            }

            // 判断 <【热控件 ID】是否等于【当前控件标识】>、<是否正在编辑>
            if (s_HotControlId == controlId && s_IsEditoring)
            {
                // TODO: 获取窗口颜色
                // s_HotControlValue = window.Color;

                Debug.Log("更新颜色");

                // 判断 <是否立即返回结果>
                if (isRetrunImmediately)
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
            }
            #endregion

            return result;
        }
        #endregion
    }
}
