using System;
using Enums;
using UnityEngine;

namespace Grid {
    public class HexProperties {
        
        private bool isPizzeria;
        private BuildingType buildingType;
        public Vector3 worldPosition;
        private int rotation;
        private int multiHexDirection;
        private int aoeRange;
        private Hex mainCoordinates;
        public Vector3 mainPosition;

        public bool IsPizzeria => isPizzeria;
        public void SetPizzeria(Boolean pizzeria) => isPizzeria = pizzeria;

        public BuildingType GetBuildingType() => buildingType;
        public void SetBuildingType(BuildingType buildingType) => this.buildingType = buildingType;

        public int GetRotation() => rotation;
        public void SetRotation(int rotation) => this.rotation = rotation;

        public int GetMultiHexDirection() => multiHexDirection;
        public void SetMultiHexDirection(int multiHexDirection) => this.multiHexDirection = multiHexDirection;

        public int GetAOERange() => aoeRange;
        public void SetAOERange(int aoeRange) => this.aoeRange = aoeRange;

        public Hex GetMainCoordinates() => mainCoordinates;
        public void SetMainCoordinates(Hex mainCoordinates) => this.mainCoordinates = mainCoordinates;

        public HexProperties() {
            rotation = 0;
            multiHexDirection = 0;
        }
    }
}