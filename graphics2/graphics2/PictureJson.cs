using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphics2
{
    [Serializable]
    public class PictureJson
    {
        public point[] Points { set; get; }
        public line[] Lines;
        public circle[] Circles;
        public curve[] Curves;

        public PictureJson(int numPoints, int numLines, int numCircles, int numCurves)
        {
            Points = new point[numPoints];
            Lines = new line[numLines];
            Circles = new circle[numCircles];
            Curves = new curve[numCurves];
        }
    }

    [Serializable]
    public struct point
    {
        public float x { set; get; }
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
