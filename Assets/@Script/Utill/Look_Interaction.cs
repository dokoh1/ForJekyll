using UnityEngine;

public class Look_Interaction : MonoBehaviour
{
    public Transform interaction_image;
    public GameObject nextObj;
    public Transform player;

    private void Start()
    {
         player = GameManager.Instance.Player.transform;
    }
    void Update()
    {
        interaction_image.transform.rotation = player.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (interaction_image != null)
        {
            if (nextObj != null)
            {
                nextObj.SetActive(true);
            }
            Destroy(this.gameObject);
        }
    }
}
