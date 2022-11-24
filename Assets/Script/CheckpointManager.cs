using UnityEngine;

public class CheckpointManager : ATriggerable
{

    [SerializeField] private GameManager gameManager;
    public bool IsActive { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        IsActive = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsActive == false)
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
        gameManager.SetRespawnPoint(gameObject.transform.position);
        var checkpointRanderer = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        checkpointRanderer.material.SetColor("_Color", Color.green);
    }
}
