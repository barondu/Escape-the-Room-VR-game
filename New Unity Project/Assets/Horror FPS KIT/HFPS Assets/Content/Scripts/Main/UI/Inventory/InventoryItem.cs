using UnityEngine;

public class InventoryItem : MonoBehaviour {

	public enum Type { NoInventoryItem, SwitcherItem, InventoryItem, WeaponItem, BackpackExpand }
	public enum WeaponType { Weapon, Ammo }
    public enum MessageType { None, Hint, Message, ItemName }
    public enum DisableType { Disable, Destroy }

	public Type ItemType = Type.InventoryItem;
	public WeaponType weaponType = WeaponType.Weapon;
    public MessageType messageType = MessageType.None;
    public DisableType disableType = DisableType.Disable;
    public string message;
    public AudioClip PickupSound;
    [Range(0, 1)]public float PickupVolume;
	public int Amount = 1;

    public bool markLightObject;

	public int WeaponID;
	public int InventoryID;
	public int BackpackExpand;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = Camera.main.transform.parent.parent.GetComponent<ScriptManager>().SoundEffects;
    }

    public void UseObject()
	{
        if (PickupSound)
        {
            audioSource.clip = PickupSound;
            audioSource.volume = PickupVolume;
            audioSource.Play();
        }

        if(disableType == DisableType.Destroy)
        {
            Destroy(gameObject);
        }
        else
        {
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            if(transform.childCount > 0)
            {
                foreach (Transform child in transform.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
	}

    public void DisableObject(bool Disable)
    {
        if (Disable == false)
        {
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            if (transform.childCount > 0)
            {
                foreach (Transform child in transform.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}
