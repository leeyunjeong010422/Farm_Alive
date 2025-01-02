using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TruckQuest : MonoBehaviour
{
    [SerializeField] int truckId;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("충돌");
        if (other.CompareTag("Box"))
        {
            XRGrabInteractable interactable = other.GetComponent<XRGrabInteractable>();
            if (interactable.isSelected)
                return;

            Debug.Log("박스");
            BoxTrigger boxTrigger = other.GetComponent<BoxTrigger>();
            if (boxTrigger == null)
                return;

            /*foreach (QuestManager.RequiredItem item in boxTrigger.requiredItems)
            {
                for (int i = 0; i < QuestManager.Instance.questsList[truckId].requiredItems.Count; i++)
                {
                    if(item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[i].itemPrefab.name)
                    {
                        Debug.Log("이름이 같음");
                        Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount} <= {item.requiredcount}");
                        if (QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount <= 0)
                            break;
                        Debug.Log("갯수 통과");

                        QuestManager.Instance.CountUpdate(truckId, i, item.requiredcount);
                        break;
                        *//*if(item.requiredcount == QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount)
                        {
                            Debug.Log("카운트가 같음");
                            QuestManager.Instance.SuccessQuest(truckId, i);
                            break;
                        }*//*
                    }
                }
            }*/

            for (int i = 0; i < boxTrigger.requiredItems.Count; i++)
            {
                QuestManager.RequiredItem item = boxTrigger.requiredItems[i];
                for (int j = 0; j < QuestManager.Instance.questsList[truckId].requiredItems.Count; j++)
                {
                    if (item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name)
                    {
                        Debug.Log("이름이 같음");
                        Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount} <= {item.requiredcount}");
                        if (QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount <= 0)
                            break;
                        Debug.Log("갯수 통과");

                        QuestManager.Instance.CountUpdate(truckId, i, item.requiredcount);
                        break;
                    }
                }
            }

            Destroy(other.gameObject);
        }
    }
}
