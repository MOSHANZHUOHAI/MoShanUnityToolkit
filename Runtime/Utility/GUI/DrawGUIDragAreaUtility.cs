using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 实用程序：IMGUI 拖拽区域绘制
    /// </summary>
    internal static class DrawGUIDragAreaUtility
    {
        #region 常量
        /// <summary>
        /// 控件哈希值
        /// </summary>
        private static readonly int CONTROL_HASH = nameof(DrawGUIDragAreaUtility).GetHashCode();
        #endregion

        #region 字段
        /// <summary>
        /// 热控件值：偏移
        /// </summary>
        /// <remarks>
        /// 开始拖拽时，点击位置与控件左上角的位置偏移
        /// </remarks>
        private static Vector2 s_HotControlValue_Offset = Vector2.zero;

        /// <summary>
        /// 热控件值：是否正在拖拽
        /// </summary>
        private static bool s_HotControlValue_IsDragging = false;
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【水平拖拽线】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户是否正在进行拖拽的判断结果。</returns>
        public static bool DrawHorizontalDragLine(Rect position, out float distance)
        {
            bool result = DrawDragLine(position, Direction2.Horizontal, out Vector2 moveDistance);

            distance = moveDistance.x;

            return result;
        }

        /// <summary>
        /// 绘制【垂直拖拽线】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户是否正在进行拖拽的判断结果。</returns>
        public static bool DrawVerticalDragLine(Rect position, out float distance)
        {
            bool result = DrawDragLine(position, Direction2.Vertical, out Vector2 moveDistance);

            distance = moveDistance.y;

            return result;
        }

        /// <summary>
        /// 绘制【拖拽线】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="direction">方向</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户是否正在进行拖拽的判断结果。</returns>
        public static bool DrawDragLine(Rect position, Direction2 direction, out Vector2 distance)
        {
            distance = Vector2.zero;

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

            #region 获取【返回值】
            switch (eventType)
            {
                // 按下鼠标
                case EventType.MouseDown:
                    // 获取【当前事件】的【鼠标位置】
                    Vector2 mousePosition = currentEvent.mousePosition;

                    // 判断 <【尺寸调整线位置】是否包含【鼠标位置】>
                    if (position.Contains(mousePosition))
                    {
                        s_HotControlValue_IsDragging = true;

                        distance = GetDistance();

                        // 使用事件
                        currentEvent.Use();

                        // 设置【GUI 实用程序】的【当前热控件标识】为【当前控件标识】
                        GUIUtility.hotControl = controlId;

                        return s_HotControlValue_IsDragging;
                    }

                    break;

                // 抬起鼠标
                case EventType.MouseUp:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        s_HotControlValue_IsDragging = false;

                        distance = GetDistance();

                        // 使用事件
                        currentEvent.Use();

                        // 重置【GUI 实用程序】中的【当前热控件标识】
                        GUIUtility.hotControl = 0;

                        return true;
                    }

                    break;

                // 拖拽鼠标
                case EventType.MouseDrag:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        s_HotControlValue_IsDragging = true;

                        distance = GetDistance();

                        // 使用事件
                        currentEvent.Use();

                        return s_HotControlValue_IsDragging;
                    }

                    break;

                // 默认
                default:
                    break;
            }
            #endregion

            // 判断 <【当前控件】是否拥有焦点>
            if (isHasFocus)
            {
                return s_HotControlValue_IsDragging;
            }

            return false;

            #region 局部方法
            // 获取【距离】
            Vector2 GetDistance()
            {
                // 判断 <【输入布局模式】是否为【水平】>
                if (direction == Direction2.Horizontal)
                {
                    return new Vector2(currentEvent.mousePosition.x - position.center.x, 0);
                }
                else
                {
                    return new Vector2(0, currentEvent.mousePosition.y - position.center.y);
                }
            }
            #endregion
        }

        // TODO: 转换为控件

        /// <summary>
        /// 绘制【拖拽区域】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户是否正在进行拖拽的判断结果。</returns>
        public static bool DrawDragArea(Rect position, out Vector2 distance)
        {
            distance = Vector2.zero;

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

            #region 获取【返回值】
            // 获取【当前事件】的【鼠标位置】
            Vector2 mousePosition;

            switch (eventType)
            {
                // 按下鼠标
                case EventType.MouseDown:
                    // 判断 <事件对应的鼠标按键是否不为【鼠标左键】>
                    if (currentEvent.button != 0)
                    {
                        break;
                    }

                    // 获取【当前事件】的【鼠标位置】
                    mousePosition = currentEvent.mousePosition;

                    // 判断 <【尺寸调整线位置】是否包含【鼠标位置】>
                    if (position.Contains(mousePosition))
                    {
                        s_HotControlValue_IsDragging = true;

                        // 记录【输入位置】到【鼠标位置】的【距离】
                        s_HotControlValue_Offset = mousePosition - position.position;

                        // 设置【GUI 实用程序】的【当前热控件标识】为【当前控件标识】
                        GUIUtility.hotControl = controlId;

                        // 使用事件
                        currentEvent.Use();

                        return s_HotControlValue_IsDragging;
                    }
                    break;

                // 抬起鼠标
                case EventType.MouseUp:
                    // 判断 <事件对应的鼠标按键是否不为【鼠标左键】>
                    if (currentEvent.button != 0)
                    {
                        break;
                    }

                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        s_HotControlValue_IsDragging = false;

                        // 获取【当前事件】的【鼠标位置】
                        mousePosition = currentEvent.mousePosition;

                        distance = mousePosition - position.position - s_HotControlValue_Offset;

                        s_HotControlValue_Offset = Vector2.zero;

                        // 重置【GUI 实用程序】中的【当前热控件标识】
                        GUIUtility.hotControl = 0;

                        return true;
                    }
                    break;

                // 拖拽鼠标
                case EventType.MouseDrag:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        s_HotControlValue_IsDragging = true;

                        // 获取【当前事件】的【鼠标位置】
                        mousePosition = currentEvent.mousePosition;

                        distance = mousePosition - position.position - s_HotControlValue_Offset;

                        // 使用事件
                        currentEvent.Use();

                        return s_HotControlValue_IsDragging;
                    }
                    break;

                // 默认
                default:
                    break;
            }
            #endregion

            // 判断 <【当前控件】是否拥有焦点>
            if (isHasFocus)
            {
                return s_HotControlValue_IsDragging;
            }

            return false;
        }
        #endregion
    }
}
