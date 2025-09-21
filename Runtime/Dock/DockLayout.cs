using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect    = global::UnityEngine.Rect;
    using Vector2 = global::UnityEngine.Vector2;

    /// <summary>
    /// 停靠布局
    /// </summary>
    [Serializable]
    internal sealed partial class DockLayout
    {
        #region 常量
        /// <summary>
        /// 宽度下限
        /// </summary>
        /// <remarks>
        /// 取值范围为[1, +∞)
        /// </remarks>
        private const int MIN_WIDTH = 120;

        /// <summary>
        /// 高度下限
        /// </summary>
        /// <remarks>
        /// 取值范围为[1, +∞)
        /// </remarks>
        private const int MIN_HEIGHT = 120;

        /// <summary>
        /// 尺寸调整手柄尺寸
        /// </summary>
        /// <remarks>
        /// 取值范围为[1, +∞)
        /// </remarks>
        private const int RESIZER_HANDLE_SIZE = 4;

        /// <summary>
        /// 尺寸调整线半宽度
        /// </summary>
        private const int RESIZE_LINE_HALF_WIDTH = RESIZER_HANDLE_SIZE;

        /// <summary>
        /// 尺寸调整线宽度
        /// </summary>
        private const int RESIZE_LINE_WIDTH = 2 * RESIZE_LINE_HALF_WIDTH;

        /// <summary>
        /// 吸附范围比例
        /// </summary>
        /// <remarks>
        /// 取值范围为[0, 0.5]
        /// </remarks>
        private const float ADSORPTION_RANGE_RATIO = 0.2f;
        #endregion

        #region 静态字段
        /// <summary>
        /// 尺寸下限
        /// </summary>
        private static readonly Vector2 s_MinSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);

        /// <summary>
        /// 尺寸调整手柄尺寸
        /// </summary>
        private static readonly Vector2 s_ResizeHandleSize = new Vector2(RESIZER_HANDLE_SIZE, RESIZER_HANDLE_SIZE);
        #endregion

        #region 静态内部方法
        /// <summary>
        /// 创建【固定停靠布局】
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns>若创建成功，返回新建的【固定停靠布局】实例；否则，返回【空】。</returns>
        internal static DockLayout CreateFixedLayout(Rect position)
        {
            return new DockLayout(position, position, DockLayoutType.Fixed, null);
        }

        /// <summary>
        /// 创建【浮动停靠布局】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="border">边界</param>
        /// <param name="windows">窗口</param>
        /// <returns>若创建成功，返回新建的【浮动停靠布局】实例；否则，返回【空】。</returns>
        internal static DockLayout CreateFloatingLayout(Rect position, Rect border, Window windows)
        {
            // 创建【停靠】
            Dock dock = Dock.CreateDock(windows);

            // 判断 <【停靠】是否为【空】>
            if (dock == null)
            {
                return null;
            }

            return new DockLayout(position, border, DockLayoutType.Floating, dock);
        }

        /// <summary>
        /// 创建【浮动停靠布局】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="border">边界</param>
        /// <param name="windows">所有窗口</param>
        /// <returns>若创建成功，返回新建的【浮动停靠布局】实例；否则，返回【空】。</returns>
        internal static DockLayout CreateFloatingLayout(Rect position, Rect border, params Window[] windows)
        {
            // 创建【停靠】
            Dock dock = Dock.CreateDock(windows);

            // 判断 <【停靠】是否为【空】>
            if (dock == null)
            {
                return null;
            }

            return new DockLayout(position, border, DockLayoutType.Floating, dock);
        }
        #endregion

        #region 字段
        /// <summary>
        /// 位置
        /// </summary>
        /// <remarks>
        /// 相对父级的局部位置
        /// </remarks>
        private Rect m_Position = Rect.zero;

        /// <summary>
        /// 边界
        /// </summary>
        private Rect m_Border = Rect.zero;

        /// <summary>
        /// 布局类型
        /// </summary>
        private DockLayoutType m_LayoutType = DockLayoutType.Fixed;

        /// <summary>
        /// 布局模式
        /// </summary>
        private Direction2 m_LayoutMode = Direction2.Horizontal;

        /// <summary>
        /// 内容
        /// </summary>
        private Dock m_Content;

        /// <summary>
        /// 父级
        /// </summary>
        private DockLayout m_Parent;

        /// <summary>
        /// 列表：子级
        /// </summary>
        private readonly List<DockLayout> m_Children = new List<DockLayout>();
        #endregion

        #region 属性
        /// <summary>
        /// 位置
        /// </summary>
        /// <remarks>
        /// 相对父级的局部位置
        /// </remarks>
        public Rect Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                Rect newValue = value.ToPositive();

                // 获取【最小尺寸】
                Vector2 minSize = MinSize;

                newValue = new Rect
                (
                    newValue.x > 0 ? newValue.x : 0,
                    newValue.y > 0 ? newValue.y : 0,
                    newValue.width  > minSize.x ? newValue.width  : minSize.x,
                    newValue.height > minSize.y ? newValue.height : minSize.y
                );

                // 判断 <【父级】是否不为【空】>
                if (m_Parent != null)
                {
                    newValue.width  = Mathf.Clamp(newValue.width , minSize.x, m_Parent.Position.width );
                    newValue.height = Mathf.Clamp(newValue.height, minSize.y, m_Parent.Position.height);
                }

                // 判断 <【位置】是否等于【输入值】>
                if (m_Position == newValue)
                {
                    return;
                }

                m_Position = newValue;

                AverageChildrenSize();
            }
        }

        /// <summary>
        /// 边界
        /// </summary>
        internal Rect Border
        {
            get
            {
                return m_Border;
            }
            set
            {
                Rect newValue = value.ToPositive();

                // 判断 <【边界】是否等于【输入值】>
                if (m_Border == newValue)
                {
                    return;
                }

                m_Border = newValue;
            }
        }

        /// <summary>
        /// 尺寸下限
        /// </summary>
        private Vector2 MinSize
        {
            get
            {
                // 判断 <是否不存在子级>
                if (!IsHasChildren)
                {
                    return s_MinSize;
                }

                // 获取【尺寸下限】
                Vector2 minSize = s_MinSize;

                // 获取【总和分量索引】
                // 0 = 向量的 X 轴索引，用于获取【所有子级布局】的【宽度下限总和】
                // 1 = 向量的 Y 轴索引，用于获取【所有子级布局】的【高度下限总和】
                int sumComponentIndex = m_LayoutMode == Direction2.Horizontal ? 0 : 1;

                // 获取【最大值分量索引】
                // 0 = 向量的 X 轴索引，用于获取【所有子级布局】的【最大宽度下限】
                // 1 = 向量的 Y 轴索引，用于获取【所有子级布局】的【最大高度下限】
                int maxComponentIndex = m_LayoutMode == Direction2.Horizontal ? 1 : 0;

                // 循环以累计所有子级布局的尺寸下限
                for (int i = 0; i < m_Children.Count; i++)
                {
                    // 获取【子级布局的尺寸下限】
                    Vector2 childMinSize = m_Children[i].MinSize;

                    // 累加【当前子级布局】的【宽度下限】
                    minSize[sumComponentIndex] = minSize[sumComponentIndex] + childMinSize[sumComponentIndex];

                    // 获取【所有子级布局】的【最大高度下限】
                    minSize[maxComponentIndex] = Math.Max(minSize[maxComponentIndex], childMinSize[maxComponentIndex]);
                }

                return minSize;
            }
        }

        /// <summary>
        /// 内容
        /// </summary>
        internal Dock Content
        {
            get
            {
                return m_Content;
            }
            private set
            {
                // 判断 <【内容】是否等于【输入值】>
                if (m_Content == value)
                {
                    return;
                }

                // 判断 <【内容】是否不为【空】>
                if (m_Content != null)
                {
                    m_Content.OnClosing  -= OnClosingContent;
                    m_Content.OnDragging -= OnDraggingContent;
                }

                m_Content = value;

                // 判断 <【内容】是否不为【空】>
                if (m_Content != null)
                {
                    m_Content.OnClosing  += OnClosingContent;
                    m_Content.OnDragging += OnDraggingContent;
                }
            }
        }

        /// <summary>
        /// 深度
        /// </summary>
        internal int Depth
        {
            get
            {
                // 判断 <【父级】是否为【空】>
                if (m_Parent == null)
                {
                    return 0;
                }

                return m_Parent.Depth + 1;
            }
        }

        /// <summary>
        /// 子级总数
        /// </summary>
        internal int ChildCount
        {
            get
            {
                // 判断 <【子级布局列表】是否为【空】>
                if (m_Children == null)
                {
                    return 0;
                }

                return m_Children.Count;
            }
        }

        /// <summary>
        /// 根布局位置
        /// </summary>
        /// <remarks>
        /// 获取自身在根布局中的位置
        /// </remarks>
        internal Rect RootPosition
        {
            get
            {
                // 判断 <是否为【根布局】>
                if (m_Parent == null)
                {
                    return m_Position;
                }

                // 获取【父级】
                DockLayout parent = m_Parent;

                // 获取【根布局位置】
                Rect rootPosition = m_Position;

                // While 循环以累加自身相对父级布局的位置偏移
                while (parent != null)
                {
                    rootPosition.position += parent.m_Position.position;

                    parent = parent.m_Parent;
                }

                return rootPosition;
            }
        }

        /// <summary>
        /// 根布局
        /// </summary>
        internal DockLayout Root
        {
            get
            {
                // 判断 <是否为【根布局】>
                if (IsRoot)
                {
                    return this;
                }

                return m_Parent.Root;
            }
        }

        /// <summary>
        /// 是否已关闭
        /// </summary>
        internal bool IsClosed
        {
            get
            {
                return !IsHasContent && !IsHasChildren;
            }
        }

        /// <summary>
        /// 是否为根布局
        /// </summary>
        private bool IsRoot
        {
            get
            {
                return m_Parent == null;
            }
        }

        /// <summary>
        /// 是否存在内容
        /// </summary>
        private bool IsHasContent
        {
            get
            {
                return m_Content != null && m_Content.State != DockState.Closed;
            }
        }

        /// <summary>
        /// 是否存在子级
        /// </summary>
        private bool IsHasChildren
        {
            get
            {
                // 判断 <【子级列表】是否为【空】>
                if (ChildCount == 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 是否无效边界
        /// </summary>
        private bool IsInvalidBorder
        {
            get
            {
                return m_Border.width <= 0 || m_Border.height <= 0;
            }
        }

        /// <summary>
        /// 是否可调整尺寸
        /// </summary>
        private bool IsResizable
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region 委托
        /// <summary>
        /// 关闭时
        /// </summary>
        private event Action<DockLayout> OnClosing;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="border">边界</param>
        /// <param name="type">布局类型</param>
        /// <param name="dock">停靠</param>
        private DockLayout(Rect position, Rect border, DockLayoutType type, Dock dock)
        {
            // 判断 <【输入布局类型】是否为【固定】>
            if (type == DockLayoutType.Fixed)
            {
                m_LayoutType = DockLayoutType.Fixed;
                Border       = position;
                Position     = position;
            }
            else
            {
                m_LayoutType = DockLayoutType.Floating;
                Border       = border;
                Position     = position;
            }

            Content = dock;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="parent">父级</param>
        /// <param name="dock">停靠</param>
        private DockLayout(Rect position, DockLayout parent, Dock dock)
        {
            Position = position;
            m_Parent = parent;
            Content  = dock;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 绘制
        /// </summary>
        internal void Draw()
        {
            DrawResizeBorder();

            DrawGUIUtility.BeginGroup(m_Position);

            // 判断 <是否存在子级>
            if (IsHasChildren)
            {
                // 循环以绘制所有子级
                for (int i = 0; i < m_Children.Count; i++)
                {
                    // 判断 <【当前子级】是否为【空】>
                    if (m_Children[i] == null)
                    {
                        break;
                    }

                    m_Children[i].Draw();
                }

                DrawResizeLine();
            }
            else
            {
                // 判断 <是否存在内容>
                if (IsHasContent)
                {
                    m_Content.Draw(new Rect(Vector2.zero, m_Position.size));
                }
            }

            DrawGUIUtility.EndGroup();
        }

        /// <summary>
        /// 获取【聚焦布局】
        /// </summary>
        /// <param name="mousePosition">鼠标位置</param>
        /// <param name="focusLayout">聚焦布局</param>
        /// <returns>返回获取布局是否成功的判断结果。</returns>
        internal bool GetFocusingLayout(Vector2 mousePosition, out DockLayout focusLayout)
        {
            // 获取【根布局】
            DockLayout root = Root;

            // 判断 <【根布局】是否不包含【点】>
            if (!Root.Contains(mousePosition))
            {
                focusLayout = null;

                return false;
            }

            focusLayout = root;

            // While 循环以获取聚焦布局
            while (focusLayout.ChildCount > 0)
            {
                // 遍历循环以获取子级中的聚焦布局
                foreach (DockLayout dockLayout in focusLayout.m_Children)
                {
                    // 判断 <【当前布局】的【根布局位置】是否包含【输入点】>
                    if (!dockLayout.RootPosition.Contains(mousePosition))
                    {
                        continue;
                    }

                    focusLayout = dockLayout;
                }
            }

            return true;
        }

        /// <summary>
        /// 包含【点】
        /// </summary>
        /// <param name="point">点</param>
        /// <returns>返回该布局范围内是否包含【输入点】的判断结果。</returns>
        internal bool Contains(Vector2 point)
        {
            return m_Position.Contains(point);
        }

        /// <summary>
        /// 包含【停靠】
        /// </summary>
        /// <param name="dock">停靠</param>
        /// <returns>返回该布局及子级布局是否包含【输入停靠】的判断结果。</returns>
        internal bool ContainsWithChildren(Dock dock)
        {
            // 判断 <【输入停靠】是否为【空】>
            if (dock == null)
            {
                return false;
            }

            // 判断 <【内容】是否为【输入停靠】>，即<【当前停靠布局】是否包含【输入停靠】>
            if (m_Content == dock)
            {
                return true;
            }

            return m_Children.Any(item => item.ContainsWithChildren(dock));
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 平均【子级尺寸】
        /// </summary>
        private void AverageChildrenSize()
        {
            // 判断 <是否不存在子级>
            if (!IsHasChildren)
            {
                return;
            }

            // 判断 <【布局模式】是否为【水平】>
            if (m_LayoutMode == Direction2.Horizontal)
            {
                // 获取【平均宽度】
                float averageWidth = m_Position.width / m_Children.Count;

                // 循环以均分子级布局的空间
                for (int i = 0; i < m_Children.Count; i++)
                {
                    m_Children[i].Position = new Rect(averageWidth * i, 0, averageWidth, m_Position.height);
                }
            }
            else
            {
                // 获取【平均高度】
                float averageHeight = m_Position.height / m_Children.Count;

                // 循环以均分子级布局的空间
                for (int i = 0; i < m_Children.Count; i++)
                {
                    m_Children[i].Position = new Rect(0, averageHeight * i, m_Position.width, averageHeight);
                }
            }
        }

        /// <summary>
        /// 关闭内容时
        /// </summary>
        private void OnClosingContent()
        {
            Content = null;

            OnClosing?.Invoke(this);
        }

        /// <summary>
        /// 拖拽内容时
        /// </summary>
        /// <param name="distance">距离</param>
        private void OnDraggingContent(Vector2 distance)
        {
            // 判断 <【布局类型】是否为【固定】>，即<是否不允许从内部变更位置>
            if (m_LayoutType == DockLayoutType.Fixed)
            {
                return;
            }

            m_Position.position += distance;

            // 判断 <【边界】是否无效>
            if (IsInvalidBorder)
            {
                return;
            }

            m_Position = m_Position.LimitPosition(m_Border);
        }

        #region 绘制
        /// <summary>
        /// 绘制【尺寸调整边框】
        /// </summary>
        private void DrawResizeBorder()
        {
            // 判断 <是否不为【根布局】>或<【布局类型】是否不为【浮动】>
            if (!IsRoot || m_LayoutType != DockLayoutType.Floating)
            {
                return;
            }

            m_Position = DrawGUIResizeBorderUtility.DrawResizeBorder
            (
                m_Position,
                s_ResizeHandleSize,
                MinSize,
                Vector2.positiveInfinity,
                m_Border
            );
        }

        /// <summary>
        /// 绘制【尺寸调整线】
        /// </summary>
        private void DrawResizeLine()
        {
            // 判断 <是否不存在子级>，即<是否不需要绘制【尺寸调整线】>
            if (!IsHasChildren)
            {
                return;
            }

            // 循环以绘制【尺寸调整线】
            for (int i = 1; i < m_Children.Count; i++)
            {
                // 获取【当前布局】
                DockLayout currentLayout = m_Children[i];

                // 获取【尺寸调整线位置】
                Rect linePosition = new Rect
                (
                    currentLayout.Position.x,
                    currentLayout.Position.y,
                    RESIZE_LINE_WIDTH,
                    RESIZE_LINE_WIDTH
                );

                // 判断 <深度是否为【1】>，即<【当前布局】的【父级】是否为【根布局】>
                bool isHasRootParent = currentLayout.Depth == 1;

                // 判断 <【布局模式】是否为【水平】>
                if (m_LayoutMode == Direction2.Horizontal)
                {
                    // 判断 <【父级】是否为【根布局】>
                    if (isHasRootParent)
                    {
                        linePosition.x -= RESIZE_LINE_HALF_WIDTH;
                        linePosition.height = Position.height;
                    }
                    else
                    {
                        linePosition.x -= RESIZE_LINE_HALF_WIDTH;
                        linePosition.y += RESIZE_LINE_HALF_WIDTH;
                        linePosition.height = Position.height - RESIZE_LINE_WIDTH;
                    }
                }
                else
                {
                    // 判断 <【父级】是否为【根布局】>
                    if (isHasRootParent)
                    {
                        linePosition.y -= RESIZE_LINE_HALF_WIDTH;
                        linePosition.width = Position.width;
                    }
                    else
                    {
                        linePosition.x += RESIZE_LINE_HALF_WIDTH;
                        linePosition.y -= RESIZE_LINE_HALF_WIDTH;
                        linePosition.width = Position.width - RESIZE_LINE_WIDTH;
                    }
                }

                // 绘制【尺寸调整线】以获取【移动距离】
                DrawGUIUtility.DrawDragLine(linePosition, m_LayoutMode, out Vector2 distance);

                // 判断 <是否不存在【移动距离】>，即<是否不需要变更布局>
                if (distance == Vector2.zero)
                {
                    continue;
                }

                // 获取【上一个布局】
                DockLayout lastLayout = m_Children[i - 1];

                // 获取【最小尺寸】
                Vector2 currentMinSize = currentLayout.MinSize;
                Vector2 lastMinSize = lastLayout.MinSize;

                Vector2 newSize;

                // 判断 <【布局模式】是否为【水平】>
                if (m_LayoutMode == Direction2.Horizontal)
                {
                    newSize = new Vector2
                    (
                        Math.Clamp
                        (
                            currentLayout.Position.width - distance.x,
                            currentMinSize.x,
                            Math.Max(currentLayout.Position.xMax - lastLayout.Position.x - lastMinSize.x, currentMinSize.x)
                        ),
                        currentLayout.Position.height
                    );

                    currentLayout.Position = new Rect
                    (
                        currentLayout.Position.xMax - newSize.x,
                        0,
                        newSize.x,
                        newSize.y
                    );

                    lastLayout.Position = new Rect
                    (
                        lastLayout.Position.x,
                        0,
                        currentLayout.Position.x - lastLayout.Position.x,
                        lastLayout.Position.height
                    );
                }
                else
                {
                    newSize = new Vector2
                    (
                        currentLayout.Position.width,
                        Math.Clamp
                        (
                            currentLayout.Position.height - distance.y,
                            currentMinSize.y,
                            Math.Max(currentLayout.Position.yMax - lastLayout.Position.y - lastMinSize.y, currentMinSize.y)
                        )
                    );

                    currentLayout.Position = new Rect
                    (
                        0,
                        currentLayout.Position.yMax - newSize.y,
                        newSize.x,
                        newSize.y
                    );

                    lastLayout.Position = new Rect
                    (
                        0,
                        lastLayout.Position.y,
                        lastLayout.Position.width,
                        currentLayout.Position.y - lastLayout.Position.y
                    );
                }
            }
        }
        #endregion

        #endregion
    }
}
