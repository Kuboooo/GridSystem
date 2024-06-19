using static Hex;

namespace DefaultNamespace {
    public class HexDirectionBuilder {
        private Hex resultHex;

        public HexDirectionBuilder() {
            resultHex = new Hex(0, 0, 0);
        }

        public HexDirectionBuilder Right() {
            resultHex = Hex.HexAdd(resultHex, new Hex(-1, 0, +1));
            return this;
        }

        public HexDirectionBuilder TopRight() {
            resultHex = Hex.HexAdd(resultHex, new Hex(0, -1, +1));
            return this;
        }


        public HexDirectionBuilder TopLeft() {
            resultHex = Hex.HexAdd(resultHex, new Hex(+1, -1, 0));
            return this;
        }

        public HexDirectionBuilder Left() {
            resultHex = Hex.HexAdd(resultHex, new Hex(+1, 0, -1));
            return this;
        }

        public HexDirectionBuilder BottomLeft() {
            resultHex = Hex.HexAdd(resultHex, new Hex(0, +1, -1));
            return this;
        }

        public HexDirectionBuilder BottomRight() {
            resultHex = Hex.HexAdd(resultHex, new Hex(-1, +1, 0));
            return this;
        }

        public Hex Build() {
            return resultHex;
        }

        public Hex BuildAndReset() {
            Hex result = resultHex;
            resultHex = new Hex(0, 0, 0);
            return result;
        }
    }
}