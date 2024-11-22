using System;

public interface IHasProgress
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public float ProgressNormalized { get; }

        public ProgressChangedEventArgs(float progressNormalized)
        {
            ProgressNormalized = progressNormalized;
        }
    }

    public abstract event EventHandler<ProgressChangedEventArgs> OnProgressChanged;
}
