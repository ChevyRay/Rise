#ifndef rect_packer_hpp
#define rect_packer_hpp
#include <memory>

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

struct vec2i
{
    int x;
    int y;
    inline vec2i() {}
    inline vec2i(int x, int y) : x(x), y(y) {}
};

template<typename T>
struct list
{
    T* items;
    size_t capacity;
    size_t count;
    
    list(int initCapacity)
    {
        items = (T*)std::malloc(sizeof(T) * initCapacity);
        capacity = initCapacity;
        count = 0;
    }
    ~list()
    {
        std::free(items);
    }
    inline const T& operator[](size_t i) const { return items[i]; }
    inline T& operator[](size_t i) { return items[i]; }
    inline void clear() { count = 0; }
    inline void add(const T& item)
    {
        if (count == capacity)
        {
            capacity *= 2;
            items = (T*)std::realloc(items, sizeof(T) * capacity);
        }
    }
    inline void remove_at(size_t index)
    {
        //for (size_t i = index + 1; i < count; ++i)
        //    items[i - 1] = items[i];
        size_t to = index * sizeof(T);
        size_t from = to + sizeof(T);
        size_t len = (count - (index + 1)) * sizeof(T);
        std::memmove(items + to, items + from, len);
        --count;
    }
};

struct pack_node
{
    vec2i size;
    int id;
    bool can_rotate;
    
    inline pack_node() {}
    inline pack_node(int w, int h, int id, bool can_rotate) : size(w, h), id(id), can_rotate(can_rotate) {}
};

struct packed_rect
{
    recti rect;
    int id;
    
    inline packed_rect() {}
    inline packed_rect(vec2i pos, const pack_node& node) : rect(pos.x, pos.y, node.size.x, node.size.y), id(node.id) {}
};

struct rect_packer
{
    list<pack_node> nodes;
    list<packed_rect> packed;
    list<recti> free;
    list<size_t> indices;
    bool find_position(const pack_node& node, vec2i* pos, int* best_area, int* best_short);
    void split_free_rect(const recti& free_rect, const recti& placed_rect);
    void place_node(vec2i pos, const pack_node& node);
    rect_packer() : nodes(4), packed(4), free(4), indices(4) {}
    ~rect_packer() {}
    void init(int width, int height);
    void add(int id, int w, int h, bool can_rotate);
    bool pack_nodes();
};

#endif
