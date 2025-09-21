using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 工具窗口
    /// </summary>
    [Serializable]
    public abstract class ToolWindow : Window
    {
        #region 常量
        /// <summary>
        /// 工具栏背景样式名称
        /// </summary>
        private const string TOOLBAR_BACKGROUND_STYLE_NAME = "Window_Toolbar_Background";
        #endregion

        #region 静态属性
        /// <summary>
        /// 风格
        /// </summary>
        private static GUISkin Skin
        {
            get
            {
                return RuntimeDockUtility.Skin;
            }
        }
        #endregion

        #region 静态私有方法
        /// <summary>
        /// 获取【样式】
        /// </summary>
        /// <param name="name">样式名称</param>
        /// <param name="defaultStyle">默认样式</param>
        /// <returns>若获取成功，返回【输入名称】对应的【样式】；否则，返回【输入默认样式】；</returns>
        private static GUIStyle GetStyle(string name, GUIStyle defaultStyle)
        {
            // 判断 <【风格】是否为【空】>
            if (Skin == null)
            {
                return defaultStyle;
            }

            // 获取【样式】
            GUIStyle style = Skin.FindStyle(name);

            // 判断 <【样式】是否为【空】>
            if (style == null)
            {
                style = defaultStyle;
            }

            return style;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 标题栏高度
        /// </summary>
        /// <remarks>
        /// 取值范围为[0, +∞)，若返回值小于等于【0】，则视为不绘制标题栏
        ///  </remarks>
        public virtual int ToolbarHeight
        {
            get
            {
                return 0;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public ToolWindow() : base() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        public ToolWindow(string name) : base(name) { }
        #endregion

        #region 私有方法
        /// <inheritdoc/>
        protected sealed override void OnDraw(Rect position)
        {
            // 判断 <【工具栏高度】是否大于【0】>，即<是否需要绘制工具栏>
            if (ToolbarHeight > 0)
            {
                DrawGUIUtility.BeginGroup
                (
                    new Rect(position.xMin, position.yMin, position.width, ToolbarHeight),
                    GetStyle(TOOLBAR_BACKGROUND_STYLE_NAME, GUI.skin.box)
                );

                OnDrawToolbar(new Rect(0, 0, position.width, ToolbarHeight));

                DrawGUIUtility.EndGroup();

                position.yMin += ToolbarHeight;
            }

            DrawGUIUtility.BeginGroup(position);

            OnDrawContent(new Rect(0, 0, position.width, position.height));

            DrawGUIUtility.EndGroup();
        }

        /// <summary>
        /// 绘制【工具栏】
        /// </summary>
        /// <param name="position">位置，初始坐标为(0, 0)</param>
        protected abstract void OnDrawToolbar(Rect position);

        /// <summary>
        /// 绘制【内容】
        /// </summary>
        /// <param name="position">位置，初始坐标为(0, 0)</param>
        protected abstract void OnDrawContent(Rect position);
        #endregion
    }
}
