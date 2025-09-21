using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 停靠
    /// </summary>
    [Serializable]
    internal sealed class Dock : IEquatable<Dock>
    {
        #region 常量
        /// <summary>
        /// 标题栏高度
        /// </summary>
        /// <remarks>
        /// 取值范围为[1, +∞)
        /// </remarks>
        private const int HEADER_HEIGHT = 28;

        /// <summary>
        /// 标题栏按钮宽度
        /// </summary>
        private const int HEADER_BUTTON_WIDTH = 28;

        /// <summary>
        /// 拖拽选项卡滚动条按钮宽度
        /// </summary>
        private const int DRAGTAB_SCROLLER_BUTTON_WIDTH = 16;

        /// <summary>
        /// 拖拽选项卡宽度
        /// </summary>
        /// <remarks>
        /// 取值范围为[0, +∞)
        /// </remarks>
        private const int DRAGTAB_WIDTH = 120;

        /// <summary>
        /// 标题栏背景样式名称
        /// </summary>
        private const string HEADER_BACKGROUND_STYLE_NAME      = "Dock_Header_Background";

        /// <summary>
        /// 关闭按钮样式名称
        /// </summary>
        private const string CLOSE_BUTTON_STYLE_NAME           = "Dock_Header_Button_CloseWindow";

        /// <summary>
        /// 最大化按钮样式名称
        /// </summary>
        private const string MAXIMIZE_BUTTON_STYLE_NAME        = "Dock_Header_Button_MaximizeWindow";

        /// <summary>
        /// 上一个按钮样式名称
        /// </summary>
        private const string PREV_BUTTON_STYLE_NAME            = "Dock_Header_DragtagScroller_Button_Prev";

        /// <summary>
        /// 下一个按钮样式名称
        /// </summary>
        private const string NEXT_BUTTON_STYLE_NAME            = "Dock_Header_DragtagScroller_Button_Next";

        /// <summary>
        /// 拖拽选项卡样式名称
        /// </summary>
        private const string DRAGTAB_STYLE_NAME                = "Dock_Header_DragtagScroller_Dragtag";

        /// <summary>
        /// 选项卡窗口背景样式名称
        /// </summary>
        private const string TAB_WINDOW_BACKGROUND_STYLE_NAME  = "Dock_TabWindow_Background";
        #endregion

        #region 静态字段
        /// <summary>
        /// 全局唯一标识
        /// </summary>
        private static int s_GloballyUniqueIdentifier = 0;
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

        #region 静态内部方法
        /// <summary>
        /// 创建【停靠】
        /// </summary>
        /// <param name="window">窗口</param>
        /// <returns>若创建成功，返回新建的【停靠】实例；否则，返回【空】。</returns>
        internal static Dock CreateDock(Window window)
        {
            // 判断 <【输入窗口】是否为【空】>
            if (window == null)
            {
                return null;
            }

            // 创建【停靠】
            Dock dock = new Dock();

            dock.m_Windows.Add(window);

            return dock;
        }

        /// <summary>
        /// 创建【停靠】
        /// </summary>
        /// <param name="windows">所有窗口</param>
        /// <returns>若创建成功，返回新建的【停靠】实例；否则，返回【空】。</returns>
        internal static Dock CreateDock(params Window[] windows)
        {
            // 判断 <【输入窗口】是否为【空】>
            if (windows == null || windows.Length == 0 || windows.All(item => item == null))
            {
                return null;
            }

            // 创建【停靠】
            Dock dock = new Dock();

            // 循环以添加所有窗口
            for (int i = 0; i < windows.Length; i++)
            {
                // 判断 <【当前窗口】是否为【空】>
                if (windows[i] == null)
                {
                    continue;
                }

                dock.m_Windows.Add(windows[i]);
            }

            return dock;
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

        #region 字段
        /// <summary>
        /// 选中选项卡索引
        /// </summary>
        private int m_SelectedTabIndex = 0;

        /// <summary>
        /// 标题栏滚动条位置
        /// </summary>
        private float m_HeaderScrollerPosition = 0;

        /// <summary>
        /// 状态
        /// </summary>
        private DockState m_State = DockState.Default;

        /// <summary>
        /// 全局唯一标识
        /// </summary>
        private readonly int m_Guid;

        /// <summary>
        /// 拖拽选项卡滚动条哈希值
        /// </summary>
        private readonly int m_DragtabScrollerHash;

        /// <summary>
        /// 列表：窗口
        /// </summary>
        private readonly List<Window> m_Windows = new List<Window>();
        #endregion

        #region 属性
        /// <summary>
        /// 全局唯一标识
        /// </summary>
        internal int Guid
        {
            get
            {
                return m_Guid;
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        internal DockState State
        {
            get
            {
                return m_State;
            }
        }

        /// <summary>
        /// 窗口总数
        /// </summary>
        internal int WindowCount
        {
            get
            {
                // 判断 <【窗口列表】是否为【空】>
                if (m_Windows == null)
                {
                    return 0;
                }

                return m_Windows.Count;
            }
        }
        #endregion

        #region 委托
        /// <summary>
        /// 关闭时
        /// </summary>
        internal event Action OnClosing;

        /// <summary>
        /// 最大化时
        /// </summary>
        internal event Action OnMaximizing;

        /// <summary>
        /// 拖拽时
        /// </summary>
        internal event Action<Vector2> OnDragging;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        private Dock()
        {
            m_Guid = s_GloballyUniqueIdentifier++;

            m_DragtabScrollerHash = nameof(Dock).GetHashCode() ^ (m_Guid.GetHashCode() << 2);
        }
        #endregion

        #region 公开方法

        #region 相等
        /// <summary>
        /// 获取【哈希码】
        /// </summary>
        /// <returns>返回该类型实例的哈希码。</returns>
        public override int GetHashCode()
        {
            return m_Guid.GetHashCode();
        }

        /// <summary>
        /// 判断【是否相等】
        /// </summary>
        /// <param name="other">其它对象</param>
        /// <returns>与输入对象进行相等比较的判断结果。</returns>
        public override bool Equals(object other)
        {
            // 判断 <【输入值】是否不为指定类型>
            if (!(other is Dock))
            {
                return false;
            }

            return Equals((Dock)other);
        }

        /// <summary>
        /// 判断【是否相等】
        /// </summary>
        /// <remarks>
        /// 接口：<see cref="IEquatable{T}">相等</see>
        /// </remarks>
        /// <param name="other">其它同类型实例</param>
        /// <returns>返回该实例与其它同类型实例进行相等比较的判断结果。</returns>
        public bool Equals(Dock other)
        {
            // 判断 <【输入值】是否为【空】>
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            // 判断 <【输入值】是否为自身>
            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return m_Guid.Equals(other.m_Guid);
        }
        #endregion

        #endregion

        #region 内部方法
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position">位置</param>
        internal void Draw(Rect position)
        {
            // 判断 <是否已关闭>
            if (m_State == DockState.Closed)
            {
                return;
            }

            // 判断 <【窗口列表】是否为【空】>
            if (m_Windows == null || m_Windows.Count == 0)
            {
                return;
            }

            DrawGUIUtility.BeginGroup(position);

            #region 绘制【标题栏】
            // 获取【标题栏位置】
            Rect headerPosition = new Rect
            (
                0,
                0,
                position.width,
                HEADER_HEIGHT
            );

            // 绘制【标题栏】
            DrawHeader(headerPosition);
            #endregion

            // 绘制【可拖拽区域】
            DrawDragableArea(headerPosition);

            #region 绘制【选项卡窗口】
            // 获取【选项卡窗口位置】
            Rect tabWindowPosition = new Rect
            (
                0,
                HEADER_HEIGHT,
                position.width,
                position.height - HEADER_HEIGHT
            );

            // 获取当前【组深度】
            int groupDepth = DrawGUIUtility.GroupDepth;

            DrawGUIUtility.BeginGroup(tabWindowPosition, GetStyle(TAB_WINDOW_BACKGROUND_STYLE_NAME, GUI.skin.box));

            // 绘制【选项卡窗口位置】
            DrawTabWindow
            (
                new Rect
                (
                    0,
                    0,
                    tabWindowPosition.width,
                    tabWindowPosition.height
                )
            );

            DrawGUIUtility.EndGroup();

            // 判断 <结束【组】到【指定组深度】是否成功>
            if (DrawGUIUtility.EndGroupTo(groupDepth))
            {
                Debug.LogError("存在未结束的 GUI 组！");
            }
            #endregion

            DrawGUIUtility.EndGroup();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        internal void Close()
        {
            // 设置为【已关闭】
            m_State = DockState.Closed;

            // 释放【所有窗口】
            m_Windows.Clear();

            // 重置【选中选项卡索引】
            m_SelectedTabIndex = 0;

            OnClosing?.Invoke();
        }

        /// <summary>
        /// 最大化
        /// </summary>
        internal void Maximize()
        {
            OnMaximizing?.Invoke();
        }

        #region 管理【窗口】
        /// <summary>
        /// 添加【窗口】
        /// </summary>
        /// <param name="window">窗口</param>
        /// <returns>返回添加窗口是否成功的判断结果。</returns>
        internal bool AddWindow(Window window)
        {
            // 判断 <【输入窗口】是否为【空】>或<【窗口列表】是否包含【输入窗口】>
            if (window == null || m_Windows.Contains(window))
            {
                return false;
            }

            m_Windows.Add(window);

            return true;
        }

        /// <summary>
        /// 移除【窗口】
        /// </summary>
        /// <param name="window">窗口</param>
        /// <returns>返回移除窗口是否成功的判断结果。</returns>
        internal bool RemoveWindow(Window window)
        {
            // 判断 <【输入窗口】是否为【空】>或<【窗口列表】是否不包含【输入窗口】>
            if (window == null || !m_Windows.Contains(window))
            {
                return false;
            }

            // 判断 <移除【输入窗口】是否成功>
            if (m_Windows.Remove(window))
            {
                OnWindowRemoved();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除【窗口】（基于索引）
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>返回移除窗口是否成功的判断结果。</returns>
        internal bool RemoveWindowAt(int index)
        {
            // 判断 <【输入索引】是否超限>
            if (index < 0 || index >= m_Windows.Count)
            {
                return false;
            }

            m_Windows.RemoveAt(index);

            OnWindowRemoved();

            return true;
        }

        /// <summary>
        /// 移除【当前窗口】
        /// </summary>
        /// <returns>若移除成功，返回已移除的【当前窗口】；否则，返回【空】。</returns>
        internal Window RemoveCurrentWindow()
        {
            // 判断 <【选中索引】是否超限>
            if (m_SelectedTabIndex < 0 || m_SelectedTabIndex >= m_Windows.Count)
            {
                return null;
            }

            // 获取【当前窗口】
            Window currentWindow = m_Windows[m_SelectedTabIndex];

            m_Windows.RemoveAt(m_SelectedTabIndex);

            m_State = DockState.Default;

            OnWindowRemoved();

            return currentWindow;
        }
        #endregion

        #endregion

        #region 私有方法
        /// <summary>
        /// 当移除窗口后
        /// </summary>
        private void OnWindowRemoved()
        {
            m_SelectedTabIndex = Math.Clamp(m_SelectedTabIndex, 0, WindowCount);

            // 判断 <【窗口总数】是否为【0】>
            if (WindowCount == 0)
            {
                Close();
            }
        }

        #region 绘制
        /// <summary>
        /// 绘制【标题栏】
        /// </summary>
        /// <param name="position">位置</param>
        private void DrawHeader(Rect position)
        {
            DrawGUIUtility.BeginGroup(position, GetStyle(HEADER_BACKGROUND_STYLE_NAME, GUI.skin.box));

            // 获取【标题栏位置】
            Rect headerPosition = new Rect(0, 0, position.width, position.height);

            // 绘制【标题栏按钮】
            headerPosition = DrawHeaderButtons(headerPosition);

            // 绘制【拖拽选项卡滚动条】
            headerPosition = DrawDragtabScroller(headerPosition);

            // 绘制【可拖拽区域】
            // DrawDragableArea(headerPosition);

            DrawGUIUtility.EndGroup();
        }

        /// <summary>
        /// 绘制【所有标题栏按钮】
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns>返回绘制后剩余的位置。</returns>
        private Rect DrawHeaderButtons(Rect position)
        {
            #region 绘制【关闭窗口按钮】
            // 绘制【关闭窗口按钮】并判断 <按钮是否被触发>
            if (GUI.Button(GetButtonPosition(), new GUIContent("×"), GetStyle(CLOSE_BUTTON_STYLE_NAME, GUI.skin.button)))
            {
                Close();
            }
            #endregion

            #region 绘制【最大化窗口按钮】
            // 判断 <是否可变更尺寸>、<【最大化时事件】是否不为【空】>
            if (OnMaximizing != null)
            {
                // 绘制【最大化窗口按钮】并判断 <按钮是否被触发>
                if (GUI.Button(GetButtonPosition(), new GUIContent("□"), GetStyle(MAXIMIZE_BUTTON_STYLE_NAME, GUI.skin.button)))
                {
                    Maximize();
                }
            }
            #endregion

            return position;

            #region 局部方法
            // 局部方法：获取【按钮位置】
            Rect GetButtonPosition()
            {
                // 移除【即将被使用的位置】
                position.xMax -= HEADER_BUTTON_WIDTH;

                return new Rect
                (
                    position.xMax,
                    position.yMin,
                    HEADER_BUTTON_WIDTH,
                    position.height
                );
            }
            #endregion
        }

        /// <summary>
        /// 绘制【拖拽选项卡滚动条】
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns>返回绘制后剩余的位置。</returns>
        private Rect DrawDragtabScroller(Rect position)
        {
            // 判断 <【所有拖拽选项卡的总宽度】是否小于等于【位置】的【宽度】>，即<是否不绘制【滚动按钮】>
            if (WindowCount * DRAGTAB_WIDTH <= position.width)
            {
                m_HeaderScrollerPosition = 0;

                DrawDragtabs(position, position);

                return new Rect
                (
                    position.x,
                    position.y,
                    position.width - WindowCount * DRAGTAB_WIDTH,
                    position.height
                );
            }

            // 判断 <是否绘制【上一个按钮】>
            bool isDrawPrevButton = m_HeaderScrollerPosition < 0;

            // 获取【滚动条位置下限】
            float minScrollerPosition = Math.Min(0, position.width - WindowCount * DRAGTAB_WIDTH);

            // 判断 <是否绘制【下一个按钮】>
            bool isDrawNextButton = m_HeaderScrollerPosition > minScrollerPosition;

            // 获取【可交互位置】
            Rect interactivePosition = position;

            // 判断 <是否绘制【上一个按钮】>
            if (isDrawPrevButton)
            {
                interactivePosition.xMin += DRAGTAB_SCROLLER_BUTTON_WIDTH;
            }

            // 判断 <是否绘制【下一个按钮】>
            if (isDrawNextButton)
            {
                interactivePosition.xMax -= DRAGTAB_SCROLLER_BUTTON_WIDTH;
            }

            DrawDragtabs(position, interactivePosition);

            // 判断 <是否绘制【上一个按钮】>
            if (isDrawPrevButton)
            {
                // 获取【上一个按钮】的【位置】
                Rect prevButtonPosition = new Rect
                (
                    position.xMin,
                    position.yMin,
                    DRAGTAB_SCROLLER_BUTTON_WIDTH,
                    position.height
                );

                DrawDragtabScrollerButton
                (
                    prevButtonPosition,
                    new GUIContent("←"),
                    1,
                    GetStyle(PREV_BUTTON_STYLE_NAME, GUI.skin.button)
                );
            }

            // 判断 <是否绘制【下一个按钮】>
            if (isDrawNextButton)
            {
                // 获取【下一个按钮】的【位置】
                Rect nextButtonPosition = new Rect
                (
                    position.xMax - DRAGTAB_SCROLLER_BUTTON_WIDTH,
                    position.yMin,
                    DRAGTAB_SCROLLER_BUTTON_WIDTH,
                    position.height
                );

                DrawDragtabScrollerButton
                (
                    nextButtonPosition,
                    new GUIContent("→"),
                    -1,
                    GetStyle(NEXT_BUTTON_STYLE_NAME, GUI.skin.button)
                );
            }

            m_HeaderScrollerPosition = Math.Clamp(m_HeaderScrollerPosition, minScrollerPosition, 0);

            return new Rect
            (
                position.x,
                position.y,
                0,
                position.height
            );
        }

        /// <summary>
        /// 绘制【所有拖拽选项卡】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="interactivePosition">可交互位置</param>
        private void DrawDragtabs(Rect position, Rect interactivePosition)
        {
            // 获取【控件标识】
            int controlId = GUIUtility.GetControlID
            (
                m_DragtabScrollerHash,
                FocusType.Passive,
                position
            );

            // 判断 <【GUI 实用程序】的【当前热控件标识】是否等于【控件标识】>，即<【当前控件】是否拥有焦点>
            bool isHasFocus = GUIUtility.hotControl == controlId;

            // 获取【当前事件】
            Event currentEvent = Event.current;

            DrawGUIUtility.BeginGroup(position);

            // 获取【选项卡总数】
            int dragtabCount = WindowCount;

            // 获取【滚动条位置】
            Rect scrollerPosition = new Rect
            (
                m_HeaderScrollerPosition,
                position.yMin,
                WindowCount * DRAGTAB_WIDTH,
                position.height
            );

            // 获取【目标索引】
            int targetIndex;

            // 判断 <【位置】是否包含【当前事件】的【鼠标位置】>
            if (position.Contains(currentEvent.mousePosition))
            {
                targetIndex = Math.Clamp
                (
                    Mathf.FloorToInt((currentEvent.mousePosition.x - scrollerPosition.xMin) / DRAGTAB_WIDTH),
                    0,
                    dragtabCount - 1
                );
            }
            else
            {
                targetIndex = -1;
            }

            // 获取【控件标识】对应的【当前事件类型】并处理对应的事件
            switch (currentEvent.GetTypeForControl(controlId))
            {
                // 重绘
                case EventType.Repaint:
                    // 获取【拖拽选项卡位置】
                    Rect dragtabPosition = new Rect
                    (
                        scrollerPosition.xMin,
                        0,
                        DRAGTAB_WIDTH,
                        position.height
                    );

                    // 获取【拖拽选项卡样式】
                    GUIStyle dragtabStyle = GetStyle(DRAGTAB_STYLE_NAME, GUI.skin.button);

                    // 判断 <是否正在拖拽选项卡>
                    if (m_State == DockState.DraggingTab)
                    {
                        // 获取【绘制选项卡索引】
                        int drawingTabIndex = 0;

                        // 循环以绘制所有窗口对应的【拖拽选项卡】
                        for (int i = 0; i < dragtabCount; i++)
                        {
                            // 判断 <【当前索引】是否等于【目标索引】>
                            if (i == targetIndex)
                            {
                                DrawDragtab
                                (
                                    dragtabPosition,
                                    new GUIContent(m_Windows[m_SelectedTabIndex].Name),
                                    true,
                                    true,
                                    dragtabStyle
                                );

                                // 更新以获取【下一个拖拽选项卡】的【位置】
                                dragtabPosition.x += DRAGTAB_WIDTH;
                            }

                            // 判断 <是否已选中【当前选项卡】>
                            if (drawingTabIndex == m_SelectedTabIndex)
                            {
                                // 更新以获取下一个【绘制选项卡索引】
                                drawingTabIndex++;
                            }

                            // 判断 <是否未选中【当前选项卡】>、<【绘制选项卡索引】是否小于【选项卡总数】>
                            if (drawingTabIndex != m_SelectedTabIndex && drawingTabIndex < dragtabCount)
                            {
                                DrawDragtab
                                (
                                    dragtabPosition,
                                    new GUIContent(m_Windows[drawingTabIndex].Name),
                                    false,
                                    false,
                                    dragtabStyle
                                );

                                // 更新以获取【下一个拖拽选项卡】的【位置】
                                dragtabPosition.x += DRAGTAB_WIDTH;

                                // 更新以获取下一个【绘制选项卡索引】
                                drawingTabIndex++;
                            }
                        }
                    }
                    else
                    {
                        // 循环以绘制所有窗口对应的【拖拽选项卡】
                        for (int i = 0; i < dragtabCount; i++)
                        {
                            DrawDragtab
                            (
                                dragtabPosition,
                                new GUIContent(m_Windows[i].Name),
                                i == targetIndex && scrollerPosition.Contains(currentEvent.mousePosition),
                                i == m_SelectedTabIndex,
                                dragtabStyle
                            );

                            // 更新以获取【下一个拖拽选项卡】的【位置】
                            dragtabPosition.x += DRAGTAB_WIDTH;
                        }
                    }

                    break;

                // 按下鼠标
                case EventType.MouseDown:
                    // 判断 <事件对应的鼠标按键是否不为【鼠标左键】>
                    if (currentEvent.button != 0)
                    {
                        break;
                    }

                    // 判断 <【目标索引】是否有效>
                    if (targetIndex > -1)
                    {
                        // 获取【目标选项卡位置】
                        Rect targetTabPosition = new Rect
                        (
                            targetIndex * DRAGTAB_WIDTH + scrollerPosition.xMin,
                            interactivePosition.y,
                            DRAGTAB_WIDTH,
                            interactivePosition.height
                        );

                        targetTabPosition.xMin = Math.Max(targetTabPosition.xMin, interactivePosition.xMin);
                        targetTabPosition.xMax = Math.Min(targetTabPosition.xMax, interactivePosition.xMax);

                        // 判断 <【目标选项卡位置】是否不包含【当前事件】的【鼠标位置】>
                        if (!targetTabPosition.Contains(currentEvent.mousePosition))
                        {
                            break;
                        }

                        // 判断 <【选中选项卡索引】是否不等于【目标索引】>
                        if (m_SelectedTabIndex != targetIndex)
                        {
                            m_SelectedTabIndex = targetIndex;
                        }

                        m_State = DockState.DraggingTab;

                        // 使用事件
                        currentEvent.Use();

                        // 设置【GUI 实用程序】的【当前热控件标识】为【当前控件标识】
                        GUIUtility.hotControl = controlId;
                    }

                    break;

                // 抬起鼠标
                case EventType.MouseUp:
                    // 判断 <事件对应的鼠标按键是否不为【鼠标左键】>
                    if (currentEvent.button != 0)
                    {
                        break;
                    }

                    // 判断 <是否正在拖拽选项卡>、<【当前控件】是否拥有焦点>
                    if (m_State == DockState.DraggingTab && isHasFocus)
                    {
                        m_State = DockState.Default;

                        // 判断 <【目标索引】是否有效且不等于【选中选项卡索引】>
                        if (targetIndex > -1 && targetIndex != m_SelectedTabIndex)
                        {
                            // 获取【选中窗口】
                            Window selectedWindow = m_Windows[m_SelectedTabIndex];

                            // 移除【选中窗口】
                            m_Windows.RemoveAt(m_SelectedTabIndex);

                            // 基于【目标索引】插入【选中窗口】
                            m_Windows.Insert(targetIndex, selectedWindow);

                            // 更新【选中选项卡索引】
                            m_SelectedTabIndex = targetIndex;
                        }

                        // 使用事件
                        currentEvent.Use();

                        // 重置【GUI 实用程序】中的【当前热控件标识】
                        GUIUtility.hotControl = 0;
                    }

                    break;

                // 拖拽鼠标
                case EventType.MouseDrag:
                    // 判断 <是否正在拖拽选项卡>、<【当前控件】是否拥有焦点>
                    if (m_State == DockState.DraggingTab && isHasFocus)
                    {
                        // 使用事件
                        currentEvent.Use();
                    }

                    break;

                // 默认
                default:
                    break;
            }

            DrawGUIUtility.EndGroup();

            #region 局部方法
            // 静态局部方法：绘制【拖拽选项卡】
            // @position  ：位置
            // @label     ：标签
            // @isHover   ：是否悬停
            // @isSelected：是否选中
            // @style     ：样式
            static void DrawDragtab(Rect position, GUIContent label, bool isHover, bool isSelected, GUIStyle style)
            {
                style.Draw(position, label, isHover, false, isSelected, false);
            }
            #endregion
        }

        /// <summary>
        /// 绘制【拖拽选项卡滚动条按钮】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="label">标签</param>
        /// <param name="direction">方向，取值范围为{-1、1}</param>
        /// <param name="style">样式</param>
        private void DrawDragtabScrollerButton(Rect position, GUIContent label, float direction, GUIStyle style)
        {
            // 绘制【按钮】并判断 <是否触发该按钮>
            if (GUI.Button(position, label, style))
            {
                direction = direction >= 0 ? 1 : -1;

                m_HeaderScrollerPosition += direction * DRAGTAB_WIDTH;
            }
        }

        /// <summary>
        /// 绘制【可拖拽区域】
        /// </summary>
        /// <param name="position">位置</param>
        private void DrawDragableArea(Rect position)
        {
            // 判断 <是否正在拖拽标签>或<【可拖拽区域位置】的【宽度】是否小于等于【0】>，即<是否无需绘制【可拖拽区域】>
            if (m_State == DockState.DraggingTab || position.width <= 0)
            {
                return;
            }

            // 绘制【可拖拽区域】并判断 <是否未拖拽>或<是否不存在【移动距离】>，即<是否不需要变更位置>
            if (!DrawGUIUtility.DrawDragArea(position, out Vector2 distance) || distance == Vector2.zero)
            {
                return;
            }

            OnDragging?.Invoke(distance);
        }

        /// <summary>
        /// 绘制【选项卡窗口】
        /// </summary>
        /// <param name="position">位置</param>
        private void DrawTabWindow(Rect position)
        {
            // 判断 <【选中选项卡索引】是否超限>
            if (m_SelectedTabIndex < 0 || m_SelectedTabIndex >= WindowCount)
            {
                return;
            }

            // 获取【被选中选项卡窗口】
            Window selectedTabWindow = m_Windows[m_SelectedTabIndex];

            // 判断 <【被选中选项卡窗口】是否为空>
            if (selectedTabWindow == null)
            {
                return;
            }

            // 绘制【被选中选项卡窗口】
            selectedTabWindow.Draw(position);
        }
        #endregion

        #endregion

        #region 运算
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="left">运算符号左侧的值</param>
        /// <param name="right">运算符号右侧的值</param>
        /// <returns>返回【运算符号左侧的值】是否等于【运算符号右侧的值】的判断结果。</returns>
        public static bool operator ==(Dock left, Dock right)
        {
            // 判断 <【左侧值】是否为【空】>
            if (left is null)
            {
                // 判断 <【右侧值】是否为【空】>
                if (right is null)
                {
                    return true;
                }

                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="left">运算符号左侧的值</param>
        /// <param name="right">运算符号右侧的值</param>
        /// <returns>返回【运算符号左侧的值】是否不等于【运算符号右侧的值】的判断结果。</returns>
        public static bool operator !=(Dock left, Dock right)
        {
            return !(left == right);
        }
        #endregion
    }
}
