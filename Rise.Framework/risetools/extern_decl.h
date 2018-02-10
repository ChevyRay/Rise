#ifndef decl_h
#define decl_h

#ifdef _WIN32
#define EXTERN_DECL __declspec(dllexport)
#else
#define EXTERN_DECL extern
#endif

#endif
