using Newtonsoft.Json.Linq;
using UnityEngine;

public class Other_players : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 10f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void HandIePlayerMove(string message)
    {
        var data = JsonUtility.FromJson<MessageData>(message);
        PlayerState playerState = data.playerState;
        Debug.Log(data.playerState);
        JObject jsonObj = JObject.Parse(message);
        JObject positionObj = jsonObj["position"] as JObject;

        Vector3 position = new Vector3(
            positionObj["X"]?.Value<float>() ?? 0,
            positionObj["Y"]?.Value<float>() ?? 0,
            positionObj["Z"]?.Value<float>() ?? 0
        );

        targetPosition = position;
        targetRotation = Quaternion.Euler(0, data.rotation, 0);
        PlayerAnimationProces(playerState);
    }

    void PlayerAnimationProces(PlayerState playerState)
    {
        if (playerState.Junmp == "IsJump")
        {
            animator.SetBool("IsMove", false);
            animator.SetBool("IsJump", true);
            animator.SetBool("IsMove-Jump", true);
            animator.SetBool("IsLand", false);
            animator.SetBool("IsMove-Human", false);
            animator.SetBool("IsRun", false);
            animator.SetBool("IsSprint", false);
            animator.SetBool("IsWalk", false);
            return;
        }
        if (playerState.IsLand)
        {
            animator.SetBool("IsLand", true);
            switch (playerState.direct)
            {
                case "IsRun":
                    animator.SetBool("IsMove", true);
                    animator.SetBool("IsRun", true);
                    animator.SetBool("IsSprint", false);
                    animator.SetBool("IsWalk", false);
                    break;
                case "IsSprint":
                    animator.SetBool("IsMove", true);
                    animator.SetBool("IsRun", false);
                    animator.SetBool("IsSprint", true);
                    animator.SetBool("IsWalk", false);
                    break;
                case "IsWalk":
                    animator.SetBool("IsMove", true);
                    animator.SetBool("IsRun", false);
                    animator.SetBool("IsSprint", false);
                    animator.SetBool("IsWalk", true);
                    break;
                default:
                    animator.SetBool("IsMove", false);
                    animator.SetBool("IsRun", false);
                    animator.SetBool("IsSprint", false);
                    animator.SetBool("IsWalk", false);
                    break;
            } 
        }
        else
        {
            animator.SetBool("IsLand", false);
            animator.SetBool("IsMove", false);
            animator.SetBool("IsJump", false);
            animator.SetBool("IsMove-Jump", false);
            animator.SetBool("IsMove-Human", true);
        }
    }
}
