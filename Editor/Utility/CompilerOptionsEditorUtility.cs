#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    /// <summary>
    /// 编辑器实用程序：编译选项
    /// </summary>
    [InitializeOnLoad] // 加载 Unity 和重新编译脚本时初始化该类型
    internal static partial class CompilerOptionsEditorUtility
    {
        #region 常量
        /// <summary>
        /// 编辑器首选项键：是否锁定程序集重载
        /// </summary>
        private const string IS_LOCK_EDITOR_PREFS_KEY            = nameof(CompilerOptionsEditorUtility) + "." + nameof(IsLock);

        /// <summary>
        /// 编辑器首选项键：是否锁定运行时程序集重载
        /// </summary>
        private const string IS_LOCK_ON_PLAYING_EDITOR_PREFS_KEY = nameof(CompilerOptionsEditorUtility) + "." + nameof(IsLockOnPlaying);
        #endregion

        #region 字段
        /// <summary>
        /// 是否锁定程序集重载
        /// </summary>
        private static bool s_IsLock;

        /// <summary>
        /// 是否锁定运行时程序集重载
        /// </summary>
        private static bool s_IsLockOnPlaying;
        #endregion

        #region 属性
        /// <summary>
        /// 是否锁定程序集重载
        /// </summary>
        public static bool IsLock
        {
            get
            {
                return s_IsLock;
            }
            private set
            {
                // 判断 <【是否锁定程序集重载】是否等于【输入值】>
                if (s_IsLock == value)
                {
                    return;
                }

                s_IsLock = value;

                // 将【是否锁定程序集重载】存入【编辑器首选项】
                EditorPrefs.SetBool(IS_LOCK_EDITOR_PREFS_KEY, s_IsLock);

                // 判断 <是否锁定程序集重载>
                if (s_IsLock)
                {
                    // 锁定程序集重载
                    EditorApplication.LockReloadAssemblies();

                    Debug.Log("程序集重载已锁定");
                }
                else
                {
                    // 判断 <是否非运行时>或<是否运行时不锁定程序集重载>
                    if (!EditorApplication.isPlaying || (EditorApplication.isPlaying && !s_IsLockOnPlaying))
                    {
                        // 解锁程序集重载
                        EditorApplication.UnlockReloadAssemblies();
                    }

                    Debug.Log("程序集重载已解锁");
                }
            }
        }

        /// <summary>
        /// 是否锁定运行时程序集重载
        /// </summary>
        public static bool IsLockOnPlaying
        {
            get
            {
                return s_IsLockOnPlaying;
            }
            private set
            {
                // 判断 <【是否锁定运行时程序集重载】是否等于[输入值]>
                if (s_IsLockOnPlaying == value)
                {
                    return;
                }

                s_IsLockOnPlaying = value;

                // 将【是否锁定运行时程序集重载】存入【编辑器首选项】
                EditorPrefs.SetBool(IS_LOCK_ON_PLAYING_EDITOR_PREFS_KEY, s_IsLockOnPlaying);

                // 判断 <是否锁定运行时程序集重载>
                if (s_IsLockOnPlaying)
                {
                    // 订阅编辑器运行模式状态变更
                    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

                    Debug.Log("已锁定运行时程序集重载");

                    // 判断 <是否运行时>
                    if (EditorApplication.isPlaying)
                    {
                        // 锁定程序集重载
                        EditorApplication.LockReloadAssemblies();
                    }
                }
                else
                {
                    // 取消订阅编辑器运行模式状态变更
                    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

                    Debug.Log("已解锁运行时程序集重载");

                    // 判断 <是否运行时不锁定程序集重载>
                    if (EditorApplication.isPlaying && !s_IsLock)
                    {
                        // 解锁程序集重载
                        EditorApplication.UnlockReloadAssemblies();
                    }
                }
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static CompilerOptionsEditorUtility()
        {
            // 从【编辑器首选项】中读取【是否锁定程序集重载】
            s_IsLock          = EditorPrefs.GetBool(IS_LOCK_EDITOR_PREFS_KEY           , false);

            // 从【编辑器首选项】中读取【是否锁定运行时程序集重载】
            s_IsLockOnPlaying = EditorPrefs.GetBool(IS_LOCK_ON_PLAYING_EDITOR_PREFS_KEY, false);

            // 判断 <是否锁定程序集重载>
            if (s_IsLock)
            {
                // 锁定程序集重载
                EditorApplication.LockReloadAssemblies();
            }
            else
            {
                // 判断 <是否非运行时>或<运行时是否不锁定运行时程序集重载>
                if (!EditorApplication.isPlaying || (EditorApplication.isPlaying && !s_IsLockOnPlaying))
                {
                    // 解锁程序集重载
                    EditorApplication.UnlockReloadAssemblies();
                }
            }

            // 判断 <是否锁定程序集重载>
            if (s_IsLockOnPlaying)
            {
                // 订阅编辑器运行模式状态变更
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

                // 判断 <是否运行时>
                if (EditorApplication.isPlaying)
                {
                    // 锁定程序集重载
                    EditorApplication.LockReloadAssemblies();
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 创建【设置提供器】
        /// </summary>
        /// <returns>返回添加条目到【首选项窗口】或【项目设置窗口】所需的设置提供器。</returns>
        [SettingsProvider] // 标记该方法可创建【设置提供器】以添加条目到【首选项窗口】或【项目设置窗口】，修饰方法必须为静态类型下的静态方法
        private static SettingsProvider CreateSettingsProvider()
        {
            return new CompilerOptionsSettingsProvider();
        }

        /// <summary>
        /// 编辑器运行模式状态变更时
        /// </summary>
        /// <param name="playModeState">当前编辑器运行模式状态变更</param>
        private static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            // 判断 <是否锁定程序集重载>
            if (s_IsLock)
            {
                // 锁定程序集重载
                EditorApplication.LockReloadAssemblies();

                return;
            }

            switch (playModeState)
            {
                // 退出【编辑】模式时，进入【运行】模式前
                case PlayModeStateChange.ExitingEditMode:
                    break;
                // 退出【运行】模式时，进入【编辑】模式前
                case PlayModeStateChange.ExitingPlayMode:
                    break;

                // 进入【编辑】模式后，下次更新时调用
                case PlayModeStateChange.EnteredEditMode:
                    // 解锁程序集重载
                    EditorApplication.UnlockReloadAssemblies();
                    break;

                // 进入【运行】模式后，下次更新时调用
                case PlayModeStateChange.EnteredPlayMode:
                    // 判断 <是否锁定运行时程序集重载>
                    if (s_IsLockOnPlaying)
                    {
                        // 锁定程序集重载
                        EditorApplication.LockReloadAssemblies();
                    }
                    break;
            }
        }

        #region 菜单选项
        /// <summary>
        /// 锁定程序集重载
        /// </summary>
        [MenuItem("Tools/编译器选项/锁定程序集重载 _F1", false, 1)]
        private static void LockReloadAssemblies()
        {
            IsLock = true;
        }

        /// <summary>
        /// 解锁程序集重载
        /// </summary>
        [MenuItem("Tools/编译器选项/解锁程序集重载 _F2", false, 2)]
        private static void UnlockReloadAssemblies()
        {
            IsLock = false;
        }

        /// <summary>
        /// 锁定运行时程序集重载
        /// </summary>
        [MenuItem("Tools/编译器选项/锁定运行时程序集重载", false, 21)]
        private static void LockPlayingReloadAssemblies()
        {
            IsLockOnPlaying = true;
        }

        /// <summary>
        /// 解锁运行时程序集重载
        /// </summary>
        [MenuItem("Tools/编译器选项/解锁运行时程序集重载", false, 22)]
        private static void UnlockPlayingReloadAssemblies()
        {
            IsLockOnPlaying = false;
        }
        #endregion

        #region 菜单选项验证
        /// <summary>
        /// 验证方法：【<see cref="LockReloadAssemblies">锁定程序集重载</see>】
        /// </summary>
        /// <returns>返回<see cref="LockReloadAssemblies">对应方法</see>是否可显示在菜单中的判断结果。</returns>
        [MenuItem("Tools/编译器选项/锁定程序集重载 _F1", true, 1)]
        private static bool LockReloadAssembliesValidate()
        {
            return !s_IsLock;
        }

        /// <summary>
        /// 验证方法：【<see cref="UnlockReloadAssemblies">解锁程序集重载</see>】
        /// </summary>
        /// <returns>返回<see cref="UnlockReloadAssemblies">对应方法</see>是否可显示在菜单中的判断结果。</returns>
        [MenuItem("Tools/编译器选项/解锁程序集重载 _F2", true, 2)]
        private static bool UnlockReloadAssembliesValidate()
        {
            return s_IsLock;
        }

        /// <summary>
        /// 验证方法：【<see cref="LockPlayingReloadAssemblies">锁定运行时程序集重载</see>】
        /// </summary>
        /// <returns>返回<see cref="LockPlayingReloadAssemblies">对应方法</see>是否可显示在菜单中的判断结果。</returns>
        [MenuItem("Tools/编译器选项/锁定运行时程序集重载", true, 21)]
        private static bool LockPlayingReloadAssembliesValidate()
        {
            return !s_IsLockOnPlaying;
        }

        /// <summary>
        /// 验证方法：【<see cref="UnlockPlayingReloadAssemblies">解锁运行时程序集重载</see>】
        /// </summary>
        /// <returns>返回<see cref="UnlockPlayingReloadAssemblies">对应方法</see>是否可显示在菜单中的判断结果。</returns>
        [MenuItem("Tools/编译器选项/解锁运行时程序集重载", true, 22)]
        private static bool UnlockPlayingReloadAssembliesValidate()
        {
            return s_IsLockOnPlaying;
        }
        #endregion

        #endregion
    }
}
#endif
