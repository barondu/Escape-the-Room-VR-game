using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFloatingItem : MonoBehaviour {

	private HFPS_GameManager gameManager;

	[Header("Raycasting")]
	public LayerMask Layer;

	public List<GameObject> FloatingIcons = new List<GameObject>();

	private List<GameObject> ItemsInDistance = new List<GameObject>();
	private List<GameObject> ItemsFloatingCache = new List<GameObject>();

	[Header("UI")]
	[Tooltip("Must be in \"Resources\" Folder!!")]
	public GameObject FloatingIconPrefab;
	public Transform FloatingIconUI;

	private KeyCode UseKey;

	private Image FloatingIcon;
	private Sprite DefaultFloatingIcon;
	private Vector2 DefaultFloatingSize;

	[Header("Distance")]
	public float distanceShow = 3;
	public float distanceDraw = 4;

	private float distance;

    private bool isCleared;
    private bool isVisible;

	void Start()
	{
        gameManager = GetComponent<HFPS_GameManager> ();
        isVisible = true;
        isCleared = true;
    }

	void Update () {
        if (FloatingIcons.Contains(null))
        {
            RemoveNullElements();
        }

		if (isCleared && FloatingIcons.Count > 0) {
            if (ItemsInDistance.Contains(null)) { ItemsInDistance.RemoveAll(GameObject => GameObject == null); }
            if (ItemsFloatingCache.Contains(null)) { ItemsFloatingCache.RemoveAll(GameObject => GameObject == null); }

            for (int i = 0; i < FloatingIcons.Count; i++)
            {
                float currentDistance = Vector3.Distance(gameManager.Player.transform.position, FloatingIcons[i].transform.position);
                if (currentDistance <= distanceDraw && !ItemsInDistance.Contains(FloatingIcons[i]))
                {
                    ItemsInDistance.Add(FloatingIcons[i]);
                }
                if (currentDistance >= distanceDraw && ItemsInDistance.Contains(FloatingIcons[i]))
                {
                    ItemsInDistance.Remove(FloatingIcons[i]);
                    ItemsFloatingCache.Remove(GetAndDestroyFloatingIcon(FloatingIcons[i]));
                }
            }

            if(ItemsInDistance != null) {
                for (int i = 0; i < ItemsInDistance.Count; i++)
                {
                    if (GetItemVisible(ItemsInDistance[i]) && isVisible)
                    {
                        if (!ContainsFloatingIcon(ItemsInDistance[i]))
                        {
                            InstantiateFloatingIcon(ItemsInDistance[i]);
                        }
                        if (ContainsFloatingIcon(ItemsInDistance[i]))
                        {
                            GetFloatingIcon(ItemsInDistance[i]).GetComponent<FloatingItemInfo>().SetVisible(true);
                        }
                    }
                    else
                    {
                        if (ContainsFloatingIcon(ItemsInDistance[i]))
                        {
                            GetFloatingIcon(ItemsInDistance[i]).GetComponent<FloatingItemInfo>().SetVisible(false);
                        }
                    }
                }
            }
        }
	}

    public void SetItemsVisibe(bool state)
    {
        isVisible = state;
    }

	void InstantiateFloatingIcon(GameObject FollowObject)
	{
		GameObject icon = Instantiate (FloatingIconPrefab);
		ItemsFloatingCache.Add (icon);
		icon.transform.SetParent (FloatingIconUI);
		icon.GetComponent<FloatingItemInfo> ().ObjectToFollow = FollowObject;
	}

	bool GetItemVisible(GameObject Object)
	{
		GameObject camera = Camera.main.gameObject;
		RaycastHit hit;
		if (Physics.Linecast (camera.transform.position, Object.transform.position, out hit, Layer)) {
			if(!(hit.collider.gameObject == Object))
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		return false;
	}

    public void SetItemVisible(GameObject Obj, bool Visible)
    {
        if (!ContainsFloatingIcon(Obj)) return;

        switch(Visible)
        {
            case true:
                isVisible = true;
                GetFloatingIcon(Obj).GetComponent<FloatingItemInfo>().SetVisible(true);
                break;
            case false:
                isVisible = false;
                GetFloatingIcon(Obj).GetComponent<FloatingItemInfo>().SetVisible(false);
                break;
        }
    }

    void RemoveNullElements()
    {
        isCleared = false;
        if (FloatingIcons.Contains(null))
        {
            FloatingIcons.RemoveAll(GameObject => GameObject == null);
            ItemsInDistance.Clear();
            ItemsFloatingCache.Clear();
        }
        isCleared = true;
    }

    public void RemoveFloatingObject(GameObject Obj)
    {
        isCleared = false;
		if (FloatingIcons.Contains (Obj)) {
			FloatingIcons.Remove (Obj);
			ItemsInDistance.Clear ();
			ItemsFloatingCache.Clear ();
		}
		Destroy (Obj);
        isCleared = true;
    }

	GameObject GetFloatingIcon(GameObject FollowObject)
	{
        foreach (var item in ItemsFloatingCache)
        {
            if (item.GetComponent<FloatingItemInfo>().ObjectToFollow == FollowObject)
            {
                return item;
            }
        }

		return null;
	}

	GameObject GetAndDestroyFloatingIcon(GameObject FollowObject)
	{
        foreach (var item in ItemsFloatingCache)
        {
            if (item.GetComponent<FloatingItemInfo>().ObjectToFollow == FollowObject)
            {
                Destroy(item);
                return item;
            }
        }

		return null;
	}

	bool ContainsFloatingIcon(GameObject FollowObject)
	{
        if (ItemsFloatingCache.Count > 0)
        {
            foreach (var item in ItemsFloatingCache)
            {
                if (item.GetComponent<FloatingItemInfo>().ObjectToFollow == FollowObject)
                {
                    return true;
                }
            }
        }

		return false;
	}
}
