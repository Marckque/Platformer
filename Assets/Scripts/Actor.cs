using UnityEngine;

public class Actor : MonoBehaviour
{
    [Header("Actor related")]
    [Header("Collider")]
    [SerializeField]
    private BoxCollider m_ActorCollider;
    private RayOrigins m_RayOrigins;

    [Header("Raycast management")]
    [SerializeField, Range(2, 10)]
    private int m_NumberOfRaycasts = 2;
    [SerializeField, Range(0f, 0.05f)]
    private float m_RaycastMargin = 0.02f;
    private float m_RaySpacing;

    protected void Start()
    {
        SetRaySpacing();        
    }

    public void Move(Vector3 velocity)
    {
        SetRaySpacing();
        SetRayOrigin(velocity);

        if (velocity.y != 0)
        {
            VerticalCollision(ref velocity);
        }

        if (velocity.x != 0)
        {
            HorizontalCollision(ref velocity);
        }
        
        transform.Translate(velocity);
    }

    private void SetRayOrigin(Vector3 velocity)
    {
        Bounds bounds = m_ActorCollider.bounds;

        Vector3 offset = new Vector3(velocity.x, velocity.y, 0f);

        m_RayOrigins.BottomLeft = new Vector3(bounds.min.x, bounds.min.y, 0f) + offset;
        m_RayOrigins.BottomRight = new Vector3(bounds.max.x, bounds.min.y, 0f) + offset;
        m_RayOrigins.TopLeft = new Vector3(bounds.min.x, bounds.max.y, 0f) + offset;
        m_RayOrigins.TopRight = new Vector3(bounds.max.x, bounds.max.y, 0f) + offset;
    }

    private void SetRaySpacing()
    {
        m_RaySpacing = m_ActorCollider.bounds.size.x / (m_NumberOfRaycasts - 1);
    }

    private void HorizontalCollision(ref Vector3 velocity)
    {
        float direction = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + m_RaycastMargin;
        Vector3 origin = (direction == -1) ? m_RayOrigins.TopLeft + (Vector3.up * direction * m_RaycastMargin) : m_RayOrigins.TopRight + (Vector3.up * -direction * m_RaycastMargin);

        for (int i = 0; i < m_NumberOfRaycasts; ++i)
        {
            Vector3 rayOrigin = origin +
                (Vector3.right * m_RaycastMargin * -direction) + // Makes raycasts start in the actor
                (Vector3.down * m_RaySpacing * (i - (m_RaycastMargin * i * 2f))); // Position of the new ray

            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.right * direction, out hit, rayLength))
            {
                if (hit.distance > 0)
                {
                    velocity.x = (hit.distance - m_RaycastMargin) * direction;
                    rayLength = hit.distance;
                }
            }

            Debug.DrawRay(rayOrigin, direction * Vector3.right * rayLength, Color.cyan);
        }
    }

    private void VerticalCollision(ref Vector3 velocity)
    {
        float direction = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + m_RaycastMargin;
        Vector3 origin = (direction == -1) ? m_RayOrigins.BottomLeft : m_RayOrigins.TopLeft;

        for (int i = 0; i < m_NumberOfRaycasts; ++i)
        {
            Vector3 rayOrigin = origin +
                (Vector3.right * m_RaycastMargin) + // Makes raycasts start in the actor
                (Vector3.up * -direction * m_RaycastMargin) + // Adds a little margin to the rays
                (Vector3.right * m_RaySpacing * (i - (m_RaycastMargin * i * 2f))); // Position of the new ray

            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.up * direction, out hit, rayLength))
            {
                if (hit.distance > 0)
                {
                    velocity.y = (hit.distance - m_RaycastMargin) * direction;
                    rayLength = hit.distance;
                }
            }

            Debug.DrawRay(rayOrigin, direction * Vector3.up * rayLength, Color.cyan);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // RayOrigins
            Gizmos.color = Color.red;
            float sphereSize = 0.1f;

            Gizmos.DrawSphere(m_RayOrigins.BottomLeft, sphereSize);
            Gizmos.DrawSphere(m_RayOrigins.BottomRight, sphereSize);
            Gizmos.DrawSphere(m_RayOrigins.TopLeft, sphereSize);
            Gizmos.DrawSphere(m_RayOrigins.TopRight, sphereSize);            
        }
    }
}

public struct RayOrigins
{
    public Vector3 BottomLeft;
    public Vector3 BottomRight;
    public Vector3 TopLeft;
    public Vector3 TopRight;
}