#include <oniguruma.h>

#if defined(_WIN32)
#define ONIGWRAP_EXPORT __declspec(dllexport)
#else
#define ONIGWRAP_EXPORT
#endif

ONIGWRAP_EXPORT
regex_t *
onigwrap_create(const char *pattern, int len, int ignoreCase, int multiline);

ONIGWRAP_EXPORT
OnigRegion *onigwrap_region_new(void);

ONIGWRAP_EXPORT
void onigwrap_region_free(OnigRegion *region);

ONIGWRAP_EXPORT
void onigwrap_free(regex_t *reg);

ONIGWRAP_EXPORT
int onigwrap_search(
    regex_t *reg, const char *charPtr, int offset, int length,
    OnigRegion *region
);

ONIGWRAP_EXPORT
int onigwrap_num_regs(const OnigRegion *region);

ONIGWRAP_EXPORT
int onigwrap_pos(const OnigRegion *region, int nth);

ONIGWRAP_EXPORT
int onigwrap_len(const OnigRegion *region, int nth);
