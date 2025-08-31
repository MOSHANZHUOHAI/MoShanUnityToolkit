using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 窗口
    /// </summary>
    [Serializable]
    public abstract class Window
    {
        #region 字段
        /// <summary>
        /// 名称
        /// </summary>
        private string m_Name = string.Empty;
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                string newValue;

                // 判断 <【输入值】是否为【空】>
                if (string.IsNullOrWhiteSpace(value))
                {
                    newValue = GetType().Name;
                }
                else
                {
                    // 使用正则表达式匹配【所有非空格的空白字符】替换为【空字符串】
                    newValue = Regex.Replace(value, @"[\s-[ ]]", string.Empty);

                    // 判断 <【输入值】是否为【空】>
                    if (string.IsNullOrWhiteSpace(newValue))
                    {
                        newValue = GetType().Name;
                    }
                }

                // 判断 <【名称】是否等于【输入值】>
                if (m_Name == newValue)
                {
                    return;
                }

                m_Name = newValue;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public Window()
        {
            m_Name = GetType().Name;

            OnEnter();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        public Window(string name)
        {
            Name = name;

            OnEnter();
        }
        #endregion

        #region 析构方法
        /// <summary>
        /// 析构方法
        /// </summary>
        ~Window()
        {
            OnExit();
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【内容】
        /// </summary>
        /// <param name="position">位置</param>
        public void Draw(in Rect position)
        {
            DrawGUIUtility.BeginGroup(position);

            OnDraw(new Rect(0, 0, position.width, position.height));

            DrawGUIUtility.EndGroup();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 进入时
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 退出时
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// 绘制时
        /// </summary>
        /// <param name="position">位置，初始坐标为(0, 0)</param>
        protected abstract void OnDraw(Rect position);
        #endregion
    }
}
