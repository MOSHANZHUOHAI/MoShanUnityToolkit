using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    /// <summary>
    /// 运行时工具入口
    /// </summary>
    [AddComponentMenu("MoShan/Toolkit/运行时工具入口")] // 添加到组件菜单
    [DisallowMultipleComponent] // 禁止同一对象上重复添加同类组件
    public sealed class RuntimeToolkitEntry : MonoBehaviour
    {
        #region 静态字段
        /// <summary>
        /// 单例实例
        /// </summary>
        private static RuntimeToolkitEntry s_Instance;
        #endregion

        #region 静态属性
        /// <summary>
        /// 单例实例
        /// </summary>
        public static RuntimeToolkitEntry Instance
        {
            get
            {
                // 判断 <【对应字段的值】是否为【空】>
                if (s_Instance == null)
                {
                    // 全场景搜索该单例类型的实例
                    s_Instance = FindObjectOfType<RuntimeToolkitEntry>();

                    // 判断 <【对应字段的值】是否为【空】>
                    if (s_Instance == null)
                    {
                        // 创建承载该单例类型的实例的游戏对象
                        GameObject newGameObject = new GameObject();

                        // 设置对象名称为该单例类型名称
                        newGameObject.name = $"@{typeof(RuntimeToolkitEntry).Name}";

                        /*
                         * 继承了【MonoBehaviour】的类型，无法直接通过构造方法创建该类型为实例。
                         * 需要通过拖动脚本到对象上，或通过 Unity 内部的 API：【GameObeject.AddComponent<T>()】，由 Unity 内部创建并添加该类型的实例到指定的游戏对象上。
                         */

                        // 创建并添加该类型的实例到指定的游戏对象上
                        s_Instance = newGameObject.AddComponent<RuntimeToolkitEntry>();
                    }

                    // 判断 <游戏是否正在运行>
                    if (global::UnityEngine.Application.isPlaying)
                    {
                        // 判断 <该实例所挂载的游戏对象是否不存在【父级游戏对象】>
                        if (s_Instance.transform.parent == null)
                        {
                            /*
                             * 单例模式实例一般存在于整个程序的生命周期中，所以该实例所挂载的游戏对象不应存在父级游戏对象。
                             */

                            // 设置该单例对象为【切换场景时不移除】
                            DontDestroyOnLoad(s_Instance);
                        }
                    }
                }

                return s_Instance;
            }
        }
        #endregion

        #region 静态公开方法
        /// <summary>
        /// 获取【单例实例】
        /// </summary>
        /// <returns>若获取成功，返回该类型对应的【单例实例】；否则，返回【空】。</returns>
        public static RuntimeToolkitEntry GetInstance()
        {
            return Instance;
        }
        #endregion

        #region 生命周期方法
        private void Awake()
        {
            // 判断 <【单例实例】是否为【空】>
            if (s_Instance == null)
            {
                s_Instance = this;
            }

            // 判断 <【单例实例】是否为【当前实例】>
            if (s_Instance == this)
            {
                // 设置该单例对象为【切换场景时不移除】
                DontDestroyOnLoad(s_Instance);

                return;
            }

            // 销毁【当前实例】，以确保只存在一个实例作为单例实例
            Destroy(this);
        }

        private void OnDrawGizmos()
        {

        }

        private void OnGUI()
        {
            try
            {
                RuntimeDockUtility.Draw();
            }
            // 捕获异常：其它
            catch (Exception otherException)
            {
                Debug.LogException(otherException);
            }
        }

        private void OnDestroy()
        {
            // 判断 <【单例实例】是否为【该实例】>
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        #endregion

    }
}
