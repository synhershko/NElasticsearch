using System;

namespace NElasticsearch.Helpers
{
    /// <summary>
    /// A helper class that translate between Disposable and Action
    /// 
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Raven.Abstractions.Extensions.DisposableAction"/> class.
        /// 
        /// </summary>
        /// <param name="action">The action.</param>
        public DisposableAction(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Execute the relevant actions
        /// 
        /// </summary>
        public void Dispose()
        {
            this.action();
        }
    }
}
