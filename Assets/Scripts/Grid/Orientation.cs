using System;

namespace Grid {
    public struct Orientation {

        public readonly double f0;
        public readonly double f1;
        public readonly double f2;
        public readonly double f3;
        public readonly double b0;
        public readonly double b1;
        public readonly double b2;
        public readonly double b3;
        public readonly double startAngle; // in multiples of 60°

        public Orientation(double f0_, double f1_, double f2_, double f3_,
            double b0_, double b1_, double b2_, double b3_,
            double startAngle_) {
            f0 = f0_;
            f1 = f1_;
            f2 = f2_;
            f3 = f3_;
            b0 = b0_;
            b1 = b1_;
            b2 = b2_;
            b3 = b3_;
            startAngle = startAngle_;
        }

        public static Orientation LayoutPointy() {
            double sqrt3 = Math.Sqrt(3.0);
            return new Orientation(sqrt3, sqrt3 / 2.0, 0.0, 3.0 / 2.0,
                sqrt3 / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0,
                0.5);
        }

    }
}