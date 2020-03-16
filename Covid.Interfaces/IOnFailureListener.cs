using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Interfaces
{
    public interface IOnFailureListener
    {
        void OnFailure(Exception e);
    }
}
