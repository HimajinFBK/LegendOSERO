using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    void Start()
    {
        SceneManager.sceneLoaded += CreatePlayerManager;
    }
    void CreatePlayerManager(Scene s , LoadSceneMode l)
    {
        if(s.buildIndex==1)
        PhotonNetwork.Instantiate(Path.Combine("PlayerManager"), Vector2.zero, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
