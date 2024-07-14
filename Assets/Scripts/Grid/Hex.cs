using System;
using System.Collections.Generic;
using Grid;

public class Hex {
    public int q_;
    public int r_;
    public int s_;
    private bool[] connections = new bool[6];
    private HexProperties properties;
    private HexWaypoints hexWaypoints;

    public Hex(int q, int r, int s) {
        if (q + r + s != 0) throw new ArgumentException("Invalid hex coordinates: q + r + s must be 0.");
        q_ = q;
        r_ = r;
        s_ = s;
        properties = new HexProperties();
        hexWaypoints = new HexWaypoints();
        properties.SetMainCoordinates(this);
    }

    public HexProperties GetHexProperties() => properties;
    public void SetHexProperties(HexProperties inputProperties) => properties = inputProperties;
    public HexWaypoints GetHexWaypoints() => hexWaypoints;
    public void SetHexWaypoints(HexWaypoints inputWaypoints) => hexWaypoints = inputWaypoints;

    public void UpdateConnectedHexesInRange(List<Hex> connHexes) {
        if (properties.GetAOERange() == 0) return;
        foreach (var villageHex in connHexes) {
            if (properties.GetConnectedHexesInRange().Contains(villageHex)) continue;
            if (properties.GetAOERange() > HexDistance(this, villageHex)) {
                properties.AddToConnectedVillagesInRange(villageHex);
            }
        }
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

    public static int HexLength(Hex hex) {
        return (Math.Abs(hex.q_) + Math.Abs(hex.r_) + Math.Abs(hex.s_)) / 2;
    }

    public static int HexDistance(Hex a, Hex b) {
        return HexLength(HexSubtract(a, b));
    }

    public static readonly List<Hex> HexDirections = new List<Hex> {
        new Hex(-1, 0, +1),
        new Hex(0, -1, +1),
        new Hex(+1, -1, 0),
        new Hex(+1, 0, -1),
        new Hex(0, +1, -1),
        new Hex(-1, +1, 0),
    };

    public static Hex HexDirection(int direction) {
        if (direction < 0 || direction >= HexDirections.Count) {
            throw new ArgumentOutOfRangeException(nameof(direction), "Direction must be between 0 and 5.");
        }

        return HexDirections[direction];
    }

    public static Hex HexNeighbor(Hex hex, int direction) {
        return HexAdd(hex, HexDirection(direction));
    }

    public static int GetDirection(Hex startHex, Hex endHex) {
        Hex directionHex = HexSubtract(startHex, endHex);
        for (int i = 0; i < HexDirections.Count; i++) {
            if (HexDirections[i] == directionHex) {
                return i;
            }
        }

        throw new ArgumentException("Hexes are not neighbors");
    }

    public void AddConnections(int[] edgeDirections) {
        for (int i = 0; i < edgeDirections.Length; i++) {
            if (edgeDirections[i] < 0 || edgeDirections[i] >= 6) {
                throw new ArgumentOutOfRangeException(nameof(edgeDirections),
                    "Edge direction must be between 0 and 5.");
            }

            connections[edgeDirections[i]] = true;
        }
    }

    public bool HasConnection(int edgeDirection) {
        if (edgeDirection < 0 || edgeDirection >= 6) {
            throw new ArgumentOutOfRangeException(nameof(edgeDirection), "Edge direction must be between 0 and 5.");
        }

        return connections[edgeDirection];
    }

    public static Hex RotateShoveHex(Hex centerHex, Hex hexToRotate, int rotation) {
        var vector = HexSubtract(hexToRotate, centerHex);
        for (int i = 0; i < rotation; i++) {
            vector = RotateShoveOnce(vector);
        }

        return HexAdd(centerHex, vector);
    }

    private static Hex RotateShoveOnce(Hex hex) {
        int q = hex.q_;
        int r = hex.r_;
        int s = hex.s_;

        int newQ = -r;
        int newR = -s;
        int newS = -q;

        return new Hex(newQ, newR, newS);
    }


    public static Point HexToPixel(Layout layout, Hex h) {
        Orientation M = layout.orientation;
        double x = (M.f0 * h.q_ + M.f1 * h.r_) * layout.size.x;
        double y = (M.f2 * h.q_ + M.f3 * h.r_) * layout.size.y;
        return new Point(x + layout.origin.x, y + layout.origin.y);
    }


    public Point HexCornerOffset(Layout layout, int corner) {
        Point size = layout.size;
        double angle = 2.0 * Math.PI * (layout.orientation.startAngle + corner) / 6;
        return new Point(size.x * Math.Cos(angle), size.y * Math.Sin(angle));
    }

    public List<Point> PolygonCorners(Layout layout, Hex h) {
        List<Point> corners = new List<Point>();
        Point center = HexToPixel(layout, h);
        for (int i = 0; i < 6; i++) {
            Point offset = HexCornerOffset(layout, i);
            corners.Add(new Point(center.x + offset.x, center.y + offset.y));
        }

        return corners;
    }

    public static FractionalHex PixelToHex(Layout layout, Point p) {
        Orientation M = layout.orientation;
        Point pt = new Point((p.x - layout.origin.x) / layout.size.x,
            (p.y - layout.origin.y) / layout.size.y);
        double q = M.b0 * pt.x + M.b1 * pt.y;
        double r = M.b2 * pt.x + M.b3 * pt.y;
        return new FractionalHex(q, r, -q - r);
    }

    public static Hex HexRound(FractionalHex h) {
        int q = (int)Math.Round(h.q);
        int r = (int)Math.Round(h.r);
        int s = (int)Math.Round(h.s);

        double q_diff = Math.Abs(q - h.q);
        double r_diff = Math.Abs(r - h.r);
        double s_diff = Math.Abs(s - h.s);

        if (q_diff > r_diff && q_diff > s_diff) {
            q = -r - s;
        }
        else if (r_diff > s_diff) {
            r = -q - s;
        }
        else {
            s = -q - r;
        }

        return new Hex(q, r, s);
    }
}