# Onigwrap

Based on [TextMateSharp](https://github.com/danipen/TextMateSharp)'s version of [onigwrap](https://github.com/fluentCODE/onigwrap).

### Differences from the source project:

 - Better native assets handling.
 - Use finalizers to free up unmanaged resources. I believe `IDisposable` is overkill here because Oniguruma regexes are not resource hungry, just GC makes this job easier.
