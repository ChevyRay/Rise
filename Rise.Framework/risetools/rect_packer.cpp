#include "rect_packer.hpp"
#include "extern_decl.h"
#include <algorithm>

extern "C"
{
    EXTERN_DECL rect_packer* new_packer(int capacity)
    {
        return new rect_packer(capacity);
    }
    
    EXTERN_DECL void free_packer(rect_packer* packer)
    {
        delete packer;
    }
    
    EXTERN_DECL void packer_init(rect_packer* packer, int w, int h)
    {
        packer->init(w, h);
    }
    
    EXTERN_DECL void packer_add(rect_packer* packer, int id, int w, int h, bool can_rotate)
    {
        packer->add(id, w, h, can_rotate);
    }
    
    EXTERN_DECL bool packer_pack(rect_packer* packer)
    {
        return packer->pack_nodes();
    }
    
    EXTERN_DECL void packer_get(rect_packer* packer, int index, int* id, int* x, int* y, int* w, int* h)
    {
        const packed_rect& rect = packer->packed[index];
        *id = rect.id;
        *x = rect.rect.x;
        *y = rect.rect.y;
        *w = rect.rect.w;
        *h = rect.rect.h;
    }
    
    EXTERN_DECL int packer_get_count(rect_packer* packer)
    {
        return (int)packer->packed.count;
    }
}

void rect_packer::init(int width, int height)
{
    nodes.clear();
    packed.clear();
    free.clear();
    free.add(recti(width, height));
}

void rect_packer::add(int id, int w, int h, bool can_rotate)
{
    pack_node node;
    node.id = id;
    node.w = w;
    node.h = h;
    node.can_rotate = can_rotate;
    nodes.add(node);
}

bool rect_packer::pack_nodes()
{
    indices.clear();
    for (size_t i = 0; i < nodes.count; ++i)
        indices.add(i);
    
    while (indices.count > 0)
    {
        //Track our best score for all rects
        int best_area = std::numeric_limits<int>::max();
        int best_short = std::numeric_limits<int>::max();
        size_t best_index = std::numeric_limits<size_t>::max();
        recti best_pos;
        
        //Find the highest scoring placement out of *all* our rects
        for (size_t i = 0; i < indices.count; ++i)
        {
            recti pos;
            int score_area, score_short;
            if (find_position(nodes[indices[i]], &pos, &score_area, &score_short))
            {
                if (score_area < best_area || (score_area == best_area && score_short < best_short))
                {
                    best_area = score_area;
                    best_short = score_short;
                    best_index = i;
                    best_pos = pos;
                }
            }
        }
        
        //If we couldn't find a node to pack, we've failed
        if (best_index == std::numeric_limits<size_t>::max())
        {
            nodes.clear();
            return false;
        }
        
        //Pack the node
        place_node(best_pos, nodes[indices[best_index]].id);
        indices.remove_at(best_index);
    }
    
    nodes.clear();
    return true;
}

bool rect_packer::find_position(const pack_node& node, recti* pos, int* best_area, int* best_short)
{
    *best_area = std::numeric_limits<int>::max();
    *best_short = std::numeric_limits<int>::max();
    pos->w = 0;
    
    int area = node.w * node.h;
    
    for (size_t i = 0; i < free.count; ++i)
    {
        int area_fit = free[i].w * free[i].h - area;
        if (area_fit <= *best_area)
        {
            //Try to place the rectangle in the free rect
            if (free[i].w >= node.w && free[i].h >= node.h)
            {
                int extra_x = std::abs(free[i].w - node.w);
                int extra_y = std::abs(free[i].h - node.h);
                int short_fit = std::min(extra_x, extra_y);
                if (area_fit < *best_area || (area_fit == *best_area && short_fit < *best_short))
                {
                    pos->x = free[i].x;
                    pos->y = free[i].y;
                    pos->w = node.w;
                    pos->h = node.h;
                    *best_area = area_fit;
                    *best_short = short_fit;
                }
            }
            
            //If we're allowed, also try to pack it rotated
            if (node.can_rotate && free[i].w >= node.h && free[i].h >= node.w)
            {
                int extra_x = std::abs(free[i].w - node.h);
                int extra_y = std::abs(free[i].h - node.w);
                int short_fit = std::min(extra_x, extra_y);
                if (area_fit < *best_area || (area_fit == *best_area && short_fit < *best_short))
                {
                    pos->x = free[i].x;
                    pos->y = free[i].y;
                    pos->w = node.h;
                    pos->h = node.w;
                    *best_area = area_fit;
                    *best_short = short_fit;
                }
            }
        }
    }
    
    return pos->w > 0;
}

void rect_packer::split_free_rect(recti free_rect, const recti& placed_rect)
{
    if (placed_rect.x < free_rect.x + free_rect.w && placed_rect.x + placed_rect.w > free_rect.x)
    {
        if (placed_rect.y > free_rect.y && placed_rect.y < free_rect.y + free_rect.h)
        {
            recti new_rect = free_rect;
            new_rect.h = placed_rect.y - new_rect.y;
            free.add(new_rect);
        }
        if (placed_rect.y + placed_rect.h < free_rect.y + free_rect.h)
        {
            recti new_rect = free_rect;
            new_rect.y = placed_rect.y + placed_rect.h;
            new_rect.h = free_rect.y + free_rect.h - (placed_rect.y + placed_rect.h);
            free.add(new_rect);
        }
    }
    
    if (placed_rect.y < free_rect.y + free_rect.h && placed_rect.y + placed_rect.h > free_rect.y)
    {
        if (placed_rect.x > free_rect.x && placed_rect.x < free_rect.x + free_rect.w)
        {
            recti new_rect = free_rect;
            new_rect.w = placed_rect.x - new_rect.x;
            free.add(new_rect);
        }
        if (placed_rect.x + placed_rect.w < free_rect.x + free_rect.w)
        {
            recti new_rect = free_rect;
            new_rect.x = placed_rect.x + placed_rect.w;
            new_rect.w = free_rect.x + free_rect.w - (placed_rect.x + placed_rect.w);
            free.add(new_rect);
        }
    }
}

void rect_packer::place_node(const recti& pos, int id)
{
    //Add the packed rect
    packed_rect rect(pos, id);
    packed.add(rect);
    
    //Split all free rectangles that contain the node
    size_t free_count = free.count;
    for (size_t i = 0; i < free_count; ++i)
    {
        if (free[i].overlaps(pos))
        {
            split_free_rect(free[i], pos);
            free.remove_at(i--);
            --free_count;
        }
    }
    
    //Prune the free list
    for (size_t i = 0; i < free.count; ++i)
    {
        for (size_t j = i + 1; j < free.count; ++j)
        {
            if (free[j].contains(free[i]))
            {
                free.remove_at(i--);
                break;
            }
            if (free[i].contains(free[j]))
                free.remove_at(j--);
        }
    }
}
