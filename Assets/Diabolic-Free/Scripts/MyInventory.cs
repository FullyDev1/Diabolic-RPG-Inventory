using UnityEngine;
using System.Collections;






public class MyInventory : MonoBehaviour 
{
    private RaycastHit hitInfo;
    private bool hashit = false;
    private Ray ray;


	public Texture item;
	public Texture defaultTexture;
	int gridWidth = 10;
	int gridHeight = 10;

	bool rmouseDown = false;
	bool lmousedown = false;


	bool bWeaponEquiped = false;//test
	bool bShieldEquiped = false;
	bool bR1Equiped = false;
	bool bR2Equiped = false;
	bool bAmuletEquiped = false;
	bool bBeltEquiped = false;
	bool bBootsEquiped = false;
	bool bGlovesEquiped = false;

    public GUIStyle lblItemDescrStyle;


    public struct Inventar
    {
        public string itemName;
        public Texture itemTexture;
        public int width;
        public int height;
        public bool drawn;
        public string opis;
        public bool occupiedCell;
        public int firstCell;
    }
   public Inventar[] charInventory;

    public int invL;
    public int invT;


    void Start()
    {
        charInventory = new Inventar[gridWidth * gridHeight];
        //invL = invT = 20;
    }

    void Update()
    {
        int litem = 1 << 8;


        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 50000.0f,litem))
        {
            ItemClass ic = hitInfo.transform.gameObject.GetComponent<ItemClass>();

            
            if (Input.GetMouseButtonDown(0))
            {
               if (ic.isEquiped != false) return;
               AddItemToInventory(hitInfo.transform.gameObject.GetComponent<ItemClass>());
               Destroy(hitInfo.transform.gameObject);
            }
        }
    }



    public int AddItemToInventory(ItemClass item)
    {
        int intialIndexLocation = -1; 
        if(item != null)
        {
            ItemClass temp =   item;      //Set up a temporary item to compare with

            if (SlotsRemaining() >= temp.height * temp.width)    
            {
                bool firstFoundLocation = true;                  
                                                                 
                int count = 0;                                   
                                                                   
                for(int i = 0; i < gridWidth-(temp.width-1); i ++) 
                {
                    for(int t = 0; t < (gridHeight)-(temp.height-1); t++)    
                    {
                        if (charInventory[i + (gridHeight * t)].occupiedCell != true)  
                        {
                            int neededCount = temp.height*temp.width;          
                            for(int j = 0; j < temp.width; j++)                
                            {
                                if (charInventory[i + (gridHeight * t) + j].occupiedCell != true)
                                                                                            
                                {
                                    for( int k = 0; k < temp.height; k++)   
                                    {
                                        if (charInventory[i + (gridHeight * (t + k)) + j].occupiedCell != true)
                                        {
                                            if(firstFoundLocation) 
                                            {
                                                firstFoundLocation = false; 
                                                                            
                                                intialIndexLocation = i+(gridWidth*t); 
                                            }
                                            count+=1;     
                                            if(count == neededCount)
                                            {
         
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if(count >= neededCount)
                            {
                                break;
                            } else if(count < neededCount)
                            {
                                firstFoundLocation = true;
                                count = 0;
                                intialIndexLocation = -1;
                            }
                        }
                    }  
                }
            }

            if (SlotsRemaining() == charInventory.Length)  
            {
                intialIndexLocation = 0; 
            }
            if(intialIndexLocation > -1) 
            {
                bool whentodraw=true;
                for(int i = 0; i < temp.width; i++)
                {
                    for(int t = 0; t < temp.height; t++)
                    {

                        charInventory[intialIndexLocation + i + (gridHeight * t)].drawn = true;
                        if (whentodraw) charInventory[intialIndexLocation + i + (gridHeight * t)].drawn = false;

                        charInventory[intialIndexLocation + i + (gridHeight * t)].itemName = item.itemName;
                        charInventory[intialIndexLocation + i + (gridHeight * t)].itemTexture = item.itemTexture;
                        charInventory[intialIndexLocation + i + (gridHeight * t)].width = item.width;
                        charInventory[intialIndexLocation + i + (gridHeight * t)].height = item.height;
                        charInventory[intialIndexLocation + i + (gridHeight * t)].opis = item.opis;
                        charInventory[intialIndexLocation + i + (gridHeight * t)].occupiedCell = true;
                        charInventory[intialIndexLocation + i + (gridHeight * t)].firstCell = intialIndexLocation;
                        whentodraw=false;
                    }
                }
            }
        }
        return intialIndexLocation;
    }


    void DeleteFromInventory(string itmNam, int fc)
    {
        for (int i = 0; i < charInventory.Length; ++i)
        {
            if (charInventory[i].itemName == itmNam && charInventory[i].firstCell == fc)
            {
                charInventory[i].occupiedCell = false;
            }

        }
    }

           
    string txtWidth = "";
    string txtHeight = "";
           
    void OnGUI()
    {


        //=======================================================character inventory=========================================
        for(int i = 0; i < gridWidth; i ++) 
        {  
            for(int t = 0; t < gridHeight; t++)
            {
                if (charInventory[i + (gridHeight * t)].occupiedCell != true)
                {
                    GUI.DrawTexture(new Rect((invL + (i * 30)), invT + (t * 30), 30, 30), defaultTexture);
                }
                else
                {
                    Inventar temp = charInventory[i + (gridHeight * t)];
                    if(!temp.drawn)
                    {
                        int w = temp.width;
                        int h = temp.height;

                        Rect buttonRect = new Rect((invL + (i * 30)), invT + (t * 30), 30 * w, 30 * h);
                        if (buttonRect.Contains(RevertMousePosValue()))
                        {
                            GUI.Label(new Rect(Screen.width / 2 - 100, 60, 200, 200), temp.opis.Replace("NEWLINE", "\n"), lblItemDescrStyle);
                        }


                        if (GUI.Button(buttonRect, temp.itemTexture))
                        {
                            if (Input.GetMouseButtonUp(0))
                            {

                                if (!bWeaponEquiped)//equip weapon
                                {
                                    GameObject HandGr = GameObject.Find("HandGrip");
                                    Vector3 gp = HandGr.transform.position;
                                    Instantiate(Resources.Load(temp.itemName), gp, Quaternion.identity);//no 2 prefabs same name
                                    GameObject cloned = GameObject.Find(temp.itemName + "(Clone)");
                                    cloned.transform.parent = HandGr.transform;
                                    cloned.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                    DeleteFromInventory(temp.itemName, temp.firstCell);
                                    bWeaponEquiped = true;
                                    ItemClass ic = cloned.GetComponent<ItemClass>();
                                    ic.isEquiped = true;
                                }

                            }
                            else if (Input.GetMouseButtonUp(1))
                            {
                                Vector3 rp = new Vector3(Random.Range(-1.0F, 1.0F), 0, Random.Range(-1.0F, 1.0F));
                                GameObject plyr = GameObject.Find("Player");
                                Vector3 PlayerPos = plyr.transform.position;
                                Instantiate(Resources.Load(temp.itemName), PlayerPos + rp, Quaternion.identity);//no 2 prefabs same name
                                DeleteFromInventory(temp.itemName, temp.firstCell);

                            }
                        }

                        temp.drawn = true;
                    }
                }
            }
        }
    }
    public int SlotsRemaining()
    {
        int count = 0;
        for (int i = 0; i < charInventory.Length; i++)
        {
            if (charInventory[i].occupiedCell != true)
            {
                count++;
            }
        }
        return count;
    }
    Vector2 RevertMousePosValue()
    {
        Vector2 mousePos =(Input.mousePosition);
        Vector2 newMousePos;
        newMousePos.x = mousePos.x;
        newMousePos.y = -mousePos.y + Screen.height;
        return newMousePos;
    }

}

     
