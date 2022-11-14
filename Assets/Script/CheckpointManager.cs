using UnityEngine;

public class CheckpointManager : MonoBehaviour, ITriggerable
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

    public void ActivateTrigger(bool status)
    {
        IsActive = status;
        if (status)
        {
            TriggerAction();
        }
    }

    public void TriggerAction()
    {
        gameManager.SetRespawnPoint(gameObject.transform.position);
        var checkpointRanderer = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        checkpointRanderer.material.SetColor("_Color", Color.green);
    }
}
