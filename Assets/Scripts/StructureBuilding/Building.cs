using Enums;
using UnityEngine;

namespace StructureBuilding {
    public class Building {
        public BuildingType Type { get; private set; }
        public GameObject buildingInstance { get; private set; }

        public Hex buildingHex { get; set; }

        public Building(BuildingType type, GameObject buildingInstance, Hex buildingHex) {
            Type = type;
            this.buildingInstance = buildingInstance;
            this.buildingHex = buildingHex;
        }
    }
}