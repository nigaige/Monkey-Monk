using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILianeAttachModifier
{
    public void OnAttach();
    public void OnDetach();
    public bool IsAttachable();
}
