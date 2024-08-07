namespace Onigwrap
{
    public interface IOnigNextMatchResult
    {
        int GetIndex();
        IOnigCaptureIndex[] GetCaptureIndices();
    }
}
