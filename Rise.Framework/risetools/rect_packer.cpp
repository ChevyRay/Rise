#include "rect_packer.hpp"
#include "extern_decl.h"
#include <algorithm>

extern "C"
{
    EXTERN_DECL rect_packer* new_packer()
    {
        return new rect_packer();
    }
    
    EXTERN_DECL void free_packer(rect_packer* packer)
    {
        delete packer;
    }
    
    EXTERN_DECL void packer_init(rect_packer* packer, int width, int height)
    {
        packer->init(width, height);
    }
    
    EXTERN_DECL void packer_add(rect_packer* packer, int id, int w, int h, bool can_rotate)
    {
        packer->add(id, w, h, can_rotate);
    }
    
    EXTERN_DECL int packer_get_count(rect_packer* packer)
    {
        return (int)packer->packed.count;
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
}

void rect_packer::init(int width, int height)
{
    nodes.clear();
    packed.clear();
    free.clear();
    free.add(recti(width, height));
}

void rect_packer::add(int w, int h, int id, bool can_rotate)
{
    pack_node node;
    node.id = id;
    node.size.x = w;
    node.size.y = h;
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
        size_t best_index = SIZE_MAX;
        vec2i best_pos;
        
        //Find the highest scoring placement out of *all* our rects
        for (size_t i = 0; i < indices.count; ++i)
        {
            size_t index = indices[i];
            vec2i pos;
            int score_area, score_short;
            if (find_position(nodes[index], &pos, &score_area, &score_short))
            {
                if (score_area < best_area || (score_area == best_area && score_short < best_short))
                {
                    best_area = score_area;
                    best_short = score_short;
                    best_index = index;
                    best_pos = pos;
                }
            }
        }
        
        //If we found a good position for it, pack it
        if (best_index < SIZE_MAX)
            place_node(best_pos, nodes[best_index]);
        
        //Whether or not we packed it, remove it from the index list
        indices.remove_at(best_index);
    }
    
    nodes.clear();
    
    return indices.count == 0;
}

bool rect_packer::find_position(const pack_node& node, vec2i* pos, int* best_area, int* best_short)
{
    *best_area = std::numeric_limits<int>::max();
    *best_short = std::numeric_limits<int>::max();
    recti best_rect(0, 0);
    
    int w = node.size.x;
    int h = node.size.y;
    int area = w * h;
    
    for (size_t i = 0; i < free.count; ++i)
    {
        int area_fit = free[i].w * free[i].h - area;
        
        //Try to place the rectangle in the free rect
        if (free[i].w >= w && free[i].h >= h)
        {
            int extra_x = std::abs(free[i].w - w);
            int extra_y = std::abs(free[i].h - h);
            int short_fit = std::min(extra_x, extra_y);
            if (area_fit < *best_area || (area_fit == *best_area && short_fit < *best_short))
            {
                best_rect.x = free[i].x;
                best_rect.y = free[i].y;
                best_rect.w = w;
                best_rect.y = h;
                *best_area = area_fit;
                *best_short = short_fit;
            }
        }
        
        //If we're allowed, also try to pack it rotated
        if (node.can_rotate && free[i].w >= h && free[i].h >= w)
        {
            int extra_x = std::abs(free[i].w - h);
            int extra_y = std::abs(free[i].h - w);
            int short_fit = std::min(extra_x, extra_y);
            if (area_fit < *best_area || (area_fit == *best_area && short_fit < *best_short))
            {
                best_rect.x = free[i].x;
                best_rect.y = free[i].y;
                best_rect.w = h;
                best_rect.y = w;
                *best_area = area_fit;
                *best_short = short_fit;
            }
        }
    }
    
    return best_rect.w > 0;
}

void rect_packer::split_free_rect(const recti& free_rect, const recti& placed_rect)
{
    if (placed_rect.x < free_rect.x + free_rect.w && placed_rect.x + placed_rect.w > free_rect.x)
    {
        if (placed_rect.y > free_rect.y && placed_rect.y < free_rect.y + free_rect.h)
        {
            recti rect = free_rect;
            rect.h = placed_rect.y - rect.y;
            free.add(rect);
        }
        
        if (placed_rect.y + placed_rect.h < free_rect.y + free_rect.h)
        {
            recti rect = free_rect;
            rect.y = placed_rect.y + placed_rect.h;
            rect.h = free_rect.y + free_rect.h - (placed_rect.y + placed_rect.h);
            free.add(rect);
        }
    }
    
    if (placed_rect.y < free_rect.y + free_rect.h && placed_rect.y + placed_rect.h > free_rect.y)
    {
        if (placed_rect.x > free_rect.x && placed_rect.x < free_rect.x + free_rect.w)
        {
            recti rect = free_rect;
            rect.w = placed_rect.x - rect.x;
            free.add(rect);
        }
        
        if (placed_rect.x + placed_rect.w < free_rect.x + free_rect.w)
        {
            recti rect = free_rect;
            rect.x = placed_rect.x + placed_rect.w;
            rect.w = free_rect.x + free_rect.w - (placed_rect.x + placed_rect.w);
            free.add(rect);
        }
    }
}

void rect_packer::place_node(vec2i pos, const pack_node& node)
{
    //Add the packed rect
    packed_rect rect(pos, node);
    packed.add(rect);
    
    //Split all free rectangles that contain the node
    size_t free_count = free.count;
    for (size_t i = 0; i < free_count; ++i)
    {
        if (free[i].overlaps(rect.rect))
        {
            split_free_rect(free[i], rect.rect);
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
