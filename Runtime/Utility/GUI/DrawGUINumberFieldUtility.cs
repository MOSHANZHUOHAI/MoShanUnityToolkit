using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 实用程序：IMGUI 数字字段绘制
    /// </summary>
    internal static class DrawGUINumberFieldUtility
    {
        #region 常量
        /// <summary>
        /// 控件哈希值
        /// </summary>
        private static readonly int CONTROL_HASH = nameof(DrawGUITextFieldUtility).GetHashCode();
        #endregion

        #region 字段
        /// <summary>
        /// 热控件值
        /// </summary>
        private static string s_HotControlValue = string.Empty;
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【整型字段】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="number">数字</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的整型内容。</returns>
        public static int DrawIntField(Rect position, int number, bool isRetrunImmediately)
        {
            return (int)DrawNumberField(position, number, isRetrunImmediately);
        }

        /// <summary>
        /// 绘制【数字字段】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="number">数字</param>
        /// <param name="isRetrunImmediately">是否立即返回结果</param>
        /// <returns>返回用户输入的数字内容。</returns>
        public static float DrawNumberField(Rect position, float number, bool isRetrunImmediately)
        {
            #region 初始化【参数】
            // 获取【文本】
            string text = number.ToString();

            // 设置【文本字段高度】
            position.height = 18;

            // 判断 <【输入文本】是否为【空】>
            if (text == null)
            {
                text = string.Empty;
            }
            #endregion

            // 获取【控件编号】
            int controlId = GUIUtility.GetControlID(CONTROL_HASH, FocusType.Passive, position);

            #region 获取【事件信息】
            // 获取【当前事件】
            Event currentEvent = Event.current;

            // 获取【控件编号】对应的【当前事件类型】
            EventType eventType = currentEvent.GetTypeForControl(controlId);
            #endregion

            // 获取【控件名称】
            string controlName = $"{CONTROL_HASH}_{controlId}";

            // 设置【当前控件】的【名称】
            GUI.SetNextControlName(controlName);

            #region 获取【返回值】
            // 获取【返回值】
            float result = number;

            // 判断 <【当前焦点所在的命名控件】与【当前控件】的【名称】是否相等>
            if (GUI.GetNameOfFocusedControl() == controlName)
            {
                // 绘制【文本控件】以更新【热控件值】
                s_HotControlValue = GUI.TextField(position, s_HotControlValue);

                // 判断 <<【当前事件】是否为【输入按键】>、<【输入按键】是否为【回车键】>>
                if ((currentEvent.isKey && currentEvent.keyCode == KeyCode.Return)
                    // 或<<【当前事件类型】是否为【按下鼠标】>、<【输入位置】是否不包含【鼠标位置】>>
                    || (eventType == EventType.MouseDown && !position.Contains(currentEvent.mousePosition)))
                {
                    // 重置【聚焦控件编号】
                    GUIUtility.hotControl      = 0;
                    GUIUtility.keyboardControl = 0;

                    UpdateResult();
                }

                // 判断 <是否立即返回结果>
                if (isRetrunImmediately)
                {
                    UpdateResult();
                }
            }
            else
            {
                // 判断 <【当前事件类型】是否为【按下鼠标】>、<【位置】是否包含【鼠标位置】>
                if (eventType == EventType.MouseDown && position.Contains(currentEvent.mousePosition))
                {
                    // 初始化【热控件值】
                    s_HotControlValue = text;

                    // 绘制【文本控件】
                    GUI.TextField(position, s_HotControlValue);

                    // 聚焦【当前控件】
                    GUI.FocusControl(controlName);

                    // 使用事件
                    currentEvent.Use();
                }
                else
                {
                    // 绘制【文本控件】
                    GUI.TextField(position, text);
                }
            }
            #endregion

            return result;

            #region 局部方法
            // 局部方法：更新返回值
            void UpdateResult()
            {
                // 判断 <【热控件值】是否可转换为【浮点数】>
                if (float.TryParse(s_HotControlValue, out float value))
                {
                    // 设置【返回值】为【热控件值】
                    result = value;

                    // 判断 <【输入值】是否不等于【返回值】>
                    if (number != result)
                    {
                        // 通知 GUI 已发生变更
                        GUI.changed = true;
                    }
                }
                else
                {
                    result = number;
                }
            }
            #endregion
        }
        #endregion
    }
}
