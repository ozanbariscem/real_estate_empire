using UnityEngine;
using MoonSharp.Interpreter;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Traffic
{
    [MoonSharpUserData]
    public class Node
    {
        public enum Directions { North, South, West, East, Invalid }

        public static event EventHandler<Vehicle> OnVehicleEntered;
        public static event EventHandler<Vehicle> OnVehicleExited;

        public static float[] modelSizes;

        public int Id { get; private set; }

        public byte laneSize;

        [JsonIgnore]
        public Dictionary<Directions, List<Vehicle>> vehicles;

        [JsonIgnore]
        public Transform transform;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(-1)]
        public int NorthNode { get; private set; } = -1;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(-1)]
        public int SouthNode { get; private set; } = -1;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(-1)]
        public int WestNode { get; private set; } = -1;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(-1)]
        public int EastNode { get; private set; } = -1;

        public Node(int id)
        {
            Id = id;
            vehicles = new Dictionary<Directions, List<Vehicle>>();
            vehicles.Add(Directions.North, new List<Vehicle>());
            vehicles.Add(Directions.South, new List<Vehicle>());
            vehicles.Add(Directions.West, new List<Vehicle>());
            vehicles.Add(Directions.East, new List<Vehicle>());
            
            Vehicle.OnNodeChanged += HandleVehicleNodeChange;
        }

        ~Node()
        {
            Vehicle.OnNodeChanged -= HandleVehicleNodeChange;
        }

        public void Move(float delta)
        {
            foreach (var key in vehicles.Keys)
            {
                if (vehicles[key].Count == 0) continue;

                List<Vehicle> list = vehicles[key];

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        list[i].Move(delta);
                        break;
                    }

                    if (!VehicleIsCloseToFront(list[i], list[i - 1]))
                    {
                        list[i].Move(delta);
                    }
                    // Debug.LogWarning($"{list[i].Id} is waiting for {list[i - 1].Id}");
                }
            }
        }

        private bool VehicleIsCloseToFront(Vehicle vehicle, Vehicle front)
        {
            float length = vehicle.ModelSize.z / 2f + front.ModelSize.z / 2f;
            length *= 1.5f;

            float distance = Vector3.Distance(front.model.position, vehicle.model.position);
            return distance < length;
        }

        private void HandleVehicleEntered(object sender, Vehicle vehicle) { }

        private void HandleVehicleExited(object sender, Vehicle vehicle) { }

        private void HandleVehicleNodeChange(object sender, Node node)
        {
            if (sender is Vehicle vehicle)
            {
                if (node == this)
                // Entered this node
                {
                    Directions direction = vehicle.direction;

                    if (direction == Directions.Invalid)
                    {
                        Console.Console.Run($"log_error Vehicle {vehicle.Id} can't enter node {Id} from node {vehicle.previousNode}");
                        return;
                    }
                    vehicles[direction].Add(vehicle);
                    OnVehicleEntered?.Invoke(this, vehicle);
                }
                // Exited this node
                else if (vehicle.previousNode == Id)
                {
                    Directions direction = vehicle.previousDirection;

                    if (direction == Directions.Invalid)
                    {
                        Console.Console.Run($"log_error Vehicle {vehicle.Id} can't exit node {Id} to node {vehicle.previousNode}");
                        return;
                    }

                    vehicles[direction].Remove(vehicle);
                    OnVehicleExited?.Invoke(this, vehicle);
                }
            }
        }

        public static Directions GetDirection(Node to, int from)
        {
            if (to.NorthNode == from)
                return Directions.South;
            if (to.SouthNode == from)
                return Directions.North;
            if (to.WestNode == from)
                return Directions.East;
            if(to.EastNode == from)
                return Directions.West;
            return Directions.Invalid;
        }

        public static Directions ReverseDirection(Directions direction)
        {
            return direction switch
            {
                Directions.North => Directions.South,
                Directions.South => Directions.North,
                Directions.West => Directions.East,
                Directions.East => Directions.West,
                _ => Directions.Invalid,
            };
        }
    }
}

