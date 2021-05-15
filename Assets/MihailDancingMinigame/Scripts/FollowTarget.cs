using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameManager.Instance.endPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, target.position) > 1)
        {
            Vector3 newPos = Vector2.MoveTowards(transform.position,
                target.position, speed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
