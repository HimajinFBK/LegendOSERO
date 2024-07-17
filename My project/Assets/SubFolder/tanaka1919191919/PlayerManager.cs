using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class PlayerManager : MonoBehaviour
{
    PhotonView v;
    void Start()
    {
        v = new PhotonView();
        if (!v.IsMine)
            return;
        PhotonNetwork.Instantiate(Path.Combine("PlayerController"), Vector2.zero, Quaternion.identity);
    }
    private void Awake()
    {
        v = GetComponent<PhotonView>();
    }
  
}
