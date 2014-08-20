using System;

namespace ColorAimbot.Helper
{
    public struct Vector
    {
        private double x;
        private double y;

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

        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }

        public double LengthSquared
        {
            get
            {
                return x * x + y * y;
            }
        }

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static explicit operator Point(Vector vector)
        {
            return new Point(vector.x, vector.y);
        }

        public static bool operator ==(Vector vector1, Vector vector2)
        {
            if (vector1.X == vector2.X)
                return vector1.Y == vector2.Y;
            else
                return false;
        }

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.x, -vector.y);
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x + vector2.x, vector1.y + vector2.y);
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x - vector2.x, vector1.y - vector2.y);
        }

        public static Point operator +(Vector vector, Point point)
        {
            return new Point(point.x + vector.x, point.y + vector.y);
        }

        public static Vector operator *(Vector vector, double scalar)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector operator *(double scalar, Vector vector)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector operator /(Vector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        public static double operator *(Vector vector1, Vector vector2)
        {
            return vector1.x * vector2.x + vector1.y * vector2.y;
        }

        public static bool Equals(Vector vector1, Vector vector2)
        {
            if (vector1.X.Equals(vector2.X))
                return vector1.Y.Equals(vector2.Y);
            else
                return false;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Vector))
                return false;
            else
                return Equals(this, (Vector)o);
        }

        public bool Equals(Vector value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public void Normalize()
        {
            this /= Math.Max(Math.Abs(x), Math.Abs(y));
            this /= Length;
        }

        public static double CrossProduct(Vector vector1, Vector vector2)
        {
            return vector1.x * vector2.y - vector1.y * vector2.x;
        }

        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            return Math.Atan2(vector1.x * vector2.y - vector2.x * vector1.y, vector1.x * vector2.x + vector1.y * vector2.y) * 57.2957795130823;
        }

        public void Negate()
        {
            x = -x;
            y = -y;
        }

        public static Vector Add(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x + vector2.x, vector1.y + vector2.y);
        }

        public static Vector Subtract(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x - vector2.x, vector1.y - vector2.y);
        }

        public static Point Add(Vector vector, Point point)
        {
            return new Point(point.x + vector.x, point.y + vector.y);
        }

        public static Vector Multiply(Vector vector, double scalar)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector Multiply(double scalar, Vector vector)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector Divide(Vector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        public static double Multiply(Vector vector1, Vector vector2)
        {
            return vector1.x * vector2.x + vector1.y * vector2.y;
        }

        public static double Determinant(Vector vector1, Vector vector2)
        {
            return vector1.x * vector2.y - vector1.y * vector2.x;
        }
    }
}
