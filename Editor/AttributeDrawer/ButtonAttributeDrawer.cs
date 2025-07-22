#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Rect = global::UnityEngine.Rect;

    /// <summary>
    /// 检视窗口编辑器：按钮特性
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件需要放置在【Editor】文件夹下；</br>
    /// <br>该类型需要配合【<see cref="ButtonAttribute">按钮特性</see>】类型使用，且对应类型所在的脚本文件应放置在非【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    [CanEditMultipleObjects] // 可编辑多个对象
    [CustomEditor(typeof(MonoBehaviour), true)] // 设置自定义检视窗口编辑器对应的类型与是否子类可继承
    internal sealed class ButtonAttributeDrawer : Editor
    {
        #region 生命周期方法
        /// <summary>
        /// 绘制检视窗口 GUI 时
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 绘制【原有检视窗口的 GUI 内容】
            base.OnInspectorGUI();

            // 获取【目标】
            object target;

            // 判断 <【当前实例】的【类型】是否为【MonoBehaviour】的子级类型>
            if (base.target.GetType().IsSubclassOf(typeof(MonoBehaviour)))
            {
                target = base.target as MonoBehaviour;
            }
            // 判断 <【当前实例】的【类型】是否为【ScriptableObject】的子级类型>
            else if (base.target.GetType().IsSubclassOf(typeof(ScriptableObject)))
            {
                target = base.target as ScriptableObject;
            }
            else
            {
                return;
            }

            // 判断 <【当前实例】是否为【空】>
            if (target == null)
            {
                return;
            }

            // 获取【类型中所有被【ButtonAttribute】标记的方法信息】
            MethodInfo[] methodInfos = target
                // 获取【目标类型】
                .GetType()
                // 获取【目标类型】中的【公开、私有、实例、静态的方法】
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                // 获取【目标类型】中的【被【ButtonAttribute】标记的方法】
                .Where(method => Attribute.IsDefined(method, typeof(ButtonAttribute)))
                // 转换为数组
                .ToArray();

            // 遍历循环以使用获取到的所有方法信息绘制按钮
            foreach (MethodInfo methodInfo in methodInfos)
            {
                DrawButton(methodInfo.GetCustomAttribute<ButtonAttribute>().Name, methodInfo);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 绘制【按钮】
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="methodInfo">方法信息</param>
        private void DrawButton(string methodName, MethodInfo methodInfo)
        {
            // 判断 <【输入法名称】是否为【空】>
            if (string.IsNullOrEmpty(methodName))
            {
                // 获取【原生方法名称】
                methodName = methodInfo.Name;
            }

            // 绘制【调用方法】按钮，并判断 <该按钮是否被触发>
            if (GUILayout.Button(new GUIContent(methodName), GUILayout.ExpandWidth(true)))
            {
                // 遍历循环所有被检查实例
                foreach (UnityEngine.Object target in targets)
                {
                    object currentTarget;

                    // 判断 <【当前实例】的【类型】是否为【MonoBehaviour】的子级类型>
                    if (target.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        currentTarget = target as MonoBehaviour;
                    }
                    // 判断 <【当前实例】的【类型】是否为【ScriptableObject】的子级类型>
                    else if (target.GetType().IsSubclassOf(typeof(ScriptableObject)))
                    {
                        currentTarget = target as ScriptableObject;
                    }
                    else
                    {
                        continue;
                    }

                    // 判断 <【当前实例】是否为【空】>
                    if (currentTarget == null)
                    {
                        continue;
                    }

                    // 获取【调用方法的返回值】
                    System.Object value = methodInfo.Invoke(currentTarget, new object[] { });

                    // 判断 <返回值是否为迭代器（即协程方法返回值）>、<当前实例为【MonoBehaviour】的子类>
                    if (value is IEnumerator coroutine && target is MonoBehaviour monoBehaviour)
                    {
                        // 开启协程
                        monoBehaviour.StartCoroutine(coroutine);
                    }
                    // 判断 <返回值是否不为【空】>
                    else if (value != null)
                    {
                        Debug.Log($"{methodInfo.Name}() 方法调用结果：{value}");
                    }
                }
            }
        }
        #endregion
    }
}
#endif
