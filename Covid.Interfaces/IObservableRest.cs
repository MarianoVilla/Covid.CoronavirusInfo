using System;
using System.Collections.Generic;

namespace Covid.Interfaces
{
    /// <summary>
    /// Describes the interface for a client-side observable rest consumer.
    /// Could've used .NET's <see cref="IObservable{T}"/> and <see cref="IObserver{T}"/>,
    /// but wanted to implement my own just for fun.
    /// Gotta keep those design patterns fresh!
    /// </summary>
    public interface IObservableRestConsumer
    {
        List<IOnSuccessListener> OnSuccessListeners { get; }
        List<IOnFailureListener> OnFailureListeners { get; }

        void AddOnSuccessListener(IOnSuccessListener Listener);
        void AddOnFailureListener(IOnFailureListener Listener);

        void NotifyFailure(Exception e);
        void NotifySuccess(object Result);



    }
}
