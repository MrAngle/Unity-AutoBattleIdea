using System;

namespace MageFactory.Shared.Model {
    public sealed class GridDimensions {
        private readonly int width;
        private readonly int height;

        public GridDimensions(int width, int height) {
            if (width < 1) {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than 0.");
            }

            if (height < 1) {
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than 0.");
            }

            this.width = width;
            this.height = height;
        }

        public int getWidth() {
            return width;
        }

        public int getHeight() {
            return height;
        }

        public bool canContain(GridDimensions other) {
            if (other == null) {
                return false;
            }

            return other.width <= width && other.height <= height;
        }
    }
}