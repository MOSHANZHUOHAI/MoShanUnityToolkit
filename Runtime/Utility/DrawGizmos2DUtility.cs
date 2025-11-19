using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MoShan.Unity.EngineExpand
{
    using Vector2 = global::UnityEngine.Vector2;
    using Vector3 = global::UnityEngine.Vector3;

    /// <summary>
    /// 实用程序：二维线框绘制
    /// </summary>
    /// <remarks>
    /// 用于在【OnDrawGizmos】或【OnDrawGizmosSelected】生命周期方法期间进行绘制
    /// </remarks>
    public static class DrawGizmos2DUtility
    {
        #region 字段
        /// <summary>
        /// 栈：矩阵变更记录
        /// </summary>
        private static readonly Stack<Matrix4x4> s_MatrixRecords = new Stack<Matrix4x4>();

        /// <summary>
        /// 栈：颜色变更记录
        /// </summary>
        private static readonly Stack<Color> s_ColorRecords = new Stack<Color>();
        #endregion

        #region 属性
        /// <summary>
        /// 颜色深度
        /// </summary>
        /// <remarks>
        /// 当前已记录的颜色变更的总数
        /// </remarks>
        public static int ColorDepth
        {
            get
            {
                return s_ColorRecords.Count;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static DrawGizmos2DUtility() { }
        #endregion

        #region 公开方法

        #region 变更【颜色】
        /// <summary>
        /// 开始【颜色变更】
        /// </summary>
        /// <returns>返回开始颜色变更后，当前已记录的【线框颜色】变更的总数。</returns>
        public static int BeginColorChange()
        {
            // 记录当前【线框颜色】
            s_ColorRecords.Push(Gizmos.color);

            return s_ColorRecords.Count;
        }

        /// <summary>
        /// 开始【颜色变更】
        /// </summary>
        /// <param name="newColor">需要变更的新【线框颜色】</param>
        /// <returns>返回开始颜色变更后，当前已记录的【线框颜色】变更的总数。</returns>
        public static int BeginColorChange(Color newColor)
        {
            // 记录当前【线框颜色】
            s_ColorRecords.Push(Gizmos.color);

            // 更新【线框颜色】
            Gizmos.color = newColor;

            return s_ColorRecords.Count;
        }

        /// <summary>
        /// 结束【颜色变更】
        /// </summary>
        /// <returns>返回结束颜色变更后，当前仍记录的【线框颜色】变更的总数。</returns>
        public static int EndColorChange()
        {
            // 判断 <【颜色变更记录栈】是否为【空】>
            if (s_ColorRecords.Count == 0)
            {
                return 0;
            }

            // 恢复【线框颜色】为顶层的记录颜色
            Gizmos.color = s_ColorRecords.Pop();

            return s_ColorRecords.Count;
        }

        /// <summary>
        /// 结束【所有颜色变更】
        /// </summary>
        public static void EndAllColorChange()
        {
            // 判断 <【颜色变更记录栈】是否为【空】>
            if (s_ColorRecords.Count == 0)
            {
                return;
            }

            // While 循环以恢复【线框颜色】为最底层的记录颜色
            while (s_ColorRecords.Count > 0)
            {
                // 恢复【线框颜色】为当前循环轮次对应的颜色
                Gizmos.color = s_ColorRecords.Pop();
            }
        }

        /// <summary>
        /// 变更【颜色】
        /// </summary>
        /// <param name="newColor">需要变更的新【线框颜色】</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeColor(Color newColor)
        {
            Gizmos.color = newColor;
        }
        #endregion

        #region 变更【矩阵】
        /// <summary>
        /// 开始【矩阵变更】
        /// </summary>
        /// <returns>返回开始矩阵变更后，当前已记录的【线框矩阵】变更的总数。</returns>
        public static int BeginMatrixChange()
        {
            // 记录当前【线框矩阵】
            s_MatrixRecords.Push(Gizmos.matrix);

            return s_MatrixRecords.Count;
        }

        /// <summary>
        /// 开始【矩阵变更】
        /// </summary>
        /// <param name="nextColor">需要变更的新线框矩阵</param>
        /// <returns>返回开始矩阵变更后，当前已记录的【线框矩阵】变更的总数。</returns>
        public static int BeginMatrixChange(Matrix4x4 newMatrix)
        {
            // 记录当前【线框矩阵】
            s_MatrixRecords.Push(Gizmos.matrix);

            // 更新【线框矩阵】
            Gizmos.matrix = newMatrix;

            return s_MatrixRecords.Count;
        }

        /// <summary>
        /// 结束【矩阵变更】
        /// </summary>
        /// <returns>返回结束矩阵变更后，当前仍记录的【线框矩阵】变更的总数。</returns>
        public static int EndMatrixChange()
        {
            // 判断 <【矩阵变更记录栈】是否为【空】>
            if (s_MatrixRecords.Count == 0)
            {
                return 0;
            }

            // 恢复【线框矩阵】为顶层的记录矩阵
            Gizmos.matrix = s_MatrixRecords.Pop();

            return s_MatrixRecords.Count;
        }

        /// <summary>
        /// 结束【所有矩阵变更】
        /// </summary>
        public static void EndAllMatrixChange()
        {
            // 判断 <【矩阵变更记录栈】是否为【空】>
            if (s_MatrixRecords.Count == 0)
            {
                return;
            }

            // While 循环以恢复【线框矩阵】为最底层的记录矩阵
            while (s_MatrixRecords.Count > 0)
            {
                // 恢复【线框矩阵】为当前循环轮次对应的矩阵
                Gizmos.matrix = s_MatrixRecords.Pop();
            }
        }

        /// <summary>
        /// 移动【矩阵】
        /// </summary>
        /// <param name="position">位置</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveMatrix(Vector2 position)
        {
            // 应用【移动】到【线框矩阵】
            Gizmos.matrix *= Matrix4x4.Translate(position);
        }

        /// <summary>
        /// 旋转【矩阵】
        /// </summary>
        /// <param name="rotation">旋转（角度制）</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateMatrix(float rotation)
        {
            // 应用【旋转】到【线框矩阵】
            Gizmos.matrix *= Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, rotation));
        }

        /// <summary>
        /// 缩放【矩阵】
        /// </summary>
        /// <param name="angle">缩放</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScaleMatrix(Vector2 scale)
        {
            // 应用【缩放】到【线框矩阵】
            Gizmos.matrix *= Matrix4x4.Scale(scale);
        }

        /// <summary>
        /// 变换【矩阵】
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转（角度制）</param>
        /// <param name="scale">缩放</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformMatrix(Vector2 position, float rotation, Vector2 scale)
        {
            Gizmos.matrix *= Matrix4x4.TRS(position, Quaternion.Euler(0.0f, 0.0f, rotation), scale);
        }

        /// <summary>
        /// 设置【矩阵】
        /// </summary>
        /// <param name="value">值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMatrix(Matrix4x4 value)
        {
            Gizmos.matrix = value;
        }

        /// <summary>
        /// 重置【矩阵】
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetMatrix()
        {
            Gizmos.matrix = Matrix4x4.identity;
        }
        #endregion

        /// <summary>
        /// 绘制【线段】
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(Vector2 from, Vector2 to)
        {
            Gizmos.DrawLine(from, to);
        }

        /// <summary>
        /// 绘制【射线】
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="direction">方向</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawRay(Vector2 from, Vector2 direction)
        {
            Gizmos.DrawRay(from, direction);
        }

        /// <summary>
        /// 绘制【圆形】
        /// </summary>
        /// <param name="center">圆形中心坐标</param>
        /// <param name="radius">圆形半径</param>
        public static void DrawCircle(Vector2 center, float radius)
        {
            BeginMatrixChange();

            MoveMatrix(center);

            // 初始化【半径】
            radius = Math.Abs(radius);

            // 获取【线段总数】，取值范围为[3, +∞)
            int lineCount = 360;

            // 获取点与点之间基于圆心的角度间隔作为【增量】
            float delta = (2 * Mathf.PI) / lineCount;

            // 创建【点数组】
            Vector3[] points = new Vector3[lineCount];

            // 循环以填充【点数组】
            for (int i = 0; i < points.Length; i++)
            {
                // 填充点
                points[i] = new Vector3
                (
                    radius * Mathf.Cos(i * delta),
                    radius * Mathf.Sin(i * delta)
                );
            }

            // 连接【结束点位】和【起始点位】以闭环线框绘制
            InternalDrawLineStrip(points, true);

            EndMatrixChange();
        }

        /// <summary>
        /// 绘制【三角形】
        /// </summary>
        /// <param name="vertex_0">顶点_0</param>
        /// <param name="vertex_1">顶点_1</param>
        /// <param name="vertex_2">顶点_2</param>
        public static void DrawTriangle(Vector2 vertex_0, Vector2 vertex_1, Vector2 vertex_2)
        {
            BeginMatrixChange();

            InternalDrawLine(vertex_0, vertex_1);
            InternalDrawLine(vertex_1, vertex_2);
            InternalDrawLine(vertex_2, vertex_0);

            EndMatrixChange();
        }

        /// <summary>
        /// 绘制【多边形】
        /// </summary>
        /// <param name="vertexs">顶点数组</param>
        public static void DrawPolygon(params Vector2[] vertexs)
        {
            // 判断 <【输入顶点数组】是否为【空】>或<顶点数组长度是否小于【2】，即无法构成至少一条直线，无意义>
            if (vertexs == null || vertexs.Length < 2)
            {
                return;
            }

            BeginMatrixChange();

            // 循环以按照输入顶点顺序绘制【多边形】线框
            for (int i = 0; i < vertexs.Length - 1; i++)
            {
                InternalDrawLine(vertexs[i], vertexs[i + 1]);
            }

            // 绘制【多边形】外框，连接【最后点位】和【起始点位】以完成闭环
            InternalDrawLine(vertexs[vertexs.Length - 1], vertexs[0]);

            EndMatrixChange();
        }

        /// <summary>
        /// 绘制【等边多边形】
        /// </summary>
        /// <param name="center">等边多边形中心坐标</param>
        /// <param name="edgeCount">等边多边形边数</param>
        /// <param name="radius">等边多边形的尺寸约束（表现为圆的半径）</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawEquilateralPolygon(Vector2 center, int edgeCount, float radius, float angle)
        {
            // 判断 <【边数】是否小于【2】，即无法构成至少一条直线，无意义>
            if (edgeCount < 2)
            {
                return;
            }

            // 判断 <【边数】是否等于【2】>
            if (edgeCount == 2)
            {
                DrawLine(center + Vector2.left * radius, center + Vector2.right * radius);

                return;
            }

            BeginMatrixChange();

            TransformMatrix(center, angle, Vector2.one);

            #region 初始化【参数】
            // 判断 <【边数】是否为【偶数】>
            if (edgeCount % 2 == 0)
            {
                // 当该图形边数为【偶数】时，通过判断<边数是否可以被【4】整除>，偏移输入角度以确保该图形正下方在输入角度为【0°】时始终为水平的边
                angle = angle - (edgeCount % 4 == 0 ? 180.0f : 360.0f) / edgeCount;
            }
            else
            {
                // 当该图形边数为【奇数】时，偏移输入角度以确保该图形正上方在输入角度为【0°】时始终为角
                angle = angle + 90.0f;
            }
            #endregion

            // 声明【间隔】
            float delta = 2 * Mathf.PI / edgeCount;

            // 声明【起始点位】以记录首个点位坐标
            Vector2 originPoint = new Vector2(radius * Mathf.Cos(0f), radius * Mathf.Sin(0f));

            // 声明【开始点位】与【结束点位】
            Vector2 startPoint = originPoint;
            Vector2 endPoint   = Vector2.zero;

            // 循环以绘制该图形线框
            for (float theta = delta; theta < 2 * Mathf.PI; theta += delta)
            {
                // 更新【结束点位】
                endPoint = new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));

                // 绘制该图形线框
                InternalDrawLine(startPoint, endPoint);

                // 更新【开始点位】，以应用于下一轮循环中的绘制
                startPoint = endPoint;
            }

            // 连接【结束点位】和【起始点位】以闭环线框绘制
            InternalDrawLine(endPoint, originPoint);

            EndMatrixChange();
        }

        /// <summary>
        /// 绘制【星形】
        /// </summary>
        /// <param name="center">星形中心坐标</param>
        /// <param name="cornerCount">星形角数</param>
        /// <param name="radius">星形的尺寸约束（表现为圆的半径）</param>
        /// <param name="scale">星形的凹陷内收比例, 取值范围为[0, 1]</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawStar(Vector2 center, int cornerCount, float radius, float scale, float angle)
        {
            // 判断 <【星形角数】是否小于【2】，即无法构成至少一条直线，无意义>
            if (cornerCount < 2)
            {
                return;
            }

            #region 初始化【参数】
            // 偏移输入角度以确保该图形正上方在输入角度为【0°】时始终为角
            angle = angle + 90f;

            // 约束【凹陷内收比例】
            scale = Mathf.Clamp01(scale);
            #endregion

            BeginMatrixChange();

            TransformMatrix(center, angle, Vector2.one);

            // 声明【间隔】
            float delta = 2 * Mathf.PI / cornerCount;

            // 声明【起始点位】以记录首个点位坐标
            Vector2 originPoint = new Vector2(radius * Mathf.Cos(0f), radius * Mathf.Sin(0f));

            // 声明【上一角点位】与【当前角点位】
            Vector2 lastCornerPoint    = originPoint;
            Vector2 currentCornerPoint = Vector2.zero;

            // 声明【收缩点位】，即角度位于两个【角点位】之间的凹陷内收处的点位
            Vector2 shrinkPoint = Vector2.zero;

            // 声明【最大收缩点位】，即【上一角点位】与【当前角点位】的中点
            Vector2 maxShrinkPoint = Vector2.zero;

            // 循环以绘制该图形线框
            for (float theta = delta; theta < 2 * Mathf.PI; theta += delta)
            {
                // 更新【当前角点位】
                currentCornerPoint = new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));

                // 更新【最大收缩点位】
                maxShrinkPoint = (lastCornerPoint + currentCornerPoint) / 2;

                // 更新【收缩点位】
                shrinkPoint = Vector2.Lerp(center, maxShrinkPoint, scale);

                // 绘制该图形上一角的左侧内收线
                InternalDrawLine(lastCornerPoint, shrinkPoint);

                // 绘制该图形当前角的右侧内收线
                InternalDrawLine(currentCornerPoint, shrinkPoint);

                // 更新【上一角点位】
                lastCornerPoint = currentCornerPoint;
            }

            // 因为以上循环结束条件判断存在误差，所以需要判断 <最后一个角的点位是否等于【起始点位】，即是否需要补充一次绘制>
            if (lastCornerPoint != originPoint)
            {
                // 更新【最大收缩点位】
                maxShrinkPoint = (lastCornerPoint + originPoint) / 2;

                // 更新【收缩点位】
                shrinkPoint = Vector2.Lerp(center, maxShrinkPoint, scale);

                // 绘制该图形上一角的左侧内收线
                InternalDrawLine(lastCornerPoint, shrinkPoint);

                // 绘制该图形起始角的右侧内收线
                InternalDrawLine(originPoint, shrinkPoint);
            }

            EndMatrixChange();
        }

        /// <summary>
        /// 绘制【贝塞尔曲线】
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="startHelper">起始点辅助器</param>
        /// <param name="endHelper">结束点辅助器</param>
        /// <param name="end">结束点</param>
        public static void DrawBezierCurve(Vector2 start, Vector2 startHelper, Vector2 endHelper, Vector2 end)
        {
            BeginMatrixChange();

            // 循环以绘制【贝塞尔曲线】
            for (int i = 0; i <= 100; i += 1)
            {
                Vector2 current = Bezier3(start, startHelper, endHelper, end, i / 100f);

                Vector2 next    = Bezier3(start, startHelper, endHelper, end, (i + 1) / 100f);

                InternalDrawLine(current, next);
            }

            EndMatrixChange();
        }

        #region 扇形
        /// <summary>
        /// 绘制【扇形】
        /// </summary>
        /// <param name="center">扇形中心坐标</param>
        /// <param name="radius">扇形半径</param>
        /// <param name="range">扇形范围（角度制），取值范围为[0°, 360°)</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawSector(Vector2 center, Vector2 radius, float range, float angle)
        {
            DrawSector(center, radius, range, angle, true);
        }

        /// <summary>
        /// 绘制【扇形】
        /// </summary>
        /// <param name="center">扇形中心坐标</param>
        /// <param name="radius">扇形半径</param>
        /// <param name="range">扇形范围（角度制），取值范围为[0°, 360°)</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        /// <param name="isDrawEdge">是否绘制边缘</param>
        public static void DrawSector(Vector2 center, Vector2 radius, float range, float angle, bool isDrawEdge)
        {
            // 判断 <扇形范围是否为【0°】>
            if (range == 0.0f)
            {
                // 判断 <是否绘制边缘>
                if (isDrawEdge)
                {
                    BeginMatrixChange();

                    TransformMatrix(center, angle, Vector2.one);

                    // 绘制中心到【目标点位】的连线
                    InternalDrawLine(Vector2.zero, new Vector2(radius.x, 0.0f));

                    EndMatrixChange();
                }

                return;
            }

            // 判断 <扇形范围是否可以被【360°】整除，即表现为圆形>
            if (range % 360.0f == 0.0f)
            {
                DrawEllipse(center, radius, angle);

                return;
            }

            BeginMatrixChange();

            TransformMatrix(center, angle, Vector2.one);

            #region 初始化【参数】
            // 初始化【半径】
            radius = new Vector2(Math.Abs(radius.x), Math.Abs(radius.y));

            // 约束【输入范围】
            range %= 360.0f;

            // 判断 <【扇形范围】是否为【负数】>
            if (range < 0.0f)
            {
                range += 360.0f;
            }
            #endregion

            // 获取【整数范围】
            int integerRange = (int)range;

            // 获取转换为【弧度制】的【半范围】
            float halfRange = range / 2.0f * Mathf.Deg2Rad;

            // 获取【开始点】
            Vector3 start = GetEllipsePointByGeometricAngle(-halfRange);
            // 获取【结束点】
            Vector3 end   = GetEllipsePointByGeometricAngle( halfRange);

            // 判断 <是否绘制边缘>
            if (isDrawEdge)
            {
                // 连接【中心】与【开始点】以绘制【扇形边缘】
                // 使用【几何角】获取点以确保半径不同但开扇角度相同的扇形的两侧边缘重合
                InternalDrawLine(Vector3.zero, start);

                // 连接【中心】与【结束点】以绘制【扇形边缘】
                // 使用【几何角】获取点以确保半径不同但开扇角度相同的扇形的两侧边缘重合
                InternalDrawLine(Vector3.zero, end  );
            }

            // 获取不包括开始点与结束点在内的【线段总数】
            int lineCount = integerRange;

            // 创建【点数组】
            Vector3[] points = new Vector3[lineCount + 2];

            // 获取不包括开始点与结束点在内的点与点之间基于圆心的角度间隔作为【增量】
            float delta = integerRange / lineCount * Mathf.Deg2Rad;

            // 获取转换为【弧度制】的【半整数范围】
            float halfIntegerRange = integerRange / 2.0f * Mathf.Deg2Rad;

            // 获取【当前点弧度】
            float currentPointRadian = -halfIntegerRange;

            // 循环以填充【点数组】
            for (int i = 1; i < points.Length - 1; i++)
            {
                // 填充点
                points[i] = GetEllipsePointByGeometricAngle(currentPointRadian);

                currentPointRadian += delta;
            }

            // 填充【开始点】
            points[0] = start;
            // 填充【结束点】
            points[points.Length - 1] = end;

            // 绘制【扇形弧线】
            InternalDrawLineStrip(points, false);

            EndMatrixChange();

            #region 局部方法
            // 局部方法：获取【椭圆上一点的坐标（基于参数角）】
            // @param radians：角度（弧度制）
            // @returns：返回【输入参数角】对应的椭圆上一点的坐标。
            Vector2 GetEllipsePointByParameterAngle(float radians)
            {
                /*
                 * 椭圆上一点（x，y）公式：
                 * x = 圆心.x + 半径.x * Cos(角度)
                 * y = 圆心.y + 半径.y * Sin(角度)
                 */

                return new Vector2
                (
                    radius.x * MathF.Cos(radians),
                    radius.y * MathF.Sin(radians)
                );
            }

            // 局部方法：获取【椭圆上一点的坐标（基于几何角）】
            // @param radians：角度（弧度制）
            // @returns：返回【输入几何角】对应的椭圆上一点的坐标。
            Vector2 GetEllipsePointByGeometricAngle(float radians)
            {
                // 获取【方向】
                Vector2 direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians));

                // 计算射线与椭圆的交点

                // 获取【斜率】
                // 射线方程: y = tan(θ) * x
                float slope = direction.y / direction.x;

                // 椭圆方程: (x / radius.X) ^ 2 + (y / radius.Y) ^ 2 = 1
                // 获取【X 轴交点】
                float xIntersect = MathF.Sqrt(1 / (1 / (radius.x * radius.x) + slope * slope / (radius.y * radius.y)));
                // 获取【Y 轴交点】
                float yIntersect = slope * xIntersect;

                // 取绝对值处理，确保值都为正
                xIntersect = Math.Abs(xIntersect);
                yIntersect = Math.Abs(yIntersect);

                // 判断 <【方向】的【X 轴坐标】是否小于【0】>，即<【当前局部坐标】是否位于笛卡尔坐标系的左侧>
                if (direction.x < 0)
                {
                    xIntersect = -xIntersect;
                }

                // 判断 <【方向】的【Y 轴坐标】是否小于【0】>，即<【当前局部坐标】是否位于笛卡尔坐标系的下方>
                if (direction.y < 0)
                {
                    yIntersect = -yIntersect;
                }

                return new Vector2(xIntersect, yIntersect);
            }
            #endregion
        }
        #endregion

        #region 椭圆
        /// <summary>
        /// 绘制【椭圆】
        /// </summary>
        /// <param name="center">椭圆中心坐标</param>
        /// <param name="radius">椭圆半径</param>
        public static void DrawEllipse(Vector2 center, Vector2 radius)
        {
            DrawEllipse(center, radius, 0.0f);
        }

        /// <summary>
        /// 绘制【椭圆】
        /// </summary>
        /// <param name="center">椭圆中心坐标</param>
        /// <param name="radius">椭圆半径</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawEllipse(Vector2 center, Vector2 radius, float angle)
        {
            BeginMatrixChange();

            TransformMatrix(center, angle, Vector2.one);

            // 获取【线段总数】，取值范围为[3, +∞)
            int lineCount = 360;

            // 获取点与点之间基于圆心的角度间隔作为【增量】
            float delta = (2 * Mathf.PI) / lineCount;

            // 创建【点数组】
            Vector3[] points = new Vector3[lineCount];

            // 循环以填充【点数组】
            for (int i = 0; i < points.Length; i++)
            {
                // 填充点
                points[i] = new Vector3
                (
                    radius.x * Mathf.Cos(i * delta),
                    radius.y * Mathf.Sin(i * delta)
                );
            }

            // 连接【结束点位】和【起始点位】以闭环线框绘制
            InternalDrawLineStrip(points, true);

            EndMatrixChange();
        }
        #endregion

        #region 矩形
        /// <summary>
        /// 绘制【矩形】
        /// </summary>
        /// <param name="center">矩形中心坐标</param>
        /// <param name="size">矩形尺寸</param>
        public static void DrawRect(Vector2 center, Vector2 size)
        {
            DrawRect(center, size.x, size.y, 0.0f);
        }

        /// <summary>
        /// 绘制【矩形】
        /// </summary>
        /// <param name="center">矩形中心坐标</param>
        /// <param name="size">矩形尺寸</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawRect(Vector2 center, Vector2 size, float angle)
        {
            DrawRect(center, size.x, size.y, angle);
        }

        /// <summary>
        /// 绘制【矩形】
        /// </summary>
        /// <param name="center">矩形中心坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        public static void DrawRect(Vector2 center, float width, float height)
        {
            DrawRect(center, width, height, 0.0f);
        }

        /// <summary>
        /// 绘制【矩形】
        /// </summary>
        /// <param name="center">矩形中心坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawRect(Vector2 center, float width, float height, float angle)
        {
            BeginMatrixChange();

            TransformMatrix(center, angle, Vector2.one);

            // 获取【矩形】顶点
            Vector2 leftTop     = new Vector2(-width / 2.0f,  height / 2.0f);
            Vector2 rightTop    = new Vector2( width / 2.0f,  height / 2.0f);
            Vector2 leftBottom  = new Vector2(-width / 2.0f, -height / 2.0f);
            Vector2 rightBottom = new Vector2( width / 2.0f, -height / 2.0f);

            // 绘制【矩形】线框
            InternalDrawLine(leftTop   , rightTop   );
            InternalDrawLine(leftTop   , leftBottom );
            InternalDrawLine(rightTop  , rightBottom);
            InternalDrawLine(leftBottom, rightBottom);

            EndMatrixChange();
        }
        #endregion

        #region 网格
        /// <summary>
        /// 绘制【网格】
        /// </summary>
        /// <param name="center">中心</param>
        /// <param name="size">格子尺寸</param>
        /// <param name="row">格子行数</param>
        /// <param name="column">格子列数</param>
        public static void DrawGrid(Vector2 center, Vector2 size, int row, int column)
        {
            DrawGrid(center, size.x, size.y, row, column, 0f, Vector2.one, Vector2.one);
        }

        /// <summary>
        /// 绘制【网格】
        /// </summary>
        /// <param name="center">中心</param>
        /// <param name="size">格子尺寸</param>
        /// <param name="row">格子行数</param>
        /// <param name="column">格子列数</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawGrid(Vector2 center, Vector2 size, int row, int column, float angle)
        {
            DrawGrid(center, size.x, size.y, row, column, angle, Vector2.one, Vector2.one);
        }

        /// <summary>
        /// 绘制【网格】
        /// </summary>
        /// <param name="center">中心</param>
        /// <param name="size">格子尺寸</param>
        /// <param name="row">格子行数</param>
        /// <param name="column">格子列数</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        /// <param name="scaleBeforeRotate">旋转前缩放</param>
        /// <param name="scaleAfterRotate">旋转后缩放</param>
        public static void DrawGrid(Vector2 center, Vector2 size, int row, int column, float angle, Vector2 scaleBeforeRotate, Vector2 scaleAfterRotate)
        {
            DrawGrid(center, size.x, size.y, row, column, angle, scaleBeforeRotate, scaleAfterRotate);
        }

        /// <summary>
        /// 绘制【网格】
        /// </summary>
        /// <param name="center">中心</param>
        /// <param name="width">格子宽度</param>
        /// <param name="height">格子高度</param>
        /// <param name="row">格子行数</param>
        /// <param name="column">格子列数</param>
        public static void DrawGrid(Vector2 center, float width, float height, int row, int column)
        {
            DrawGrid(center, width, width, row, column, 0f, Vector2.one, Vector2.one);
        }

        /// <summary>
        /// 绘制【网格】
        /// </summary>
        /// <param name="center">中心</param>
        /// <param name="width">格子宽度</param>
        /// <param name="height">格子高度</param>
        /// <param name="row">格子行数</param>
        /// <param name="column">格子列数</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawGrid(Vector2 center, float width, float height, int row, int column, float angle)
        {
            DrawGrid(center, width, width, row, column, angle, Vector2.one, Vector2.one);
        }

        /// <summary>
        /// 绘制【网格】
        /// </summary>
        /// <param name="center">中心</param>
        /// <param name="width">格子宽度</param>
        /// <param name="height">格子高度</param>
        /// <param name="row">格子行数</param>
        /// <param name="column">格子列数</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        /// <param name="scaleBeforeRotate">旋转前缩放</param>
        /// <param name="scaleAfterRotate">旋转后缩放</param>
        public static void DrawGrid(Vector2 center, float width, float height, int row, int column, float angle, Vector2 scaleBeforeRotate, Vector2 scaleAfterRotate)
        {
            BeginMatrixChange();

            // 应用【旋转前的缩放】到【线框矩阵】
            ScaleMatrix(new Vector3(scaleBeforeRotate.x, scaleBeforeRotate.y, 1));

            // 应用【旋转】到【线框矩阵】
            RotateMatrix(angle);

            // 应用【旋转后的缩放】到【线框矩阵】
            ScaleMatrix(new Vector3(scaleAfterRotate.x, scaleAfterRotate.y, 1));

            // 应用【旋转】到【线框矩阵】
            MoveMatrix(center);

            // 获取网格【总尺寸】
            Vector2 totalSize = new Vector2(width * column, height * row);

            // 获取网格【起始点（左下角）】
            Vector2 startPoint = -totalSize * 0.5f;

            // 循环以绘制【网格】
            for (int i = 0; i <= row; i++)
            {
                for (int j = 0; j <= column; j++)
                {
                    // 计算当前格子的位置
                    Vector2 point = startPoint + new Vector2(j * width, i * height);

                    // 绘制【垂直线】
                    if (j < column)
                    {
                        Vector2 nextPoint = startPoint + new Vector2((j + 1) * width, i * height);

                        InternalDrawLine(point, nextPoint);
                    }

                    // 绘制【水平线】
                    if (i < row)
                    {
                        Vector2 nextPoint = startPoint + new Vector2(j * width, (i + 1) * height);

                        InternalDrawLine(point, nextPoint);
                    }
                }
            }

            EndMatrixChange();
        }
        #endregion

        #region 箭头
        /// <summary>
        /// 绘制【箭头】
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="direction">方向（角度制）</param>
        /// <param name="distance">距离</param>
        /// <param name="arrowHeadLength">箭头头部长度</param>
        /// <param name="arrowHeadAngle">箭头头部展开角度，取值范围[0°, 180°]</param>
        public static void DrawArrow(Vector2 start, float direction, float distance, float arrowHeadLength, float arrowHeadAngle)
        {
            // 获取【终点】
            Vector2 end = start + new Vector2(distance, 0);

            // 判断 <方向是否不为【0】，即是否需要旋转>
            if (direction != 0f)
            {
                // 旋转【终点】
                end = RotatePoint(end, start, direction);
            }

            DrawArrow(start, end, arrowHeadLength, arrowHeadAngle);
        }

        /// <summary>
        /// 绘制【箭头】
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="arrowHeadLength">箭头头部长度</param>
        /// <param name="arrowHeadAngle">箭头头部展开角度，取值范围[0°, 180°]</param>
        public static void DrawArrow(Vector2 start, Vector2 end, float arrowHeadLength, float arrowHeadAngle)
        {
            BeginMatrixChange();

            // 绘制【箭头主线】
            InternalDrawLine(start, end);

            // 约束输入的【箭头头部展开角度】
            arrowHeadAngle = arrowHeadAngle % 180f;

            // 判断 <【箭头头部展开角度】是否为【负数】>
            if (arrowHeadAngle < 0.0f)
            {
                arrowHeadAngle += 180.0f;
            }

            // 获取【方向向量】
            Vector2 direction = (end - start).normalized;

            // 获取【箭头头部展开半角度】
            arrowHeadAngle *= 0.5f;

            // 获取【箭头头部向量】
            Vector2 right = Quaternion.Euler(0, 0, arrowHeadAngle) * -direction;
            Vector2 left = Quaternion.Euler(0, 0, -arrowHeadAngle) * -direction;

            // 绘制【箭头头部线段】
            InternalDrawLine(end, end + right * arrowHeadLength);
            InternalDrawLine(end, end + left * arrowHeadLength);

            EndMatrixChange();
        }
        #endregion

        #region 菱形
        /// <summary>
        /// 绘制【菱形】
        /// </summary>
        /// <param name="center">菱形中心坐标</param>
        /// <param name="size">菱形尺寸</param>
        public static void DrawDiamond(Vector2 center, Vector2 size)
        {
            DrawDiamond(center, size.x, size.y, 0f);
        }

        /// <summary>
        /// 绘制【菱形】
        /// </summary>
        /// <param name="center">菱形中心坐标</param>
        /// <param name="size">菱形尺寸</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawDiamond(Vector2 center, Vector2 size, float angle)
        {
            DrawDiamond(center, size.x, size.y, angle);
        }

        /// <summary>
        /// 绘制【菱形】
        /// </summary>
        /// <param name="center">菱形中心坐标</param>
        /// <param name="width">菱形宽度</param>
        /// <param name="height">菱形高度</param>
        public static void DrawDiamond(Vector2 center, float width, float height)
        {
            DrawDiamond(center, width, height, 0f);
        }

        /// <summary>
        /// 绘制【菱形】
        /// </summary>
        /// <param name="center">菱形中心坐标</param>
        /// <param name="width">菱形宽度</param>
        /// <param name="height">菱形高度</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        public static void DrawDiamond(Vector2 center, float width, float height, float angle)
        {
            BeginMatrixChange();

            TransformMatrix(center, angle, Vector2.one);

            // 获取【菱形】顶点
            Vector2 top    = new Vector2(0.0f         ,  height * 0.5f);
            Vector2 bottom = new Vector2(0.0f         , -height * 0.5f);
            Vector2 left   = new Vector2(-width * 0.5f, 0.0f          );
            Vector2 right  = new Vector2( width * 0.5f, 0.0f          );

            // 绘制【菱形】线框
            InternalDrawLine(top   , right );
            InternalDrawLine(right , bottom);
            InternalDrawLine(bottom, left  );
            InternalDrawLine(left  , top   );

            EndMatrixChange();
        }
        #endregion

        #endregion

        #region 私有方法
        /// <summary>
        /// 内部绘制【线段】
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InternalDrawLine(Vector3 from, Vector3 to)
        {
            Gizmos.DrawLine(from, to);
        }

        /// <summary>
        /// 内部绘制【线段序列】
        /// </summary>
        /// <remarks>
        /// 使用前一个点与后一个点进行线段绘制
        /// </remarks>
        /// <param name="points">所有点</param>
        /// <param name="looped">是否连接第一个点与最后一个点</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InternalDrawLineStrip(ReadOnlySpan<Vector3> points, bool looped)
        {
            Gizmos.DrawLineStrip(points, looped);
        }

        /// <summary>
        /// 旋转点
        /// </summary>
        /// <param name="point">需要进行旋转的点</param>
        /// <param name="pivot">轴心</param>
        /// <param name="angle">旋转角度（角度制），取值范围为[0°, 360°)</param>
        /// <returns>返回围绕轴心进行旋转后的旋转点。</returns>
        private static Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
        {
            // 将【角度制】角度转换为【弧度制】角度
            float angleRadians = angle * Mathf.Deg2Rad;

            // 计算旋转后的坐标
            float cos = Mathf.Cos(angleRadians);
            float sin = Mathf.Sin(angleRadians);

            float x = pivot.x + (point.x - pivot.x) * cos - (point.y - pivot.y) * sin;
            float y = pivot.y + (point.x - pivot.x) * sin + (point.y - pivot.y) * cos;

            return new Vector2(x, y);
        }

        /// <summary>
        /// 三阶贝塞尔曲线
        /// </summary>
        /// <param name="point_0">起点</param>
        /// <param name="point_1">控制点_1</param>
        /// <param name="point_2">控制点_2</param>
        /// <param name="point_3">终点</param>
        /// <param name="t">插值，取值范围为[0, 1]</param>
        /// <returns>返回在多点之间进行贝塞尔曲线插值的结果。</returns>
        private static Vector2 Bezier3(Vector2 point_0, Vector2 point_1, Vector2 point_2, Vector2 point_3, float t)
        {
            t = Mathf.Clamp01(t);

            Vector2 point_0_1 = (1 - t) * point_0 + t * point_1;
            Vector2 point_1_2 = (1 - t) * point_1 + t * point_2;
            Vector2 point_2_3 = (1 - t) * point_2 + t * point_3;

            Vector2 Point_0_1_2 = (1 - t) * point_0_1 + t * point_1_2;
            Vector2 Point_1_2_3 = (1 - t) * point_1_2 + t * point_2_3;

            return (1 - t) * Point_0_1_2 + t * Point_1_2_3;
        }
        #endregion
    }
}
