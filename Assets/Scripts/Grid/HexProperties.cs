using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Grid {
    public class HexProperties {
        private BuildingType buildingType;
        public Vector3 worldPosition;
        private int rotation;
        private int multiHexDirection;
        private int aoeRange;
        private Hex mainCoordinates;
        public Vector3 mainPosition;
        private HashSet<Hex> connectedVillagesInRange = new();

        public bool IsPizzeria { get; private set; }

        public void SetPizzeria(bool pizzeria) => IsPizzeria = pizzeria;

        public BuildingType GetBuildingType() => buildingType;
        public void SetBuildingType(BuildingType buildingTypeIn) => buildingType = buildingTypeIn;

        public int GetRotation() => rotation;
        public void SetRotation(int rotationIn) => rotation = rotationIn;

        public int GetMultiHexDirection() => multiHexDirection;
        public void SetMultiHexDirection(int multiHexDirectionIn) => multiHexDirection = multiHexDirectionIn;

        public int GetAOERange() => aoeRange;
        public void SetAOERange(int aoeRangeIn) => aoeRange = aoeRangeIn;

        public Hex GetMainCoordinates() => mainCoordinates;
        public void SetMainCoordinates(Hex mainCoordinatesIn) => mainCoordinates = mainCoordinatesIn;

        public HashSet<Hex> GetConnectedHexesInRange() => connectedVillagesInRange;

        public void SetConnectedHexesInRange(HashSet<Hex> inputConnectedHexesInRange) =>
            connectedVillagesInRange = inputConnectedHexesInRange;

        public void AddToConnectedVillagesInRange(Hex hex) => connectedVillagesInRange.Add(hex);
        public void RemoveFromConnectedVillagesInRange(Hex hex) => connectedVillagesInRange.Remove(hex);
    }
}