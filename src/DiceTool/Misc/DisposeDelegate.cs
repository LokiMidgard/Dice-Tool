using System;

namespace Dice.Misc
{
    internal class DisposeDelegate : IDisposable
    {
        private Func<bool> p;

        public DisposeDelegate(Func<bool> p)
        {
            this.p = p;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                    this.p();
                this.disposedValue = true;
            }
        }
        public void Dispose() => this.Dispose(true);
        #endregion
    }
}