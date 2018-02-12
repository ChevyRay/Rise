#ifndef rect_packer_hpp
#define rect_packer_hpp
#include <memory>
#include <cassert>

struct recti
{
    int x;
    int y;
    int w;
    int h;
    inline recti() {}
    inline recti(int x, int y, int w, int h) : x(x), y(y), w(w), h(h) {}
    inline recti(int w, int h) : x(0), y(0), w(w), h(h) {}
    inline bool contains(const recti& rect) const
    {
        return rect.x >= x && rect.y >= y && rect.x + rect.w <= x + w && rect.y + rect.h <= y + h;
    }
    inline bool overlaps(const recti& rect) const
    {
        return x < rect.x + rect.w && y < rect.y + rect.h && x + w > rect.x && y + h > rect.y;
    }
};

template<typename T>
struct list
{
    T* items;
    size_t capacity;
    size_t count;
    
    list(size_t initCapacity)
    {
        items = (T*)std::malloc(sizeof(T) * initCapacity);
        capacity = initCapacity;
        count = 0;
    }
    ~list()
    {
        std::free(items);
    }
    inline const T& operator[](size_t i) const
    {
        assert(i < count);
        return items[i];
    }
    inline T& operator[](size_t i)
    {
        assert(i < count);
        return items[i];
    }
    inline void clear() { count = 0; }
    inline void add(const T& item)
    {
        if (count == capacity)
        {
            capacity *= 2;
            items = (T*)std::realloc(items, sizeof(T) * capacity);
        }
        items[count++] = item;
    }
    inline void remove_at(size_t index)
    {
        for (size_t i = index + 1; i < count; ++i)
            items[i - 1] = items[i];
        --count;
    }
    void sort(int (*compare)(const T& a, const T& b))
    {
        if (count > 1)
        {
            for (int i = 0; i < count - 1; ++i)
            {
                int j = i + 1;
                while (j > 0 && compare(items[j - 1], items[j]) > 0)
                {
                    std::swap(items[j - 1], items[j]);
                    --j;
                }
            }
        }
    }
};

struct pack_node
{
    int w;
    int h;
    int id;
    bool can_rotate;
    
    inline pack_node() {}
    inline pack_node(int w, int h, int id, bool can_rotate) : w(w), h(h), id(id), can_rotate(can_rotate && w != h) {}
};

struct packed_rect
{
    recti rect;
    int id;
    
    inline packed_rect() {}
    inline packed_rect(recti rect, int id) : rect(rect), id(id) {}
};

struct rect_packer
{
    list<pack_node> nodes;
    list<packed_rect> packed;
    list<recti> free;
    list<size_t> indices;
    bool find_position(const pack_node& node, recti* pos, int* best_area, int* best_short);
    void split_free_rect(recti free_rect, const recti& placed_rect);
    void place_node(const recti& pos, int id);
    rect_packer(size_t capacity) : nodes(capacity), packed(capacity), free(capacity), indices(capacity) {}
    ~rect_packer() {}
    void init(int width, int height);
    void add(int id, int w, int h, bool can_rotate);
    bool pack_nodes();
};

#endif
