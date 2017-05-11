using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphics2
{
    [Serializable]
    class PictureJson
    {
        public List<point> Points;
        public List<line> Lines;
        public List<circle> Circles;
        public List<curve> Curves;
    }

    [Serializable]
    public struct point
    {
        public float x;
        public float y;

        public point(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }

    [Serializable]
    public struct centerPoint
    {
        public float x;
        public float y;

        public centerPoint(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }

    [Serializable]
    public struct line
    {
        public point first;
        public point second;

        public line(point one, point two)
        {
            first = one;
            second = two;
        }
    }

    [Serializable]
    public struct circle
    {
        public centerPoint center;
        public float radius;

        public circle(centerPoint c, float rad)
        {
            center = c;
            radius = rad;
        }
    }

    [Serializable]
    public struct curve
    {
        public point first;
        public point second;
        public point thired;
        public point fourth;

        public curve(point one, point two, point three, point four)
        {
            first = one;
            second = two;
            thired = three;
            fourth = four;
        }
    }
}
