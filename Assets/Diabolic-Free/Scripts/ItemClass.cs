using UnityEngine;
using System.Collections;
[System.Serializable]
public class ItemClass : MonoBehaviour 
{
	public string itemName;
	public Texture itemTexture;
	public int width;
	public int height;
	public bool drawn = false;
    public string opis;
    public bool isEquiped = false;
}
