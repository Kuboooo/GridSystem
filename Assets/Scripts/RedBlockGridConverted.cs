using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBlockGridConverted
{

    public struct Hex {

        public int q_;
        public int r_;
        public int s_;

        public Hex(int q, int r, int s) {
            q_ = q;
            r_ = r;
            s_ = s;
        }

        public static bool operator ==(Hex a, Hex b) {
            return a.q_ == b.q_ && a.r_ == b.r_ && a.s_ == b.s_;
        }

        public static bool operator !=(Hex a, Hex b) {
            return !(a == b);
        }

        // You may also want to override Equals and GetHashCode methods
        // to ensure consistency with the equality operators.
        public override bool Equals(object obj) {
            if (obj is Hex other) {
                return this == other;
            }

            return false;
        }

        public override int GetHashCode() {
            return (q_, r_, s_).GetHashCode();
        }

        public static Hex HexAdd(Hex a, Hex b) {
            return new Hex(a.q_ + b.q_, a.r_ + b.r_, a.s_ + b.s_);
        }

        public static Hex HexSubtract(Hex a, Hex b) {
            return new Hex(a.q_ - b.q_, a.r_ - b.r_, a.s_ - b.s_);
        }

        public static Hex HexMultiply(Hex a, int k) {
            return new Hex(a.q_ * k, a.r_ * k, a.s_ * k);
        }
        public static int HexLength(Hex hex)
        {
            return (Math.Abs(hex.q_) + Math.Abs(hex.r_) + Math.Abs(hex.s_)) / 2;
        }

        public static int HexDistance(Hex a, Hex b)
        {
            return HexLength(HexSubtract(a, b));
        }
        public static readonly List<Hex> HexDirections = new List<Hex>
        {
            new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1),
            new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)
        };

        public static Hex HexDirection(int direction)
        {
            if (direction < 0 || direction >= HexDirections.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "Direction must be between 0 and 5.");
            }
            return HexDirections[direction];
        }

        public static Hex HexNeighbor(Hex hex, int direction)
        {
            return HexAdd(hex, HexDirection(direction));
        }

        
    }
  public  struct Orientation
    {
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
            double startAngle_)
        {
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
        public static Orientation LayoutPointy()
        {
            double sqrt3 = Math.Sqrt(3.0);
            return new Orientation(sqrt3, sqrt3 / 2.0, 0.0, 3.0 / 2.0,
                sqrt3 / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0,
                0.5);
        }
    }
    
    
    public struct Layout
    {
        public readonly Orientation orientation;
        public readonly Point size;
        public readonly Point origin;

        public Layout(Orientation orientation_, Point size_, Point origin_)
        {
            orientation = orientation_;
            size = size_;
            origin = origin_;
        }
    }

    public struct Point
    {
        public readonly double x;
        public readonly double y;

        public Point(double x_, double y_)
        {
            x = x_;
            y = y_;
        }
    }
    public Point HexToPixel(Layout layout, Hex h)
    {
        Orientation M = layout.orientation;
        double x = (M.f0 * h.q_ + M.f1 * h.r_) * layout.size.x;
        double y = (M.f2 * h.q_+ M.f3 * h.r_) * layout.size.y;
        return new Point(x + layout.origin.x, y + layout.origin.y);
    }
    
    // FractionalHex PixelToHex(Layout layout, Point p)
    // {
    //     Orientation M = layout.orientation;
    //     Point pt = new Point((p.x - layout.origin.x) / layout.size.x,
    //         (p.y - layout.origin.y) / layout.size.y);
    //     double q = M.b0 * pt.x + M.b1 * pt.y;
    //     double r = M.b2 * pt.x + M.b3 * pt.y;
    //     return new FractionalHex(q, r, -q - r);
    // }
   public Point HexCornerOffset(Layout layout, int corner)
    {
        Point size = layout.size;
        double angle = 2.0 * Math.PI * (layout.orientation.startAngle + corner) / 6;
        return new Point(size.x * Math.Cos(angle), size.y * Math.Sin(angle));
    }

    public List<Point> PolygonCorners(Layout layout, Hex h)
    {
        List<Point> corners = new List<Point>();
        Point center = HexToPixel(layout, h);
        for (int i = 0; i < 6; i++)
        {
            Point offset = HexCornerOffset(layout, i);
            corners.Add(new Point(center.x + offset.x, center.y + offset.y));
        }
        return corners;
    }
}
