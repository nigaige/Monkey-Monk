using UnityEngine;

public class CheckpointManager : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;
    public bool IsActive { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        IsActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsActive == false)
        {
            IsActive = true;
            gameManager.SetRespawnPoint(gameObject.transform.position);
            var checkpointRanderer = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
            checkpointRanderer.material.SetColor("_Color", Color.green);
        }
    }
}
