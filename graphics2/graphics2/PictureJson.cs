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
       // public point[] Points;
        public line[] Lines;
        public circle[] Circles;
        public curve[] Curves;
        public poligon[] Poligon;

        public PictureJson(int numPoints, int numLines, int numCircles, int numCurves, int numPoli)
        {
            //Points = new point[numPoints];
            Lines = new line[numLines];
            Circles = new circle[numCircles];
            Curves = new curve[numCurves];
            Poligon = new poligon[numPoli];
        }

        public PictureJson(PictureJson other)
        {
           // Points = other.Points;
            Lines = other.Lines;
            Circles = other.Circles;
            Curves = other.Curves;
            Poligon = other.Poligon;
        }
    }

    [Serializable]
    public struct point
    {
        public double x;
        public double y;

        public point(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }

    [Serializable]
    public struct centerPoint
    {
        public double x;
        public double y;

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
        public double radius;

        public circle(centerPoint c, float rad)
        {
            center = c;
            radius = rad;
        }
    }

    [Serializable]
    public struct poligon
    {
        public centerPoint center;
        public point radius;
        public int polis;

        public poligon(centerPoint c, point rad, int number)
        {
            center = c;
            radius = rad;
            polis = number;
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
