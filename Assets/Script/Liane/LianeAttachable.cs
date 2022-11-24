using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LianeAttachable : MonoBehaviour
{
    public virtual void OnAttach()
    {
        foreach (ILianeAttachModifier modifier in GetComponents<ILianeAttachModifier>())
        {
            modifier.OnAttach();
        }
    }

    public virtual void OnDetach()
    {
        foreach (ILianeAttachModifier modifier in GetComponents<ILianeAttachModifier>())
        {
            modifier.OnDetach();
        }
    }

    public bool IsAttachable()
    {
        foreach (ILianeAttachModifier modifier in GetComponents<ILianeAttachModifier>())
        {
            if (!modifier.IsAttachable()) return false;
        }

        return true;
    }

}
