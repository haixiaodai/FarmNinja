using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Patrolling : MonoBehaviour
    {
        // 此脚本可以用于任何对象，支持沿着一条路线为标志的waypoints。
        // 此脚本管理向前看沿路线的数量，并跟踪进度和圈数。

        [SerializeField] private WaypointCircuit circuit; // 一个引用关于 waypoint-based 我们应该沿着的路线

        //actually the speed
        [SerializeField] public float lookAheadForTargetOffset = 5;
        // 我们将目标，沿着路线的偏移

        [SerializeField] private float lookAheadForTargetFactor = .1f;
        // 一个倍数，增加目标与沿着线路的距离，基于当前速度

        [SerializeField] private float lookAheadForSpeedOffset = 10;
        // 前面的偏移的唯一路线速度调整(作为waypoint目标的rotation变换)

        [SerializeField] private float lookAheadForSpeedFactor = .2f;
        // 一个倍数，沿着路线调整速度添加距离

        [SerializeField] private ProgressStyle progressStyle = ProgressStyle.SmoothAlongRoute;
        // 进度样式：是否smoothly更新位置沿着路线(好的曲线路径)或只是正常到达每个航点。

        [SerializeField] private float pointToPointThreshold = 4;
        // 接近waypoint的阈值，一旦达到这个值，目标将切换到下一个目标地点：只用于PointToPoint模式。

        public enum ProgressStyle
        {
            SmoothAlongRoute,
            PointToPoint,
        }

        // 这些是public，由其他对象读取。让一个AI知道在哪里头！
        public WaypointCircuit.RoutePoint targetPoint { get; private set; }
        public WaypointCircuit.RoutePoint speedPoint { get; private set; }
        public WaypointCircuit.RoutePoint progressPoint { get; private set; }

        public Transform target;

        private float progressDistance; // 圆形（环形）路线的进展，平滑smooth模式中使用。
        private int progressNum; // 当前waypoint数，点对点point-to-point模式中使用。
        private Vector3 lastPosition; // 用于计算当前速度(因为我们可能没有一个刚体组件)
        private float speed; // 此对象的当前速度(从最后一帧的delta计算) 

        // 设置脚本属性
        private void Start()
        {
            // 我们使用transform表示目标点，这个点被认为是为即将到来的变化的速度点。这允许此信息传递给 AI 而无需进一步依赖此组件。您可以手动创建transform设置该组件 *and* AI，然后此组件将更新它，和 AI 可以阅读它。

            if (target == null)
            {
                target = new GameObject(name + " Waypoint Target").transform;
            }

            Reset();
        }


        // 对象重置为合理的值
        public void Reset()
        {
            progressDistance = 0;
            progressNum = 0;
            if (progressStyle == ProgressStyle.PointToPoint)
            {
                target.position = circuit.Waypoints[progressNum].position;
                target.rotation = circuit.Waypoints[progressNum].rotation;
            }
        }


        private void Update()
        {
            if (progressStyle == ProgressStyle.SmoothAlongRoute)
            {
                // 确定我们目前目标的位置 (这是不同于当前的进展位置，它是一个确定值沿着前方路线的) 我们使用 lerp 作为一种随着时间的推移速度进行平滑处理的简单方式。
                if (Time.deltaTime > 0)
                {
                    speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude / Time.deltaTime,
                                       Time.deltaTime);
                }
                target.position =
                    circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed)
                           .position;
                target.rotation =
                    Quaternion.LookRotation(
                        circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed)
                               .direction);


                // 得到我们的当前路线的进展
                progressPoint = circuit.GetRoutePoint(progressDistance);
                Vector3 progressDelta = progressPoint.position - transform.position;
                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude * 0.5f;
                }

                lastPosition = transform.position;
            }
            else
            {
                // 点对点模式。 如果我们足够近，只是增加Waypoint：

                Vector3 targetDelta = target.position - transform.position;
                if (targetDelta.magnitude < pointToPointThreshold)
                {
                    progressNum = (progressNum + 1) % circuit.Waypoints.Length;
                }


                target.position = circuit.Waypoints[progressNum].position;
                target.rotation = circuit.Waypoints[progressNum].rotation;

                // 得到我们的当前路线的进展
                progressPoint = circuit.GetRoutePoint(progressDistance);
                Vector3 progressDelta = progressPoint.position - transform.position;
                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude;
                }
                lastPosition = transform.position;
            }
        }


        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + target.forward);
            }
        }
    }

