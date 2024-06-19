namespace Grid {
    public struct Layout {

        public readonly Orientation orientation;
        public readonly Point size;
        public readonly Point origin;

        public Layout(Orientation orientation_, Point size_, Point origin_) {
            orientation = orientation_;
            size = size_;
            origin = origin_;
        }

    }
}