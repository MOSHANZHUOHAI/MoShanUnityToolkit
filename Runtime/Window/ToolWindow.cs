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
                DrawGUIUtility.BeginGroup(position);

                OnDrawToolbar(new Rect(0, 0, position.width, position.height));

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
