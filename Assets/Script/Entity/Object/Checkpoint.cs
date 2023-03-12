using UnityEngine;

public class Checkpoint : ATriggerable
{
    void Start()
    {
        IsActive = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsActive == false && other.tag == "Player")
        {
            ActivateTrigger(true);
        }
    }

    public override void ActivateTrigger(bool status)
    {
        IsActive = status;
        if (status)
        {
            TriggerAction();
        }
    }

    public override void TriggerAction()
    {
        GameManager.Instance.SetRespawnPoint(gameObject.transform.position);
        var checkpointRanderer = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        checkpointRanderer.material.SetColor("_BaseColor", Color.green);
    }
}
