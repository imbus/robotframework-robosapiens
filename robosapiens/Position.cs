using System;

namespace RoboSAPiens {
    public struct Position {
        const int horizontalDeviation = 5;
        const int verticalDeviation = 5;
        const int verticalSeparation = 30;
        const int horizontalSeparation = 25;
        const int leftMargin = 3;
        const int rightMargin = 3;

        public int bottom {get;}
        public int left {get;}
        public int right {get;}
        public int top {get;}

        public Position(int height, int left, int top, int width) {
            bottom = top + height;
            this.left = left;
            right = left + width;
            this.top = top;
        }

        bool centerAlignedWith(Position other) {
            var symmetryAxis = left + (right - left)/2;
            var otherSymmetryAxis = other.left + (other.right - other.left)/2;

            return Math.Abs(symmetryAxis - otherSymmetryAxis) < horizontalDeviation;
        }

        bool horizontalNeighborOf(Position other) {
            if (right <= other.left) {
                return (other.left - right) <= horizontalSeparation;
            }

            if (other.right <= left) {
                return (left - other.right) <= horizontalSeparation;
            }

            return false;
        }

        bool leftAlignedWith(Position other) {
            return Math.Abs(left - other.left) < leftMargin;
        }

        bool rightAlignedWith(Position other) {
            return Math.Abs(right - other.right) < rightMargin;
        }

        bool verticalNeighborOf(Position other) {
            if (bottom < other.top) {
                return (other.top - bottom) < verticalSeparation;
            }

            if (other.bottom < top) {
                return (top - other.bottom) < verticalSeparation;
            }

            return false;
        }

        //   ________   ____________
        //  \ Label \  \ Component \
        //  \_______\  \___________\
        //
        //   ____________   ________
        //  \ Component \  \ Label \
        //  \___________\  \_______\
        //

        public bool horizontalAlignedWith(Position other) {
            var symmetryAxis = top + (bottom - top)/2;
            var otherSymmetryAxis = other.top + (other.bottom - other.top)/2;

            return Math.Abs(symmetryAxis - otherSymmetryAxis) < verticalDeviation;
        }

        //   ________
        //  \ Label \
        //  \_______\
        //   ____________
        //  \ Component \
        //  \___________\
        //

        public bool verticalAlignedWith(Position other) {
            return (left >= other.left && right <= other.right) || 
                   (other.left >= left && other.right <= right);
        }

        //   ________
        //  \ Label \
        //  \_______\
        //              ____________
        //             \ Component \
        //             \___________\
        //

        public bool verticalAndHorizontalNeighborOf(Position other) {
            return verticalNeighborOf(other) && horizontalNeighborOf(other);
        }
    }
}