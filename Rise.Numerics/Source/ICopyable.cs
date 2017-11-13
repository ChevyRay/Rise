using System;
namespace Rise
{
    public interface ICopyable<T>
    {
        void CopyTo(out T other);
    }
}
