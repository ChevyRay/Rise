using System;
using System.Collections.Generic;
namespace Rise
{
    public interface IShape
    {
        bool Contains(Vector2 point);
        void GetBounds(out Rectangle result);
        void Project(Vector2 axis, out float min, out float max);
        Vector2 Project(Vector2 point);
        bool Raycast(ref Ray ray);
        bool Raycast(ref Ray ray, out float dist);
        bool Raycast(ref Ray ray, out RayHit hit);
    }
}
