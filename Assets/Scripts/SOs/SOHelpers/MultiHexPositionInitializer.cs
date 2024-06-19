using System.Collections.Generic;
using DefaultNamespace;
using static Hex;

namespace SOs.SOHelpers {
    public class MultiHexPositionInitializer {
        public List<Hex> InitializeHospitalMultiHexPositionDirectionList() {
            var multiHexPositionDirectionList = new List<Hex>();
            HexDirectionBuilder builder = new HexDirectionBuilder();
            multiHexPositionDirectionList.Add(builder.Right().BuildAndReset());
            multiHexPositionDirectionList.Add(builder.TopRight().BuildAndReset());
            multiHexPositionDirectionList.Add(builder.Right().Right().BuildAndReset());
            multiHexPositionDirectionList.Add(builder.Right().TopRight().BuildAndReset());
            return multiHexPositionDirectionList;
        }
    }
}