using UnityEngine;
using UnityEngine.UI;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    private Transform target;
    bool canStart = false;
    bool enableTargetSprite = false;

    public void SelectTarget(Transform Target, float newSpeed = 50, bool EnableTargetSprite = false)
    {
        target = Target;
        canStart = true;
        speed = newSpeed;
        enableTargetSprite = EnableTargetSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(!canStart)
        {
            return;
        }

        if (Vector2.Distance(transform.position, target.position) > 3)
        {
            Vector3 newPos = Vector2.MoveTowards(transform.position,
                target.position, speed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
        else
        {
            if(enableTargetSprite)
            {
                target.GetComponent<Image>().enabled = true;
            }

            Destroy(this.gameObject);
        }
    }
}
