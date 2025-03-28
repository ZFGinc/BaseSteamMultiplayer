using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Multiplayer")
        {
            if (PlayerModel.activeSelf == false)
            {
                SetPosition();
                PlayerModel.SetActive(true);
            }
            if (hasAuthority) Movement();
        }
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5,5), 0.8f, Random.Range(-5, 5));
    }

    public void Movement()
    {
        float xDir = Input.GetAxis("Horizontal");
        float yDir = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(xDir, 0f, yDir);
        transform.position += moveDir * Speed;
    }
}
