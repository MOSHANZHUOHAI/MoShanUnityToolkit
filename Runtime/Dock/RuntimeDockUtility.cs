using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 实用程序：运行时停靠
    /// </summary>
    public static class RuntimeDockUtility
    {
        #region 常量
        /// <summary>
        /// 工具栏高度
        /// </summary>
        private const int TOOLBAR_HEIGHT = 28;

        /// <summary>
        /// 风格路径
        /// </summary>
        private const string SKIN_PATH = "GUISkins/RuntimeDockGUISkin";
        #endregion

        #region 字段
        /// <summary>
        /// 是否启用
        /// </summary>
        private static bool s_IsEnable = true;

        /// <summary>
        /// 是否绘制固定布局
        /// </summary>
        private static bool s_IsDrawFixedLayout = true;

        /// <summary>
        /// 是否绘制浮动布局
        /// </summary>
        private static bool s_IsDrawFloatingLayout = true;

        /// <summary>
        /// 尺寸
        /// </summary>
        private static Vector2 s_Size;

        /// <summary>
        /// 聚焦布局
        /// </summary>
        private static DockLayout s_FocusingLayout;

        /// <summary>
        /// 风格
        /// </summary>
        private static GUISkin s_Skin;

        /// <summary>
        /// 固定布局
        /// </summary>
        private static readonly DockLayout s_FixedLayout;

        /// <summary>
        /// 列表：浮动布局
        /// </summary>
        private static readonly List<DockLayout> s_FloatingLayouts = new List<DockLayout>();
        #endregion

        #region 属性
        /// <summary>
        /// 是否启用
        /// </summary>
        internal static bool IsEnable
        {
            get
            {
                return s_IsEnable;
            }
            set
            {
                // 判断 <【是否启用】是否等于【输入值】>
                if (s_IsEnable == value)
                {
                    return;
                }

                s_IsEnable = value;
            }
        }

        /// <summary>
        /// 风格
        /// </summary>
        internal static GUISkin Skin
        {
            get
            {
                return s_Skin;
            }
        }

        /// <summary>
        /// 窗口样式
        /// </summary>
        private static GUIStyle WindowStyle
        {
            get
            {
                // 判断 <【风格】是否为【空】>
                if (s_Skin == null)
                {
                    return GUI.skin.window;
                }

                return s_Skin.window;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static RuntimeDockUtility()
        {
            s_Size = new Vector2(Screen.width, Screen.height);

            // 从【资源】文件夹中获取【GUI 风格】
            s_Skin = Resources.Load<GUISkin>(SKIN_PATH);

            // 创建【根布局】
            s_FixedLayout = DockLayout.CreateFixedLayout(new Rect
            (
                0,
                0,
                Screen.width,
                Screen.height - TOOLBAR_HEIGHT
            ));
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 添加【窗口】
        /// </summary>
        /// <param name="window">窗口</param>
        public static void AddWindow(Window window)
        {
            s_FloatingLayouts.Add
            (
                DockLayout.CreateFloatingLayout
                (
                    new Rect(s_FixedLayout.Position.position + s_FixedLayout.Position.size * 0.25f, s_FixedLayout.Position.size * 0.5f),
                    s_FixedLayout.Position,
                    window
                )
            );
        }

        /// <summary>
        /// 添加【窗口】
        /// </summary>
        /// <param name="windows">所有窗口</param>
        public static void AddWindow(params Window[] windows)
        {
            s_FloatingLayouts.Add
            (
                DockLayout.CreateFloatingLayout
                (
                    new Rect(s_FixedLayout.Position.position + s_FixedLayout.Position.size * 0.25f, s_FixedLayout.Position.size * 0.5f),
                    s_FixedLayout.Position,
                    windows
                )
            );
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 绘制
        /// </summary>
        internal static void Draw()
        {
            #region 绘制【工具栏】
            // 获取【工具栏位置】
            Rect toolbarPosition = new Rect(0, 0, s_Size.x, TOOLBAR_HEIGHT);

            DrawGUIUtility.BeginGroup(toolbarPosition, WindowStyle);

            // 重置【内容位置】的坐标到当前绘制区域的左上角
            toolbarPosition.position = Vector2.zero;

            DrawToolbar(toolbarPosition);

            DrawGUIUtility.EndGroup();
            #endregion

            // 判断 <是否未启用>
            if (!s_IsEnable)
            {
                return;
            }

            #region 绘制【内容】
            // 获取【内容位置】
            Rect contentPosition = new Rect(0, TOOLBAR_HEIGHT, s_Size.x, s_Size.y - TOOLBAR_HEIGHT);

            DrawGUIUtility.BeginGroup(contentPosition);

            // 重置【内容位置】的坐标到当前绘制区域的左上角
            contentPosition.position = Vector2.zero;

            DrawContnet(contentPosition);

            DrawGUIUtility.EndGroup();
            #endregion
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 绘制【工具栏】
        /// </summary>
        /// <param name="position">位置</param>
        private static void DrawToolbar(Rect position)
        {
            GUILayout.BeginHorizontal();

            s_IsEnable             = GUILayout.Toggle(s_IsEnable            , new GUIContent("启用"));

            s_IsDrawFixedLayout    = GUILayout.Toggle(s_IsDrawFixedLayout   , new GUIContent("固定窗口"));

            s_IsDrawFloatingLayout = GUILayout.Toggle(s_IsDrawFloatingLayout, new GUIContent("浮动窗口"));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制【内容】
        /// </summary>
        /// <param name="position">位置</param>
        private static void DrawContnet(Rect position)
        {
            // 判断 <【当前事件】的【类型】是否为【按下鼠标】>
            if (Event.current.type == EventType.MouseDown)
            {
                FocusLayout(GetFocusingLayout());
            }

            DrawFixedLayout();

            DrawFloatingLayouts();
        }

        /// <summary>
        /// 获取【聚焦布局】
        /// </summary>
        private static DockLayout GetFocusingLayout()
        {
            // 获取【鼠标位置】
            Vector2 mousePosition = Event.current.mousePosition;

            #region 聚焦【浮动布局】
            // 判断 <是否绘制【浮动布局】>
            if (s_IsDrawFloatingLayout)
            {
                // 循环以置顶焦点所在的浮动布局
                for (int i = s_FloatingLayouts.Count - 1; i >= 0; i--)
                {
                    // 获取【当前布局】
                    DockLayout currentLayout = s_FloatingLayouts[i];

                    // 判断 <【当前布局】是否为【空】>
                    if (currentLayout == null)
                    {
                        continue;
                    }

                    // 判断 <【当前布局】是否不包含【鼠标位置】>
                    if (!currentLayout.Position.Contains(mousePosition))
                    {
                        continue;
                    }

                    return currentLayout;
                }
            }
            #endregion

            #region 聚焦【固定布局】
            // 判断 <是否绘制【固定布局】>
            if (s_IsDrawFixedLayout)
            {
                // 判断 <获取【聚焦布局】是否成功>、<【聚焦布局】的【内容】是否不为【空】>
                if (s_FixedLayout.GetFocusingLayout(mousePosition, out DockLayout focusLayout) && focusLayout.Content != null)
                {
                    return focusLayout;
                }
            }
            #endregion

            return null;
        }

        /// <summary>
        /// 聚焦【布局】
        /// </summary>
        /// <param name="layout">布局</param>
        private static void FocusLayout(DockLayout layout)
        {
            // 判断 <【聚焦布局】是否等于【输入布局】>
            if (s_FocusingLayout == layout)
            {
                return;
            }

            s_FocusingLayout = layout;

            // 判断 <【聚焦布局】是否为【空】>
            if (s_FocusingLayout == null)
            {
                return;
            }

            // 判断 <移除【聚焦布局】是否成功>
            if (s_FloatingLayouts.Remove(s_FocusingLayout))
            {
                // 添加【聚焦布局】以置顶【聚焦布局】
                s_FloatingLayouts.Add(s_FocusingLayout);
            }
        }

        /// <summary>
        /// 绘制【固定布局】
        /// </summary>
        private static void DrawFixedLayout()
        {
            // 判断 <是否不绘制【固定布局】>
            if (!s_IsDrawFixedLayout)
            {
                return;
            }

            s_FixedLayout.Draw();
        }

        /// <summary>
        /// 绘制【浮动布局】
        /// </summary>
        private static void DrawFloatingLayouts()
        {
            // 判断 <是否不绘制【浮动布局】>
            if (!s_IsDrawFloatingLayout)
            {
                return;
            }

            // 判断 <【浮动停靠列表】是否不为【空】>
            if (s_FloatingLayouts != null && s_FloatingLayouts.Count > 0)
            {
                // 循环以绘制【所有浮动停靠】
                for (int i = 0; i < s_FloatingLayouts.Count; i++)
                {
                    // 获取【当前布局】
                    DockLayout currentLayout = s_FloatingLayouts[i];

                    // 判断 <【当前布局】是否为【空】>、<【当前布局】是否已关闭>
                    if (currentLayout == null || currentLayout.IsClosed)
                    {
                        s_FloatingLayouts.RemoveAt(i);

                        i--;

                        continue;
                    }

                    currentLayout.Draw();
                }
            }
        }
        #endregion
    }
}
