using System;
using UnityEngine;

public class MultiColliderChildTrigger : MonoBehaviour
{

    public Action<Collider> triggerEnter;
    public Action<Collider> triggerExit;

    private void OnTriggerEnter(Collider other)
    {
        triggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        triggerExit?.Invoke(other);
    }
}
