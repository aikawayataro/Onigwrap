#include "onigwrap.h"
#include <stddef.h>

regex_t *
onigwrap_create(const char *pattern, int len, int ignoreCase, int multiline) {
    regex_t *reg = NULL;

    OnigErrorInfo einfo;

    OnigOptionType onigOptions = ONIG_OPTION_NONE | ONIG_OPTION_CAPTURE_GROUP;

    if (ignoreCase == 1)
        onigOptions |= ONIG_OPTION_IGNORECASE;

    if (multiline == 1)
        onigOptions |= ONIG_OPTION_MULTILINE;

    const OnigUChar *stringStart = (const void *)pattern;
    const OnigUChar *stringEnd = stringStart + len;

    onig_new(
        &reg, stringStart, stringEnd, onigOptions, ONIG_ENCODING_UTF16_LE,
        ONIG_SYNTAX_DEFAULT, &einfo
    );

    return reg;
}

OnigRegion *onigwrap_region_new(void) {
    return onig_region_new();
}

void onigwrap_region_free(OnigRegion *region) {
    onig_region_free(region, 1);
}

void onigwrap_free(regex_t *reg) {
    onig_free(reg);
}

int onigwrap_search(
    regex_t *reg, const char *charPtr, int offset, int length,
    OnigRegion *region
) {
    const OnigUChar *stringStart = (const void *)charPtr;
    const OnigUChar *stringEnd = (const void *)(charPtr + length);
    const OnigUChar *stringOffset = (const void *)(charPtr + offset);
    const OnigUChar *stringRange = stringEnd;

    int result = onig_search(
        reg, stringStart, stringEnd, stringOffset, stringRange, region,
        ONIG_OPTION_NONE
    );
    return result;
}

int onigwrap_num_regs(const OnigRegion *region) {
    return region->num_regs;
}

int onigwrap_pos(const OnigRegion *region, int nth) {
    if (nth < region->num_regs) {
        int result = region->beg[nth];
        if (result < 0)
            return result;
        return result / 2;
    }
    return -1;
}

int onigwrap_len(const OnigRegion *region, int nth) {
    if (nth < region->num_regs) {
        int result = region->end[nth] - region->beg[nth];
        return result / 2;
    }
    return -2;
}
