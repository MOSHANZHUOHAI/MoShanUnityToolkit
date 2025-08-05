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
        /// 热控件值
        /// </summary>
        private static Vector2 s_HotControlValue = Vector2.zero;
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【水平拖拽线】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户输入的水平拖拽线位置。</returns>
        public static Rect DrawHorizontalDragLine(Rect position, out float distance)
        {
            Rect result = DrawDragLine(position, Direction2.Horizontal, out Vector2 moveDistance);

            distance = moveDistance.x;

            return result;
        }

        /// <summary>
        /// 绘制【垂直拖拽线】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户输入的垂直拖拽线位置。</returns>
        public static Rect DrawVerticalDragLine(Rect position, out float distance)
        {
            Rect result = DrawDragLine(position, Direction2.Vertical, out Vector2 moveDistance);

            distance = moveDistance.y;

            return result;
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
            distance = Vector2.zero;

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

            #region 获取【返回值】
            Rect result = position;

            switch (eventType)
            {
                // 按下鼠标
                case EventType.MouseDown:
                    // 获取【当前事件】的【鼠标位置】
                    Vector2 mousePosition = currentEvent.mousePosition;

                    // 判断 <【尺寸调整线位置】是否包含【鼠标位置】>
                    if (position.Contains(mousePosition))
                    {
                        distance = GetDistance();

                        GetPosition();

                        // 设置【GUI 实用程序】的【当前热控件 ID】为【当前控件 ID】
                        GUIUtility.hotControl = controlId;
                    }
                    break;

                // 抬起鼠标
                case EventType.MouseUp:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        distance = GetDistance();

                        GetPosition();

                        // 重置【GUI 实用程序】中的【当前热控件 ID】
                        GUIUtility.hotControl = 0;
                    }
                    break;

                // 拖拽鼠标
                case EventType.MouseDrag:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        distance = GetDistance();

                        GetPosition();

                        // 使用事件
                        currentEvent.Use();
                    }
                    break;

                // 默认
                default:
                    break;
            }
            #endregion

            return result;

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

            // 获取【位置】
            void GetPosition()
            {
                // 判断 <【输入布局模式】是否为【水平】>
                if (direction == Direction2.Horizontal)
                {
                    result.x -= currentEvent.mousePosition.x - position.center.x;
                }
                else
                {
                    result.y -= currentEvent.mousePosition.y - position.center.y;
                }
            }
            #endregion
        }

        /// <summary>
        /// 绘制【拖拽区域】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">移动距离</param>
        /// <returns>返回用户输入的拖拽区域位置。</returns>
        public static Rect DrawDragArea(Rect position, out Vector2 distance)
        {
            distance = Vector2.zero;

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

            #region 获取【返回值】
            Rect result = position;

            // 获取【当前事件】的【鼠标位置】
            Vector2 mousePosition;

            switch (eventType)
            {
                // 按下鼠标
                case EventType.MouseDown:
                    // 获取【当前事件】的【鼠标位置】
                    mousePosition = currentEvent.mousePosition;

                    // 判断 <【尺寸调整线位置】是否包含【鼠标位置】>
                    if (position.Contains(mousePosition))
                    {
                        // 记录【输入位置】到【鼠标位置】的【距离】
                        s_HotControlValue = mousePosition - position.position;

                        // 设置【GUI 实用程序】的【当前热控件 ID】为【当前控件 ID】
                        GUIUtility.hotControl = controlId;
                    }
                    break;

                // 抬起鼠标
                case EventType.MouseUp:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        // 获取【当前事件】的【鼠标位置】
                        mousePosition = currentEvent.mousePosition;

                        distance = mousePosition - position.position - s_HotControlValue;

                        result.position += distance;

                        s_HotControlValue = Vector2.zero;

                        // 重置【GUI 实用程序】中的【当前热控件 ID】
                        GUIUtility.hotControl = 0;
                    }
                    break;

                // 拖拽鼠标
                case EventType.MouseDrag:
                    // 判断 <【当前控件】是否拥有焦点>
                    if (isHasFocus)
                    {
                        // 获取【当前事件】的【鼠标位置】
                        mousePosition = currentEvent.mousePosition;

                        distance = mousePosition - position.position - s_HotControlValue;

                        result.position += distance;

                        // 使用事件
                        currentEvent.Use();
                    }
                    break;

                // 默认
                default:
                    break;
            }
            #endregion

            return result;
        }
        #endregion
    }
}
