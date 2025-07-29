using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Vector2 = global::UnityEngine.Vector2;
    using Vector3 = global::UnityEngine.Vector3;

    /// <summary>
    /// 实用程序：绘制 GL
    /// </summary>
    /// <remarks>
    /// 在URP渲染管线下，需要在【OnGUI】生命周期方法期间进行绘制
    /// </remarks>
    public static partial class DrawGLUtility
    {
        #region 字段
        /// <summary>
        /// 是否正在绘制
        /// </summary>
        private static bool s_IsDrawing = false;

        /// <summary>
        /// 矩阵模式
        /// </summary>
        private static GLMatrixMode s_MatrixMode = GLMatrixMode.World;

        /// <summary>
        /// 形状模式
        /// </summary>
        private static DrawShapeMode s_ShapeMode = DrawShapeMode.Both;

        /// <summary>
        /// 线材质
        /// </summary>
        private static Material s_LineMaterial;
        #endregion

        #region 属性
        /// <summary>
        /// 矩阵模式
        /// </summary>
        public static GLMatrixMode MatrixMode
        {
            get
            {
                return s_MatrixMode;
            }
            set
            {
                // 判断 <【矩阵模式】是否等于【输入值】>
                if (s_MatrixMode == value)
                {
                    return;
                }

                s_MatrixMode = value;
            }
        }

        /// <summary>
        /// 形状模式
        /// </summary>
        public static DrawShapeMode ShapeMode
        {
            get
            {
                return s_ShapeMode;
            }
            set
            {
                // 判断 <【形状模式】是否等于【输入值】>
                if (s_ShapeMode == value)
                {
                    return;
                }

                s_ShapeMode = value;
            }
        }

        /// <summary>
        /// 是否绘制线
        /// </summary>
        private static bool IsDrawLine
        {
            get
            {
                return s_ShapeMode == DrawShapeMode.Both || s_ShapeMode == DrawShapeMode.Line;
            }
        }

        /// <summary>
        /// 是否绘制填充
        /// </summary>
        private static bool IsDrawFall
        {
            get
            {
                return s_ShapeMode == DrawShapeMode.Both || s_ShapeMode == DrawShapeMode.Fall;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static DrawGLUtility()
        {
            // 判断 <【线材质】是否为【空】>
            if (!s_LineMaterial)
            {
                s_LineMaterial = new Material(Shader.Find("UI/Default"));

                s_LineMaterial.hideFlags = HideFlags.HideAndDontSave;

                // Unity 的内置材质，用于绘制简单的彩色内容
                Shader shader = Shader.Find("Hidden/Internal-Colored");

                // 创建【材质】
                s_LineMaterial = new Material(shader);

                s_LineMaterial.hideFlags = HideFlags.HideAndDontSave;

                // 启用【Alpha 混合（透明度通道）】
                s_LineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                s_LineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                // 关闭【背面剔除】
                s_LineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

                // 关闭【深度写入】
                s_LineMaterial.SetInt("_ZWrite", 0);
            }
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 绘制【线段】
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="color">颜色</param>
        public static void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            BeginDraw(GLDrawMode.Line);

            // 设置【颜色】
            GL.Color(color);

            AddLineVertex(start, end);

            EndDraw();
        }

        #region 形状
        /// <summary>
        /// 绘制【三角形】
        /// </summary>
        /// <param name="vertex_0">顶点_0</param>
        /// <param name="vertex_1">顶点_1</param>
        /// <param name="vertex_2">顶点_2</param>
        /// <param name="lineColor">线颜色</param>
        /// <param name="fallColor">填充颜色</param>
        public static void DrawTriangle(Vector3 vertex_0, Vector3 vertex_1, Vector3 vertex_2, Color lineColor, Color fallColor)
        {
            // 判断 <是否绘制填充>
            if (IsDrawFall)
            {
                BeginDraw(GLDrawMode.Triangle);

                // 设置【颜色】
                GL.Color(fallColor);

                AddTriangleVertex(vertex_0, vertex_1, vertex_2);

                EndDraw();
            }

            // 判断 <是否绘制线>
            if (IsDrawLine)
            {
                BeginDraw(GLDrawMode.Line);

                // 设置【颜色】
                GL.Color(lineColor);

                AddLineVertex(vertex_0, vertex_1);
                AddLineVertex(vertex_1, vertex_2);
                AddLineVertex(vertex_2, vertex_0);

                EndDraw();
            }
        }

        /// <summary>
        /// 绘制【四边形】
        /// </summary>
        /// <param name="vertex_0">顶点_0</param>
        /// <param name="vertex_1">顶点_1</param>
        /// <param name="vertex_2">顶点_2</param>
        /// <param name="vertex_2">顶点_3</param>
        /// <param name="lineColor">线颜色</param>
        /// <param name="fallColor">填充颜色</param>
        public static void DrawQuad(Vector3 vertex_0, Vector3 vertex_1, Vector3 vertex_2, Vector3 vertex_3, Color lineColor, Color fallColor)
        {
            // 判断 <是否绘制填充>
            if (IsDrawFall)
            {
                BeginDraw(GLDrawMode.Quad);

                // 设置【颜色】
                GL.Color(fallColor);

                AddQuadVertex(vertex_0, vertex_1, vertex_2, vertex_3);

                EndDraw();
            }

            // 判断 <是否绘制线>
            if (IsDrawLine)
            {
                BeginDraw(GLDrawMode.Line);

                // 设置【颜色】
                GL.Color(lineColor);

                AddLineVertex(vertex_0, vertex_1);
                AddLineVertex(vertex_1, vertex_2);
                AddLineVertex(vertex_2, vertex_3);
                AddLineVertex(vertex_3, vertex_0);

                EndDraw();
            }
        }

        /// <summary>
        /// 绘制【圆】
        /// </summary>
        /// <param name="center">中心</param>
        /// <param name="radius">半径</param>
        /// <param name="lineColor">线颜色</param>
        /// <param name="fallColor">填充颜色</param>
        public static void DrawCircle(Vector3 center, float radius, Color lineColor, Color fallColor)
        {
            // 初始化【半径】
            radius = Math.Abs(radius);
            
            // 判断 <是否绘制填充>
            if (IsDrawFall)
            {
                BeginDraw(GLDrawMode.Triangle);

                // 设置【颜色】
                GL.Color(fallColor);

                // 获取【间隔】
                float delta = 2 * Mathf.PI / 360;

                // 获取【起始点位】以记录首个点位坐标
                Vector2 originPoint = center + new Vector3(radius * Mathf.Cos(0.0f), radius * Mathf.Sin(0.0f));

                // 获取【开始点位】与【结束点位】
                Vector2 startPoint = originPoint;
                Vector2 endPoint = Vector2.zero;

                // 循环以绘制该图形填充
                for (float theta = delta; theta < 2 * Mathf.PI; theta += delta)
                {
                    // 更新【结束点位】
                    endPoint = center + new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));

                    AddTriangleVertex(center, startPoint, endPoint);

                    // 更新【开始点位】，以应用于下一轮循环中的绘制
                    startPoint = endPoint;
                }

                // 连接【结束点位】和【起始点位】以闭环填充绘制
                AddTriangleVertex(center, endPoint, originPoint);

                EndDraw();
            }

            // 判断 <是否绘制线>
            if (IsDrawLine)
            {
                BeginDraw(GLDrawMode.Line);

                // 设置【颜色】
                GL.Color(lineColor);

                // 获取【间隔】
                float delta = 2 * Mathf.PI / 360;

                // 获取【起始点位】以记录首个点位坐标
                Vector2 originPoint = center + new Vector3(radius * Mathf.Cos(0.0f), radius * Mathf.Sin(0.0f));

                // 获取【开始点位】与【结束点位】
                Vector2 startPoint = originPoint;
                Vector2 endPoint = Vector2.zero;

                // 循环以绘制该图形线框
                for (float theta = delta; theta < 2 * Mathf.PI; theta += delta)
                {
                    // 更新【结束点位】
                    endPoint = center + new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));

                    AddLineVertex(startPoint, endPoint);

                    // 更新【开始点位】，以应用于下一轮循环中的绘制
                    startPoint = endPoint;
                }

                // 连接【结束点位】和【起始点位】以闭环线框绘制
                AddLineVertex(endPoint, originPoint);

                EndDraw();
            }
        }

        /// <summary>
        /// 绘制【矩形】
        /// </summary>
        /// <param name="center">矩形中心坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        /// <param name="lineColor">线颜色</param>
        /// <param name="fallColor">填充颜色</param>
        public static void DrawRect(Vector3 center, float width, float height, float angle, Color lineColor, Color fallColor)
        {
            // 获取【矩形】顶点
            Vector2 leftTop     = center + new Vector3(-width / 2,  height / 2);
            Vector2 rightTop    = center + new Vector3( width / 2,  height / 2);
            Vector2 leftBottom  = center + new Vector3(-width / 2, -height / 2);
            Vector2 rightBottom = center + new Vector3( width / 2, -height / 2);

            // 基于【中心位置】与【旋转角度】以偏移【顶点】
            leftTop     = RotatePoint(leftTop    , center, angle);
            rightTop    = RotatePoint(rightTop   , center, angle);
            leftBottom  = RotatePoint(leftBottom , center, angle);
            rightBottom = RotatePoint(rightBottom, center, angle);

            DrawQuad(leftTop, rightTop, rightBottom, leftBottom, lineColor, fallColor);
        }
        #endregion

        #endregion

        #region 私有方法
        /// <summary>
        /// 开始绘制
        /// </summary>
        /// <param name="drawMode">绘制模式</param>
        private static void BeginDraw(GLDrawMode drawMode)
        {
            // 判断 <【输入模式】是否为【无】>，即<【输入模式】是否无效>
            if (drawMode == GLDrawMode.None)
            {
                return;
            }

            // 保存【模型、视图和投影矩阵】到【矩阵堆栈顶部】
            GL.PushMatrix();

            // 应用材质
            s_LineMaterial.SetPass(0);

            switch (s_MatrixMode)
            {
                // 默认
                case GLMatrixMode.Identity:
                    // 将标识加载到当前的模型和视图矩阵中。
                    GL.LoadIdentity();
                    break;

                // 正交
                case GLMatrixMode.Ortho:
                    // 将正交投影加载到投影矩阵中，将标识加载到 模型和视图矩阵中。
                    GL.LoadOrtho();
                    break;

                // 屏幕像素
                case GLMatrixMode.Pixel:
                    // 设置一个用于像素校正渲染的矩阵。
                    // 将正交投影加载到投影矩阵中，将标识加载到 模型和视图矩阵中。
                    // 在该投影矩阵中，X 和 Y 坐标直接映射到像素。坐标(0, 0) 位于当前摄像机视口的左下角。Z 坐标从近平面 的 1 到远平面的 - 100。
                    GL.LoadPixelMatrix();
                    break;

                // 世界空间
                case GLMatrixMode.World:
                    // 将标识加载到当前的模型和视图矩阵中。
                    GL.LoadIdentity();

                    // 判断 <【主摄像机】是否不为【空】>
                    if (Camera.main != null)
                    {
                        // 获取【主摄像机】的【矩阵】
                        Matrix4x4 cameraMatrix = Camera.main.worldToCameraMatrix;

                        // 补偿 Unity 摄像机矩阵的 Z 轴反转
                        cameraMatrix *= Matrix4x4.Scale(new Vector3(1, 1, -1));

                        // 设置【GL 矩阵】为【世界空间矩阵】
                        GL.MultMatrix(cameraMatrix);
                    }
                    else
                    {
                        Debug.LogWarning("【主摄像机】为【空】，无法正确初始化 GL 绘制。");
                    }
                    break;

                // 默认
                default:
                    // 将标识加载到当前的模型和视图矩阵中。
                    GL.LoadIdentity();
                    break;
            }

            // 开始【绘制图元】
            GL.Begin((int)drawMode);

            // 开始【绘制】
            s_IsDrawing = true;
        }

        /// <summary>
        /// 结束绘制
        /// </summary>
        private static void EndDraw()
        {
            // 结束【绘制图元】
            GL.End();

            // 恢复【模型、视图和投影矩阵】为【矩阵堆栈顶部】
            GL.PopMatrix();

            // 关闭【绘制】
            s_IsDrawing = false;
        }

        /// <summary>
        /// 添加【线段顶点】
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        private static void AddLineVertex(Vector3 start, Vector3 end)
        {
            // 添加【开始顶点】
            GL.Vertex(start);

            // 添加【结束顶点】
            GL.Vertex(end);
        }

        /// <summary>
        /// 添加【三角形顶点】
        /// </summary>
        /// <param name="vertex_0">顶点_0</param>
        /// <param name="vertex_1">顶点_1</param>
        /// <param name="vertex_2">顶点_2</param>
        private static void AddTriangleVertex(Vector3 vertex_0, Vector3 vertex_1, Vector3 vertex_2)
        {
            // 添加【顶点】
            GL.Vertex(vertex_0);
            GL.Vertex(vertex_1);
            GL.Vertex(vertex_2);
        }

        /// <summary>
        /// 添加【四边形顶点】
        /// </summary>
        /// <param name="vertex_0">顶点_0</param>
        /// <param name="vertex_1">顶点_1</param>
        /// <param name="vertex_2">顶点_2</param>
        /// <param name="vertex_3">顶点_3</param>
        private static void AddQuadVertex(Vector3 vertex_0, Vector3 vertex_1, Vector3 vertex_2, Vector3 vertex_3)
        {
            // 添加【顶点】
            GL.Vertex(vertex_0);
            GL.Vertex(vertex_1);
            GL.Vertex(vertex_2);
            GL.Vertex(vertex_3);
        }

        /// <summary>
        /// 旋转点
        /// </summary>
        /// <param name="point">需要进行旋转的点</param>
        /// <param name="pivot">轴心</param>
        /// <param name="angle">旋转角度(角度制)，取值范围为[0°, 360°)</param>
        /// <returns>返回围绕轴心进行旋转后的旋转点。</returns>
        private static Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
        {
            // 将【角度制】角度转换为【弧度制】角度
            float angleRadians = angle * Mathf.Deg2Rad;

            // 计算旋转后的坐标
            float cosTheta = Mathf.Cos(angleRadians);
            float sinTheta = Mathf.Sin(angleRadians);

            float x = pivot.x + (point.x - pivot.x) * cosTheta - (point.y - pivot.y) * sinTheta;
            float y = pivot.y + (point.x - pivot.x) * sinTheta + (point.y - pivot.y) * cosTheta;

            return new Vector2(x, y);
        }
        #endregion
    }
}
