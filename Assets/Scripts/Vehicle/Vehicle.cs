using System;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Traffic
{
    [MoonSharpUserData]
    public class Vehicle
    {
        public static event EventHandler<Node> OnNodeChanged;
        public static event EventHandler<Vehicle> OnLaneIndexChanged;
        private readonly static float defaultSpeed = 5f;
        private static int vehicleCount;

        public int Id { get; private set; }

        private Vector3 offset;
        public Vector3 ModelSize { get; private set; }

        public Types type;
        public Transform model;
        public Transform transform;

        private int laneIndex;
        private float speed = 5f;
        public int previousNode = -1;
        public Node node;
        public Node.Directions direction;
        public Node.Directions previousDirection;

        public Vehicle()
        {
            Id = vehicleCount++;
            direction = Node.Directions.Invalid;

            OnNodeChanged += HandleNodeChanged;
            OnLaneIndexChanged += HandleLaneIndexChanged;

            Time.TimeManager.Instance.OnIntervalChanged += HandleIntervalChange;
        }

        ~Vehicle()
        {
            OnNodeChanged -= HandleNodeChanged;
            OnLaneIndexChanged -= HandleLaneIndexChanged;

            Time.TimeManager.Instance.OnIntervalChanged -= HandleIntervalChange;
        }

        private void HandleNodeChanged(object sender, Node node)
        {
            if (sender == this)
            {
                //SelectLane(UnityEngine.Random.Range(0, (int)(laneSize / 2f)));
                SelectLane(0);
            }
        }

        private void HandleLaneIndexChanged(object sender, Vehicle vehicle)
        {
            if (vehicle == this)
            {
                offset = new Vector3(Node.modelSizes[laneIndex], 0, -Node.modelSizes[laneIndex]);
            }
        }

        private void HandleIntervalChange(object sender, Time.Intervals intervals)
        {
            speed = defaultSpeed * (1f / intervals.Interval.tick_in_seconds);
        }

        public void Set(int id)
        {
            transform.name = $"{Id}";
            Renderer renderer = model.gameObject.GetComponent<Renderer>();
            ModelSize = renderer.bounds.size;

            SelectNode(id);
            transform.position = node.transform.position;
            SelectNextNode();
        }

        public void Move(float delta)
        {
            transform.position = Vector3.MoveTowards(transform.position, node.transform.position, speed * delta);

            Vector3 distance = node.transform.position - transform.position;
            if (distance != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(distance.normalized);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    lookRotation,
                    speed / Node.modelSizes[laneIndex] * delta);
            }

            if (Vector3.Distance(transform.position, node.transform.position) < .1f)
            {
                SelectNextNode();
            }

            ChangeLane(delta);
        }

        private void ChangeLane(float delta)
        {
            model.localPosition = offset / 2f;
            //model.localPosition = Vector3.Lerp(model.localPosition, offset/2f, speed/2f * delta);
        }

        private void SelectNextNode()
        {
            List<int> ids = new List<int>();
            if (node.NorthNode != -1 && previousNode != node.NorthNode)
                ids.Add(node.NorthNode);
            if (node.SouthNode != -1 && previousNode != node.SouthNode)
                ids.Add(node.SouthNode);
            if (node.WestNode != -1 && previousNode != node.WestNode)
                ids.Add(node.WestNode);
            if (node.EastNode != -1 && previousNode != node.EastNode)
                ids.Add(node.EastNode);
            if (ids.Count == 0) return;

            int id = UnityEngine.Random.Range(0, ids.Count);
            SelectNode(ids[id]);
        }

        public void SelectNode(int id)
        {
            if (node != null)
            {
                previousNode = node.Id;
                previousDirection = direction;
            }
            node = TrafficManager.Instance.nodes[type][id];
            direction = Node.GetDirection(node, previousNode);
            //transform.LookAt(node.transform);

            OnNodeChanged?.Invoke(this, node);
        }

        public void SelectLane(int index)
        {
            laneIndex = index;

            OnLaneIndexChanged?.Invoke(this, this);
        }
    }
}

