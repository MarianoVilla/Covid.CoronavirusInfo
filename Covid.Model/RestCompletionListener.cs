using Covid.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class RestCompletionListener : IOnSuccessListener, IOnFailureListener
    {
        public event EventHandler<object> Success;
        public event EventHandler<Exception> Failure;
        public RestCompletionListener(EventHandler<object> SuccessMethod, EventHandler<Exception> FailureMethod)
        {
            Success = SuccessMethod;
            Failure = FailureMethod;
        }
        public void OnFailure(Exception e)
        {
            Failure?.Invoke(this, e);
        }

        public void OnSuccess(object Result)
        {
            Success?.Invoke(this, Result);
        }
    }
}
