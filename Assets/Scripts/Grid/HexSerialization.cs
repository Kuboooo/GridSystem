﻿using System.IO;
using Enums;
using UnityEngine;

namespace Grid {
    public class HexSerialization {
        public void Save(BinaryWriter writer, Hex coordinates, HexProperties properties) {
            writer.Write(coordinates.q_);
            writer.Write(coordinates.r_);
            writer.Write(coordinates.s_);
            writer.Write(properties.IsPizzeria);
            writer.Write(properties.worldPosition.x);
            writer.Write(properties.worldPosition.y);
            writer.Write(properties.worldPosition.z);
            writer.Write(properties.GetRotation());
            writer.Write(properties.GetBuildingType().ToString());
            writer.Write(properties.GetMultiHexDirection());
            writer.Write(properties.GetAOERange());
            writer.Write(properties.GetMainCoordinates().q_);
            writer.Write(properties.GetMainCoordinates().r_);
            writer.Write(properties.GetMainCoordinates().s_);
            writer.Write(properties.mainPosition.x);
            writer.Write(properties.mainPosition.y);
            writer.Write(properties.mainPosition.z);
        }

        public void Load(BinaryReader reader, Hex coordinates, HexProperties properties) {            
            coordinates.q_ = reader.ReadInt32();
            coordinates.r_ = reader.ReadInt32();
            coordinates.s_ = reader.ReadInt32();
            properties.SetPizzeria(reader.ReadBoolean());
            properties.worldPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            properties.SetRotation(reader.ReadInt32());
            properties.SetBuildingType((BuildingType)System.Enum.Parse(typeof(BuildingType), reader.ReadString()));
            properties.SetMultiHexDirection(reader.ReadInt32());
            properties.SetAOERange(reader.ReadInt32());
            properties.SetMainCoordinates(new Hex(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
            properties.mainPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}