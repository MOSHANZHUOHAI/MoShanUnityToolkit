#if UNITY_EDITOR
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Vector2 = global::UnityEngine.Vector2;
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 编辑器窗口：纹理 UV 编辑器
    /// </summary>
    /// <remarks>
    /// 注意：该脚本需要放在 Editor 文件夹下
    /// </remarks>
    internal sealed class TextureUVEditor : EditorWindow
    {
        #region 启动
        /// <summary>
        /// 显示窗口
        /// </summary>
        [MenuItem("Tools/纹理 UV 编辑器")]
        public static void ShowWindow()
        {
            // 获取窗口
            TextureUVEditor window = EditorWindow.GetWindow<TextureUVEditor>(true, "纹理 UV 编辑器", true);

            // 设置窗口的大小
            float width  = 500f;
            float height = 800f;

            // 设置窗口的大小下限
            window.minSize = new Vector2(width, height);

            // 获取当前显示器的分辨率以设置窗口到屏幕中心
            window.position = new Rect(Screen.currentResolution.width / 2 - width / 2, Screen.currentResolution.height / 2 - height / 2, width, height);

            // 绘制窗口
            window.Show();

            // 聚焦窗口
            window.Focus();
        }
        #endregion

        #region 常量
        /// <summary>
        /// 默认 UV 偏移
        /// </summary>
        private static readonly Rect DEFAULT_UV_OFFEST = new Rect(0f, 0f, 1f, 1f);
        #endregion

        #region 字段
        /// <summary>
        /// 纹理
        /// </summary>
        private Texture m_Texture;

        /// <summary>
        /// 显示尺寸
        /// </summary>
        private Vector2 m_Size = Vector2.one;

        /// <summary>
        /// UV 偏移
        /// </summary>
        private Rect m_UvOffest = DEFAULT_UV_OFFEST;

        /// <summary>
        /// 是否同步比例
        /// </summary>
        private bool m_IsSynchronizeScale = true;

        /// <summary>
        /// 是否同步尺寸
        /// </summary>
        private bool m_IsSynchronizeSize = false;

        /// <summary>
        /// 滚动视图位置
        /// </summary>
        private Vector2 m_ScrollViewPosition = Vector2.zero;
        #endregion

        #region 属性
        /// <summary>
        /// 纹理
        /// </summary>
        private Texture Texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                // 判断 <对应字段的值是否不等于输入值>
                if (m_Texture != value)
                {
                    m_Texture = value;

                    // 重置 UV 偏移
                    m_UvOffest = DEFAULT_UV_OFFEST;
                }
            }
        }

        /// <summary>
        /// 显示尺寸
        /// </summary>
        private Vector2 Size
        {
            get
            {
                return m_Size;
            }
            set
            {
                Vector2 newValue = new Vector2(MathF.Max(value.x, 1), MathF.Max(value.y, 1));

                // 判断 <对应字段的值是否不等于输入值>
                if (m_Size != newValue)
                {
                    m_Size = newValue;
                }
            }
        }

        /// <summary>
        /// 是否同步比例
        /// </summary>
        private bool IsSynchronizeScale
        {
            get
            {
                return m_IsSynchronizeScale;
            }
            set
            {
                // 判断 <对应字段的值是否不等于输入值>
                if (m_IsSynchronizeScale != value)
                {
                    m_IsSynchronizeScale = value;

                    // 判断 <对应字段的值是否为 true>
                    if (m_IsSynchronizeScale)
                    {
                        m_IsSynchronizeSize = false;
                    }
                }
            }
        }

        /// <summary>
        /// 是否同步尺寸
        /// </summary>
        private bool IsSynchronizeSize
        {
            get
            {
                return m_IsSynchronizeSize;
            }
            set
            {
                // 判断 <对应字段的值是否不等于输入值>
                if (m_IsSynchronizeSize != value)
                {
                    m_IsSynchronizeSize = value;

                    // 判断 <对应字段的值是否为 true>
                    if (m_IsSynchronizeSize)
                    {
                        m_IsSynchronizeScale = false;
                    }
                }
            }
        }
        #endregion

        #region 生命周期方法
        private void OnGUI()
        {
            #region 绘制 标题
            float titleHeight = 26f; // 标题高度

            // 绘制 标题
            GUI.Label
            (
                new Rect(0, 0, position.width, titleHeight),
                new GUIContent("纹理 UV 编辑窗口"),
                new GUIStyle("TimeRulerBackground") { alignment = TextAnchor.MiddleCenter, fixedHeight = 0 }
            );
            #endregion

            #region 绘制 分割区域
            Rect splitAreaRect = new Rect(0f, titleHeight, position.width, position.height - titleHeight);

            // 绘制 顶部区域
            Rect topAreaRect = new Rect
            (
                splitAreaRect.x,
                splitAreaRect.y,
                splitAreaRect.width,
                splitAreaRect.height / 2
            );

            DrawSplitArea(topAreaRect, new GUIStyle("CurveEditorBackground"), DrawTopArea);

            // 绘制 底部区域
            Rect bottomAreaRect = new Rect
            (
                splitAreaRect.x,
                splitAreaRect.y + splitAreaRect.height / 2,
                splitAreaRect.width,
                splitAreaRect.height / 2
            );

            DrawSplitArea(bottomAreaRect, new GUIStyle("RL Background"), DrawBottomArea);
            #endregion
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 设置 UV 偏移
        /// </summary>
        /// <param name="value">指定需要进行设置的新 UV 偏移值。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetUVOffest(Rect value)
        {
            // 判断 是否同步缩放比例
            if (IsSynchronizeScale)
            {
                // 判断 对应字段的值的宽度是否相等输入值的宽度（即宽度是否发生变化）
                if (m_UvOffest.width != value.width)
                {
                    // 若 newRect.width 为 0，可能导致出现 NaN（除数不可为 0）
                    if (value.width != 0)
                    {
                        Vector2 newSize = Vector2.one;
                        if (m_UvOffest.width == m_UvOffest.height && m_UvOffest.width == 0)
                        {
                            float scale = 1;

                            newSize = new Vector2(value.width, value.width * scale);
                        }
                        else
                        {
                            float scale = m_UvOffest.height / MathF.Max(m_UvOffest.width, 0.00001f);

                            newSize = new Vector2(value.width, value.width * scale);
                        }

                        m_UvOffest.Set(Mathf.Clamp(value.x, 0, 1 - newSize.x), Mathf.Clamp(value.y, 0, 1 - newSize.y), Mathf.Clamp01(newSize.x), Mathf.Clamp01(newSize.y));
                    }
                    else
                    {
                        m_UvOffest.Set(0f, 0f, 0f, 0f);
                    }
                }
                // 判断 对应字段的值的高度是否相等输入值的高度（即 高度是否发生变化）
                else if (m_UvOffest.height != value.height)
                {
                    // 若 newRect.height 为 0，可能导致出现 NaN（除数不可为 0）
                    if (value.height != 0)
                    {
                        Vector2 newSize = Vector2.one;

                        // 判断 对应字段的值的宽、高度是否相等且都等于 0
                        if (m_UvOffest.width == m_UvOffest.height && m_UvOffest.width == 0)
                        {
                            float scale = 1;

                            newSize = new Vector2(value.height, value.height * scale);
                        }
                        else
                        {
                            float scale = m_UvOffest.width / MathF.Max(m_UvOffest.height, 0.00001f);

                            newSize = new Vector2(value.height * scale, value.height);
                        }

                        m_UvOffest.Set(Mathf.Clamp(value.x, 0, 1 - newSize.x), Mathf.Clamp(value.y, 0, 1 - newSize.y), Mathf.Clamp01(newSize.x), Mathf.Clamp01(newSize.y));
                    }
                    else
                    {
                        m_UvOffest.Set(0f, 0f, 0f, 0f);
                    }
                }
                else
                {
                    m_UvOffest.Set(Mathf.Clamp(value.x, 0, 1 - value.width), Mathf.Clamp(value.y, 0, 1 - value.height), Mathf.Clamp01(value.width), Mathf.Clamp01(value.height));
                }
            }
            // 判断 是否同步缩放尺寸
            else if (IsSynchronizeSize)
            {
                // 判断 对应字段的值的宽度是否相等输入值的宽度（即 宽度是否发生变化）
                if (m_UvOffest.width != value.width)
                {
                    m_UvOffest.Set(Mathf.Clamp(value.x, 0, 1 - value.width), Mathf.Clamp(value.y, 0, 1 - value.height), Mathf.Clamp01(value.width), Mathf.Clamp01(value.width));
                }
                // 判断 对应字段的值的高度是否相等输入值的高度（即 高度是否发生变化）
                else if (m_UvOffest.height != value.height)
                {
                    m_UvOffest.Set(Mathf.Clamp(value.x, 0, 1 - value.width), Mathf.Clamp(value.y, 0, 1 - value.height), Mathf.Clamp01(value.height), Mathf.Clamp01(value.height));
                }
                else
                {
                    m_UvOffest.Set(Mathf.Clamp(value.x, 0, 1 - value.width), Mathf.Clamp(value.y, 0, 1 - value.height), Mathf.Clamp01(value.width), Mathf.Clamp01(value.height));
                }
            }
            else
            {
                m_UvOffest.Set(Mathf.Clamp(value.x, 0, 1 - value.width), Mathf.Clamp(value.y, 0, 1 - value.height), Mathf.Clamp01(value.width), Mathf.Clamp01(value.height));
            }
        }

        /// <summary>
        /// 转换为实际绘制坐标
        /// </summary>
        /// <param name="center">指定需要转换的组件的中心位置。</param>
        /// <param name="size">指定需要转换的组件的大小。</param>
        /// <returns>返回值为需要绘制的组件的左上角位置。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2 TransformPosition(Vector2 center, Vector2 size)
        {
            return new Vector2(center.x - size.x / 2, center.y - size.y / 2);
        }

        #region 绘制
        /// <summary>
        /// 绘制分割区域
        /// </summary>
        /// <param name="rect">指定需要绘制的区域。</param>
        /// <param name="background">指定需要绘制的区域的背景的 GUI 样式。</param>
        /// <param name="action">指定用于绘制的区域的委托。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawSplitArea(Rect rect, GUIStyle background, Action<Rect> action)
        {
            Rect areaRect = new Rect(Vector2.zero, rect.size);

            GUI.BeginGroup(rect, background); // 开始 分组
            GUILayout.BeginArea(areaRect); // 开始 GUILayout区域
            {
                action?.Invoke(areaRect); // 调用委托
            }
            GUILayout.EndArea(); // 结束 GUILayout区域
            GUI.EndGroup(); // 结束 分组

            GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandHeight(false));
        }

        /// <summary>
        /// 绘制带线框的纹理
        /// </summary>
        /// <param name="rect">指定需要绘制的区域。</param>
        /// <param name="texture">指定需要绘制边框的纹理。</param>
        /// <param name="uvOffest">指定需要绘制的纹理的 UV 偏移</param>
        /// <param name="borderWidth">指定需要绘制的边框的宽度。</param>
        /// <param name="borderColor">指定需要绘制的边框的颜色。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawTextureWithBorder(Rect rect, Texture2D texture, Rect uvOffest, float borderWidth, Color borderColor)
        {
            GUI.DrawTextureWithTexCoords
            (
                rect,
                texture,
                uvOffest,
                true
            );

            // 获取 线框端点
            Vector3[] corners = new Vector3[]
            {
                new Vector3(rect.xMin - borderWidth / 2, rect.yMin - borderWidth / 2, 0),
                new Vector3(rect.xMax + borderWidth / 2, rect.yMin - borderWidth / 2, 0),
                new Vector3(rect.xMax + borderWidth / 2, rect.yMax + borderWidth / 2, 0),
                new Vector3(rect.xMin - borderWidth / 2, rect.yMax + borderWidth / 2, 0)
            };

            // 判断 事件系统当前标签是否为重绘
            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = borderColor;
                // 绘制 线框
                Handles.DrawAAPolyLine(borderWidth, corners[0], corners[1], corners[2], corners[3], corners[0]);
            }
        }

        /// <summary>
        /// 绘制线网格
        /// </summary>
        /// <remarks>
        /// 网格中心默认为有线经过。
        /// </remarks>
        /// <param name="rect">指定需要绘制的区域。</param>
        /// <param name="gridSize">指定需要绘制的网格的大小。</param>
        /// <param name="gridColor">指定需要绘制的网格的颜色。</param>
        /// <param name="offest">指定需要绘制的网格的中心点的偏移。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLineGrid(Rect rect, float gridSize, Color gridColor, Vector2 offest)
        {
            // 计算中心点
            Vector2 center = new Vector2(rect.x + rect.width / 2 + offest.x % gridSize, rect.y + rect.height / 2 + offest.y % gridSize);

            int countHorizontal = Mathf.CeilToInt(rect.width / gridSize / 2 + 2); // 获取 水平方向需要绘制的网格线数
            int countVertical = Mathf.CeilToInt(rect.height / gridSize / 2 + 2); // 获取 垂直方向需要绘制的网格线数

            Handles.BeginGUI(); // 开始 2D GUI绘制
            {
                Handles.color = gridColor; // 设置 绘制颜色

                // 循环以绘制水平网格线
                for (int i = -countHorizontal; i <= countHorizontal; i++)
                {
                    Handles.DrawLine
                    (
                        new Vector3(center.x + i * gridSize, rect.y, 0),
                        new Vector3(center.x + i * gridSize, rect.y + rect.height, 0)
                    );
                }

                // 循环以绘制垂直网格线
                for (int j = -countVertical; j <= countVertical; j++)
                {
                    Handles.DrawLine
                    (
                        new Vector3(rect.x, center.y + j * gridSize, 0),
                        new Vector3(rect.x + rect.width, center.y + j * gridSize, 0)
                    );
                }
            }
            Handles.EndGUI(); // 结束 2D GUI绘制
        }
        #endregion

        #endregion

        #region 回调方法
        /// <summary>
        /// 绘制顶部区域
        /// </summary>
        /// <param name="areaRect">指定需要绘制的区域。</param>
        private void DrawTopArea(Rect areaRect)
        {
            DrawLineGrid(areaRect, 40f, Color.gray, Vector2.zero); // 绘制 网格

            Vector2 center = new Vector2(areaRect.x + areaRect.width / 2, areaRect.y + areaRect.height / 2);

            // 判断 纹理是否不为空
            if (Texture != null)
            {
                #region 获取 显示尺寸比例
                Vector2 showSizeScale = Vector2.one;
                // 判断 显示尺寸的宽度是否大于高度
                if (Size.x > Size.y)
                {
                    showSizeScale.y = (float)Size.y / (float)Size.x;
                }
                // 判断 显示尺寸的高度是否大于宽度
                else if (Size.x < Size.y)
                {
                    showSizeScale.x = (float)Size.x / (float)Size.y;
                }
                #endregion

                float baseSize = 360f;

                Vector2 textureSize = baseSize * showSizeScale;

                DrawTextureWithBorder
                (
                    new Rect(TransformPosition(center, textureSize), textureSize),
                    (Texture2D)Texture,
                    m_UvOffest,
                    4f,
                    Color.red
                );
            }
            else
            {
                DrawTextureWithBorder
                (
                    new Rect(TransformPosition(center, Vector2.one * 80f), Vector2.one * 80f),
                    Texture2D.whiteTexture,
                    DEFAULT_UV_OFFEST,
                    4f,
                    Color.red
                );
            }
        }

        /// <summary>
        /// 绘制底部区域
        /// </summary>
        /// <param name="areaRect">指定需要绘制的区域。</param>
        private void DrawBottomArea(Rect areaRect)
        {
            m_ScrollViewPosition = EditorGUILayout.BeginScrollView(m_ScrollViewPosition); // 开始 滚动视图

            EditorGUILayout.Space();

            Texture = (Texture)EditorGUILayout.ObjectField(new GUIContent("纹理"), Texture, typeof(Texture), false);

            // 判断 纹理是否不为空
            if (Texture != null)
            {
                GUI.enabled = false; // 禁止 字段编辑
                EditorGUILayout.Vector2Field(new GUIContent("纹理尺寸"), new Vector2(Texture.width, Texture.height));
                GUI.enabled = true; // 启用 字段编辑

                EditorGUILayout.Space();

                Size = EditorGUILayout.Vector2IntField(new GUIContent("渲染面片尺寸（比例）", "当纹理 UV 与所需渲染到的面片 UV 相等时，纹理渲染在该尺寸比例的面片上的效果"), new Vector2Int((int)Size.x, (int)Size.y));

                #region 绘制 按钮（面片尺寸比例与 UV 偏移比例快速设置）
                // 判断 是否按下【设为纹理尺寸】按钮
                if (GUILayout.Button(new GUIContent("设为纹理尺寸")))
                {
                    Size = new Vector2(Texture.width, Texture.height);
                }
                // 判断 是否按下【依据面片比例调整 UV 偏移宽高】按钮
                if (GUILayout.Button(new GUIContent("依据面片比例调整 UV 偏移宽高")))
                {
                    Vector2 textureScale = new Vector2(Texture.width, Texture.height) / MathF.Max(Texture.width, Texture.height); // 获取 最简纹理尺寸比例

                    Vector2 sizeScale = Size / MathF.Max(Size.x, Size.y); // 获取 最简渲染面片尺寸比例

                    Vector2 uvScale = Vector2.one / new Vector2((float)textureScale.x / (float)sizeScale.x, (float)textureScale.y / (float)sizeScale.y); // 获取 UV 尺寸比例

                    uvScale /= MathF.Max(uvScale.x, uvScale.y); // 获取 最简 UV 尺寸比例

                    m_UvOffest.width = uvScale.x;
                    m_UvOffest.height = uvScale.y;
                }
                #endregion

                EditorGUILayout.Space();

                IsSynchronizeScale = EditorGUILayout.Toggle(new GUIContent("是否同步比例缩放", "是否同步缩放 UV 的宽度与高度的比例"), IsSynchronizeScale);
                IsSynchronizeSize = EditorGUILayout.Toggle(new GUIContent("是否同步宽高变化", "是否同步变化 UV 的宽度与高度"), IsSynchronizeSize);

                EditorGUILayout.Space();

                #region 归一化 UV 偏移
                EditorGUI.BeginChangeCheck(); // 开始 GUI 变更检测
                Rect newRect = EditorGUILayout.RectField(new GUIContent("UV 偏移（归一化）", "最基础的归一化 UV 坐标参数"), m_UvOffest);
                if (EditorGUI.EndChangeCheck()) // 结束 GUI 变更检测，判断 是否发生 GUI 变更
                {
                    SetUVOffest(newRect);
                }
                #endregion

                EditorGUILayout.Space();

                #region 逐像素 UV 偏移
                EditorGUILayout.LabelField(new GUIContent("UV 偏移（逐像素）", "基于纹理尺寸的逐像素 UV 坐标参数"));

                EditorGUI.BeginChangeCheck(); // 开始 GUI 变更检测
                EditorGUI.indentLevel++; // 增加 缩进等级
                float x = EditorGUILayout.Slider(new GUIContent("X 轴坐标"), m_UvOffest.x * Texture.width, 0f, (1 - m_UvOffest.width) * Texture.width) / Texture.width;
                float y = EditorGUILayout.Slider(new GUIContent("Y 轴坐标"), m_UvOffest.y * Texture.height, 0f, (1 - m_UvOffest.height) * Texture.height) / Texture.height;
                float width = EditorGUILayout.Slider(new GUIContent("宽度"), m_UvOffest.width * Texture.width, 0f, Texture.width) / Texture.width;
                float height = EditorGUILayout.Slider(new GUIContent("高度"), m_UvOffest.height * Texture.height, 0f, Texture.height) / Texture.height;
                EditorGUI.indentLevel--; // 减少 缩进等级
                if (EditorGUI.EndChangeCheck()) // 结束 GUI 变更检测，判断 是否发生 GUI 变更
                {
                    newRect = new Rect(x, y, width, height);

                    SetUVOffest(newRect);
                }
                #endregion

                EditorGUILayout.Space();

                #region 绘制 按钮
                // 判断 是否按下【打印 UV 偏移】按钮
                if (GUILayout.Button(new GUIContent("打印 UV 偏移")))
                {
                    Debug.Log($"【UV 偏移】数据：\nRect({m_UvOffest.x},{m_UvOffest.y},{m_UvOffest.width},{m_UvOffest.height})");
                }

                // 判断 是否按下【复制 UV 偏移】按钮
                if (GUILayout.Button(new GUIContent("复制 UV 偏移", "复制 UV 偏移的数据到剪贴板")))
                {
                    GUIUtility.systemCopyBuffer = $"Rect({m_UvOffest.x},{m_UvOffest.y},{m_UvOffest.width},{m_UvOffest.height})";
                }

                // 判断 是否按下【重置 UV 偏移】按钮
                if (GUILayout.Button(new GUIContent("重置 UV 偏移")))
                {
                    m_UvOffest = DEFAULT_UV_OFFEST;
                }
                #endregion
            }
            else
            {
                EditorGUILayout.BeginHorizontal();  // 开始水平布局
                {
                    GUILayout.FlexibleSpace();  // 添加可伸缩的空间

                    GUILayout.Label(new GUIContent("请导入纹理"), EditorStyles.boldLabel);// 添加文本，设置为粗体

                    GUILayout.FlexibleSpace();  // 添加可伸缩的空间
                }
                EditorGUILayout.EndHorizontal();  // 结束水平布局
            }

            EditorGUILayout.EndScrollView(); // 结束 滚动视图
        }
        #endregion
    }
}
#endif
