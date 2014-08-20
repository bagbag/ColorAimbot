namespace ColorAimbot.Helper
{
    public struct Point
    {
        internal double x;
        internal double y;

        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static explicit operator Vector(Point point)
        {
            return new Vector(point.x, point.y);
        }

        public static bool operator ==(Point point1, Point point2)
        {
            if (point1.X == point2.X)
                return point1.Y == point2.Y;
            else
                return false;
        }

        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        public static Point operator +(Point point, Vector vector)
        {
            return new Point(point.x + vector.x, point.y + vector.y);
        }

        public static Point operator -(Point point, Vector vector)
        {
            return new Point(point.x - vector.x, point.y - vector.y);
        }

        public static Vector operator -(Point point1, Point point2)
        {
            return new Vector(point1.x - point2.x, point1.y - point2.y);
        }

        public static bool Equals(Point point1, Point point2)
        {
            if (point1.X.Equals(point2.X))
                return point1.Y.Equals(point2.Y);
            else
                return false;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Point))
                return false;
            else
                return Equals(this, (Point)o);
        }

        public bool Equals(Point value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public void Offset(double offsetX, double offsetY)
        {
            x += offsetX;
            y += offsetY;
        }

        public static Point Add(Point point, Vector vector)
        {
            return new Point(point.x + vector.x, point.y + vector.y);
        }

        public static Point Subtract(Point point, Vector vector)
        {
            return new Point(point.x - vector.x, point.y - vector.y);
        }

        public static Vector Subtract(Point point1, Point point2)
        {
            return new Vector(point1.x - point2.x, point1.y - point2.y);
        }
    }
}
