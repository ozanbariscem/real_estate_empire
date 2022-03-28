using System;
using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Traffic
{
    [MoonSharpUserData]
    public class TrafficManager : Manager.Manager
    {
        public static TrafficManager Instance { get; private set; }

        public event EventHandler OnNodesLoaded;
        public event EventHandler OnNodeTransformsLoaded;
        public event EventHandler OnNodesDataLoaded;
        public event EventHandler OnVehiclesLoaded;

        public event EventHandler OnVehiclesSet;

        public List<Node> nodeList;

        public Dictionary<Types, List<Node>> nodes;
        public Dictionary<Types, List<Transform>> vehicles;
        public Dictionary<Types, List<Transform>> nodeTransforms;

        public Dictionary<Types, Transform> vehicleParents;

        private bool pause;

        private float updateInterval;
        private float lastUpdate;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Update()
        {
            if (pause) return;
            if (UnityEngine.Time.time - lastUpdate < updateInterval) return;
            if (nodeList == null) return;

            foreach (var node in nodeList)
            {
                node.Move(updateInterval);
                lastUpdate = UnityEngine.Time.time;
            }
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "traffic/manager.lua";
            pause = true;
            updateInterval = 1 / 30f;

            LoadScript();
            GetModelSizes();
            LoadNodes();
            RaiseOnRulesLoaded();
            SetVehicles();
        }

        private void GetModelSizes()
        {
            Table modelSizeTable = script.Globals.Get("modelSize").Table;
            float[] modelSizes = new float[modelSizeTable.Length];

            int i = 0;
            foreach (var value in modelSizeTable.Values)
            {
                modelSizes[i] = (float)(value.Number);
                i++;
            }
            Node.modelSizes = modelSizes;
        }

        public void HandleMapLoaded(object sender, Transform map)
        {
            vehicleParents = new Dictionary<Types, Transform>();
            vehicleParents.Add(Types.Sea, map.Find("Paths/Sea/Vehicles"));
            vehicleParents.Add(Types.Air, map.Find("Paths/Air/Vehicles"));
            vehicleParents.Add(Types.Land, map.Find("Paths/Land/Vehicles"));

            nodeTransforms = new Dictionary<Types, List<Transform>>();
            nodeTransforms.Add(Types.Sea, LoadNodeTransforms(map, "Sea"));
            nodeTransforms.Add(Types.Air, LoadNodeTransforms(map, "Air"));
            nodeTransforms.Add(Types.Land, LoadNodeTransforms(map, "Land"));
            OnNodeTransformsLoaded?.Invoke(this, EventArgs.Empty);

            vehicles = new Dictionary<Types, List<Transform>>();
            vehicles.Add(Types.Sea, LoadVehicles(map, "Sea"));
            vehicles.Add(Types.Air, LoadVehicles(map, "Air"));
            vehicles.Add(Types.Land, LoadVehicles(map, "Land"));
            OnVehiclesLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void HandleCameraZoom(object sender, float zoom)
        {
            if (zoom < .2f)
            {
                updateInterval = 1 / 60f;
            }
            else if (zoom < .4f)
            {
                updateInterval = 1 / 30f;

                if (vehicleParents != null)
                    foreach (var key in vehicleParents.Keys)
                    {
                        vehicleParents[key].gameObject.SetActive(true);
                    }
            }
            else
            {
                updateInterval = float.MaxValue;

                if (vehicleParents != null)
                    foreach (var key in vehicleParents.Keys)
                    {
                        vehicleParents[key].gameObject.SetActive(false);
                    }
            }
        }

        private void SetVehicles()
        {
            DynValue variable = script.Globals.Get("vehicle_amounts");
            if (!variable.IsNil())
            {
                Table amounts = variable.Table;

                for (int i = 1; i < amounts.Length + 1; i++)
                {
                    Table table = amounts[i] as Table;

                    if (Enum.TryParse(table[1] as string, out Types type))
                    {
                        if (int.TryParse(table[2] as string, out var amount))
                        {
                            for (int j = 0; j < amount; j++)
                            {
                                GameObject parent = new GameObject("Parent");
                                parent.transform.SetParent(vehicleParents[type]);

                                List<Transform> vehicles = this.vehicles[type];
                                int random = UnityEngine.Random.Range(0, vehicles.Count);
                                GameObject mesh = Instantiate(vehicles[random].gameObject);
                                mesh.gameObject.SetActive(true);
                                mesh.transform.SetParent(parent.transform);

                                Vehicle vehicle = new Vehicle();
                                vehicle.model = mesh.transform;
                                vehicle.transform = parent.transform;
                                vehicle.type = type;
                                vehicle.Set(UnityEngine.Random.Range(0, nodeList.Count));
                            }
                        }
                    }
                }
            }

            OnVehiclesSet?.Invoke(this, EventArgs.Empty);
        }

        private void LoadNodes()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/traffic/nodes.json");
            
            if (json != null)
            {
                nodes = JsonConvert.DeserializeObject<Dictionary<Types, List<Node>>>(json);

                foreach (var key in nodes.Keys)
                {
                    foreach (var node in nodes[key])
                    {
                        node.transform = nodeTransforms[key][node.Id];
                    }
                }
            }

            nodeList = new List<Node>();

            foreach (var value in nodes.Values)
            {
                nodeList.AddRange(value);
            }

            OnNodesDataLoaded?.Invoke(this, EventArgs.Empty);
        }

        private List<Transform> LoadVehicles(Transform map, string type)
        {
            Transform vehicleTransform = map.Find($"Paths/{type}/Prefabs");

            List<Transform> vehicles = new List<Transform>();
            if (vehicleTransform != null)
            {
                foreach (Transform vehicle in vehicleTransform)
                {
                    vehicles.Add(vehicle);
                }
            }
            return vehicles;
        }
        
        private List<Transform> LoadNodeTransforms(Transform map, string type)
        {
            Transform transform = map.Find($"Paths/{type}/Nodes");
            if (transform == null) return null;

            transform.gameObject.SetActive(false);
            List<Transform> transforms = new List<Transform>();
            foreach (Transform node in transform)
            {
                if (int.TryParse(node.name, out var id))
                {
                    if (id == transforms.Count)
                        transforms.Add(node);
                    else
                    {
                        Console.Console.Run("log_error ID mismatch while trying to assing node transforms. Make sure path nodes are ordered according to their ids.");
                        return null;
                    }
                }
            }

            OnNodesLoaded?.Invoke(this, EventArgs.Empty);
            return transforms;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            Map.MapManager.Instance.OnMapLoaded += HandleMapLoaded;
            REE.Camera.Camera.Singleton.OnCameraZoomed += HandleCameraZoom;
            Time.TimeManager.Instance.OnResumed += (sender, date) => { pause = false; };
            Time.TimeManager.Instance.OnPaused += (sender, date) => { pause = true; };
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Map.MapManager.Instance.OnMapLoaded -= HandleMapLoaded;
            REE.Camera.Camera.Singleton.OnCameraZoomed -= HandleCameraZoom;
        }
    }
}
