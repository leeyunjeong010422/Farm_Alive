using Fusion;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using static QuestManager;

public class BoxTrigger : MonoBehaviourPun
{
    [SerializeField] public GameObject boxTape;
    [SerializeField] public List<RequiredItem> requiredItems;
    [SerializeField] public BoxCover boxCover;
    [SerializeField] public List<int> idList = new List<int>();
    [SerializeField] Collider openCollider;
    [SerializeField] Collider closedCollider;

    [SerializeField] public delegate void OnRequiredItemsChanged(List<RequiredItem> items);
    [SerializeField] public event OnRequiredItemsChanged RequiredItemsChanged;


    private void Start()
    {
        boxCover = GetComponent<BoxCover>();
    }

    private void OnEnable()
    {
        Debug.Log("리스트");
        requiredItems = new List<RequiredItem>();
        NotifyRequiredItemsChanged();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tape"))
        {
            Debug.Log("포장시작");
            if (boxCover.IsOpen)
                return;

            Taping taping = other.GetComponent<Taping>();
            if (taping != null && !boxCover.IsPackaged)
            {
                Debug.Log("상자테이핑시작준비");
                taping.StartTaping(this, this.boxCover);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tape"))
        {
            Taping taping = other.GetComponent<Taping>();
            if (taping != null)
            {
                taping.StopTaping();
            }
        }
    }

    public void CountUpdate(int viewId, bool isBool)
    {
        if (!isBool)
        {
            PhotonView itemView = PhotonView.Find(viewId);
            idList.Add(viewId);

            Rigidbody itemRigid = itemView.GetComponent<Rigidbody>();
            itemRigid.drag = 10;
            itemRigid.angularDrag = 1;
            Crop cropView = itemView.GetComponent<Crop>();
            if (requiredItems.Count > 0)
            {
                foreach (QuestManager.RequiredItem item in requiredItems)
                {
                    Debug.Log(requiredItems);
                    if (item.itemPrefab.name == itemView.gameObject.name)
                    {
                        item.requiredcount += cropView.Value;
                        Debug.Log("카운트업");
                        NotifyRequiredItemsChanged();
                        return;
                    }
                }
                requiredItems.Add(new RequiredItem(itemView.gameObject, cropView.Value));
            }
            else
            {
                requiredItems.Add(new RequiredItem(itemView.gameObject, cropView.Value));
            }

            NotifyRequiredItemsChanged();
        }
        else
        {
            PhotonView itemView = PhotonView.Find(viewId);

            Crop cropView = itemView.GetComponent<Crop>();
            if (requiredItems.Count > 0)
            {
                for (int i = requiredItems.Count - 1; i >= 0; i--)
                {
                    if (requiredItems[i].itemPrefab.name == itemView.gameObject.name)
                    {
                        requiredItems[i].requiredcount -= cropView.Value;

                        Rigidbody itemRigid = itemView.GetComponent<Rigidbody>();
                        itemRigid.drag = 0;
                        itemRigid.angularDrag = 0.05f;

                        if (requiredItems[i].requiredcount <= 0)
                        {
                            if (idList.Contains(itemView.ViewID))
                            {
                                idList.Remove(itemView.ViewID);
                                Debug.Log($"리스트에서 {itemView} 제거");
                            }
                            requiredItems.RemoveAt(i);
                        }

                        NotifyRequiredItemsChanged();
                        return;
                    }
                }
            }
        }
    }

    public void CompleteTaping()
    {
        boxCover.tape.SetActive(true);
        boxCover.IsPackaged = true;
        foreach (var id in idList)
        {
            GameObject crop = PhotonView.Find(id).gameObject;
            crop.SetActive(false);
        }

        Debug.Log($"테이핑 완료: {this.name}");
    }

    private void NotifyRequiredItemsChanged()
    {
        RequiredItemsChanged?.Invoke(requiredItems);
    }

    private void OnDestroy()
    {
        foreach (int crop in idList)
        {
            PhotonView cropView = PhotonView.Find(crop);

            Destroy(cropView.gameObject);
        }
    }
}