using System;
using System.Collections.Generic;
namespace Rise.Entities
{
    public static class IListExtensions
    {
        public static void Shuffle<T>(this IList<T> values)
        {
            if (values.Count > 1)
            {
                int n = values.Count;
                int k;
                while (n > 1)
                {
                    n--;
                    k = Rand.Int(n + 1);
                    T value = values[k];
                    values[k] = values[n];
                    values[n] = value;
                }
            }
        }

        public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list.Count > 0)
            {
                T temp;
                int j;
                for (var i = 0; i < list.Count - 1; ++i)
                {
                    j = i + 1;
                    while (j > 0 && comparison(list[j - 1], list[j]) > 0)
                    {
                        temp = list[j - 1];
                        list[j - 1] = list[j];
                        list[j--] = temp;
                    }
                }
            }
        }
        public static void InsertionSort<T>(this IList<T> list) where T : IComparable
        {
            if (list.Count > 0)
            {
                T temp;
                int j;
                for (var i = 0; i < list.Count - 1; ++i)
                {
                    j = i + 1;
                    while (j > 0 && list[j - 1].CompareTo(list[j]) > 0)
                    {
                        temp = list[j - 1];
                        list[j - 1] = list[j];
                        list[j--] = temp;
                    }
                }
            }
        }

        //This is not threadsafe, but I don't want to create a new array every time MergeSort is called??
        static int[] mergeTemp = new int[64];

        public static void MergeSort(int[] input)
        {
            MergeSort(0, input.Length - 1);

            void MergeSort(int low, int high)
            {
                if (low < high)
                {
                    int middle = low / 2 + high / 2;
                    MergeSort(low, middle);
                    MergeSort(middle + 1, high);
                    Merge(low, middle, high);
                }

                void Merge(int a, int b, int c)
                {
                    int left = a;
                    int right = b + 1;
                    int count = (c - a) + 1;
                    int tempIndex = 0;

                    if (mergeTemp.Length < count)
                        Array.Resize(ref mergeTemp, count);

                    while ((left <= b) && (right <= c))
                    {
                        if (input[left] < input[right])
                            mergeTemp[tempIndex++] = input[left++];
                        else
                            mergeTemp[tempIndex++] = input[right++];
                    }

                    if (left <= b)
                        while (left <= b)
                            mergeTemp[tempIndex++] = input[left++];

                    if (right <= c)
                        while (right <= c)
                            mergeTemp[tempIndex++] = input[right++];

                    for (int i = 0; i < count; i++)
                        input[a + i] = mergeTemp[i];
                }
            }
        }
    }
}
