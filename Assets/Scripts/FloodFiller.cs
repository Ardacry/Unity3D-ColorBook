﻿//------------------------------------------------ // This script performs flood fill operation based // on input taken by user. It fills the portion of // given texture with desired color until the area  // to be filled exhausted. // /**/ --> commented features are in test. Toggle // on while using. //------------------------------------------------- using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEditor; using UnityEngine.EventSystems;  public class FloodFiller: MonoBehaviour {     private Vector2 clickPosition; // (x,y) location of user click     public Texture2D texture;     private Color32 targetColor;     public Color32 color; // selected color for filling the empty spaces     public static Stack<HistoryItem> histStack = new Stack<HistoryItem>(); // It holds action history     bool undo = false;     bool redo = false;     public static Stack<HistoryItem> redoStack = new Stack<HistoryItem>(); // It holds undo actions for re-doing      void Start()     {         texture = (Texture2D)gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture;          RGBBalancer(); // see definition below         texture.Apply();          color = Color.white; // initilazing white as a starting color     }      // Update is called once per frame     void Update()     {         if (EventSystem.current.IsPointerOverGameObject() | EventSystem.current.currentSelectedGameObject != null)         {             return;         }
        // Input collector function that detects a touch on the screen and work if it exists.
        // It simply takes the location of touch and adjust it with the texture and pass it to
        // FloodFill function to operate.
        if (Input.touchCount == 1 & Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                clickPosition.x = (hit.point.x - hit.collider.bounds.min.x) / hit.collider.bounds.size.x;
                clickPosition.y = (hit.point.y - hit.collider.bounds.min.y) / hit.collider.bounds.size.y;
                clickPosition.x = (int)(clickPosition.x * texture.width);
                clickPosition.y = (int)(clickPosition.y * texture.height);

                redoStack.Clear(); // clearing redo stack if any action other than undo performed                  FloodFill(clickPosition, color);
                texture.Apply();
            }         }     }      // This method takes (x,y) coordinate of the pixel clicked by user and the color to be filled.     // It fetches the target color of the clicked pixel and replace it with clr by traversing every     // neighboring pixels of the starting pixel by iterating every pixel to the north, south, east and     // west locations.     public void FloodFill(Vector2 position, Color32 clr)     {         // if undo or redo function called, target color is selected by called function. Otherwise, targetColor is         // clicked pixel and furthermore the action is recorded into histStack         if (undo | redo)         {             undo = false;             redo = false;         }         else         {
            targetColor = texture.GetPixel((int)position.x, (int)position.y);
             // if target color is already given color, return back.             if (ColorCompare(clr, targetColor))                 return;                          //if target color is black, return back             if (targetColor == Color.black)                 return;              HistoryItem item = new HistoryItem();             item.position = position;             item.histColor = clr;             item.histTargetColor = targetColor;             histStack.Push(item);           }          Queue<Vector2> queue = new Queue<Vector2>();         queue.Enqueue(position);          // This loop iterates every pixel by enqueuing all to a queue and set their color         // to clr if their current color is target color and not clr. In each iteration          // the upmost element of the queue is popped and processed. Loop terminates when         // queue becomes empty.         while (queue.Count > 0)         {             Vector2 current = queue.Dequeue();                         if (!ColorCompare(texture.GetPixel((int)(current.x - 1), (int)current.y), clr))             {                 if (ColorCompare(texture.GetPixel((int)(current.x - 1), (int)current.y), targetColor))                 {                     Vector2 temporary = new Vector2((current.x - 1), current.y);                     queue.Enqueue(temporary);                     texture.SetPixel((int)(current.x - 1), (int)current.y, clr);                 }                 /*                 if((texture.GetPixel((int)(current.x - 1), (int)current.y) == Color.black))                 {                     texture.SetPixel((int)(current.x - 1), (int)current.y, clr);                  }*/             }                         if (!ColorCompare(texture.GetPixel((int)(current.x + 1), (int)current.y), clr))             {                 if (ColorCompare(texture.GetPixel((int)(current.x + 1), (int)current.y), targetColor))                 {                     texture.SetPixel((int)(current.x + 1), (int)current.y, clr);                     Vector2 temporary = new Vector2((current.x + 1), current.y);                     queue.Enqueue(temporary);                 }                 /*                 if ((texture.GetPixel((int)(current.x + 1), (int)current.y) == Color.black))                 {                     texture.SetPixel((int)(current.x + 1), (int)current.y, clr);                                      }*/             }             if (!ColorCompare(texture.GetPixel((int)(current.x), (int)(current.y - 1)), clr))             {                 if (ColorCompare(texture.GetPixel((int)(current.x), (int)(current.y - 1)), targetColor))                 {                     texture.SetPixel((int)(current.x), (int)current.y - 1, clr);                     Vector2 temporary = new Vector2((current.x), current.y - 1);                     queue.Enqueue(temporary);                 }                 /*                 if ((texture.GetPixel((int)(current.x), (int)(current.y - 1)) == Color.black))                 {                     texture.SetPixel((int)(current.x), (int)current.y - 1, clr);                 }*/             }             if (!ColorCompare(texture.GetPixel((int)(current.x), ((int)(current.y + 1))), clr))             {                 if (ColorCompare(texture.GetPixel((int)(current.x), (int)(current.y + 1)), targetColor))                 {                     texture.SetPixel((int)(current.x), (int)current.y + 1, clr);                     Vector2 temporary = new Vector2((current.x), current.y + 1);                     queue.Enqueue(temporary);                 }                 /*                 if ((texture.GetPixel((int)(current.x), (int)(current.y + 1)) == Color.black))                 {                     texture.SetPixel((int)(current.x), (int)current.y + 1, clr);                 }*/             }         }      }     // RGBBalancer scans the whole image and adjust whiteish colors rgb value to Color.white and     // blackish colors rgb value to Color.black. Resulting texture contains only Color.white and     // Color.black     void RGBBalancer()     {         for (int i = 0; i < texture.height; i++)         {             for (int j = 0; j < texture.width; j++)             {                 if(texture.GetPixel(j, i).r > 0.5)                 {                     texture.SetPixel(j, i, Color.white);                 }                 else                 {                     texture.SetPixel(j, i, Color.black);                 }             }         }     }      // This method redo all previous changes on the texture by scanning each pixel      // and assinging them Color.white     void TextureCleaner()     {         for (int i = 0; i < texture.height; i++)         {             for (int j = 0; j < texture.width; j++)             {                 if (texture.GetPixel(j, i) != Color.black)                 {                     texture.SetPixel(j, i, Color.white);                 }             }         }     }      // When game ends, TextureCleaner method called and quited afterwards.     private void OnApplicationQuit()     {         TextureCleaner();         texture.Apply();     }      // This method erase all previous changes if corresponding button is clicked.     public void EraseAll()     {         redoStack.Clear(); // if this method performed redo stack cleared         TextureCleaner();         texture.Apply();     }      // Below methods are called if their associated buttons are clicked. When     // they are called, they change selected color parameter accordingly.     // White color is used as an eraser for previous changes.     public void EraseArea()     {         color = Color.white;     }      // This method undo the previous change one by one by popping the most recent item     // from histStack that records history. By the time, it pushes the popped item to      // redoStack for Redo function to perform redo event if undo was performed before     public void Undo()     {         if (histStack.Count != 0)         {             HistoryItem temp = histStack.Pop();              redoStack.Push(temp);                 Color32 tempColor = temp.histTargetColor;             Vector2 tempPosition = temp.position;             targetColor = temp.histColor;             undo = true;              FloodFill(tempPosition, tempColor);             texture.Apply();         }     }     // This method re-does the undoed event before one by one by popping the most recent      // item from redoStack. By the time, it pushes the popped item to histStack for Undo     // function to perform undo event.     public void Redo()     {         if(redoStack.Count != 0)         {             HistoryItem temp = redoStack.Pop();              histStack.Push(temp);              Color32 tempColor = temp.histColor;             Vector2 tempPosition = temp.position;             targetColor = temp.histTargetColor;             redo = true;              FloodFill(tempPosition, tempColor);             texture.Apply();         }     }      // This method enables the FloodFiller script(this script) for flood fill, erase and undo operations     public void EnableFloodFill()     {         gameObject.GetComponent<FloodFiller>().enabled = true;     }

    // Below methods are for selecting the various configurations of pre-selected color. For example,     // if user clicks yellow button before various yellow configurations can be selected by SelectYellow     // functions. RGBA values are used to assign Color32 variables.
    public void SelectBlue1()     {         Color32 tempColor = new Color32(0, 0, 50, 1);         color = tempColor;     }     public void SelectBlue2()     {         Color32 tempColor = new Color32(0, 0, 100, 1);         color = tempColor;     }     public void SelectBlue3()     {         Color32 tempColor = new Color32(0, 0, 150, 1);         color = tempColor;     }     public void SelectBlue4()     {         Color32 tempColor = new Color32(0, 0, 200, 1);         color = tempColor;     }     public void SelectBlue5()     {         Color32 tempColor = new Color32(0, 0, 250, 1);         color = tempColor;     }     public void SelectYellow1()     {         Color32 tempColor = new Color32(255, 255, 204, 1);         color = tempColor;     }     public void SelectYellow2()     {         Color32 tempColor = new Color32(255, 255, 153, 1);         color = tempColor;     }     public void SelectYellow3()     {         Color32 tempColor = new Color32(255, 255, 26, 1);         color = tempColor;     }     public void SelectYellow4()     {         Color32 tempColor = new Color32(204, 204, 0, 1);         color = tempColor;     }     public void SelectYellow5()     {         Color32 tempColor = new Color32(128, 128, 0, 1);         color = tempColor;     }     public void SelectGreen1()     {         Color32 tempColor = new Color32(153, 255, 187, 1);         color = tempColor;     }     public void SelectGreen2()     {         Color32 tempColor = new Color32(26, 255, 102, 1);         color = tempColor;     }     public void SelectGreen3()     {         Color32 tempColor = new Color32(0, 204, 68, 1);         color = tempColor;     }     public void SelectGreen4()     {         Color32 tempColor = new Color32(0, 128, 43, 1);         color = tempColor;     }     public void SelectGreen5()     {         Color32 tempColor = new Color32(0, 51, 17, 1);         color = tempColor;     }
    public void SelectPink1()     {         Color32 tempColor = new Color32(255, 192, 203, 1);         color = tempColor;     }     public void SelectPink2()     {         Color32 tempColor = new Color32(255, 105, 180, 1);         color = tempColor;     }     public void SelectPink3()     {         Color32 tempColor = new Color32(255, 20, 147, 1);         color = tempColor;     }     public void SelectPink4()     {         Color32 tempColor = new Color32(219, 112, 147, 1);         color = tempColor;     }     public void SelectPink5()     {         Color32 tempColor = new Color32(199, 21, 133, 1);         color = tempColor;     }     public void SelectRed1()     {         Color32 tempColor = new Color32(250, 128, 114, 1);         color = tempColor;     }     public void SelectRed2()     {         Color32 tempColor = new Color32(205, 92, 92, 1);         color = tempColor;     }     public void SelectRed3()     {         Color32 tempColor = new Color32(220, 20, 60, 1);         color = tempColor;     }     public void SelectRed4()     {         Color32 tempColor = new Color32(255, 0, 0, 1);         color = tempColor;     }     public void SelectRed5()     {         Color32 tempColor = new Color32(139, 0, 0, 1);         color = tempColor;     }
    public void SelectOrange1()     {         Color32 tempColor = new Color32(255, 165, 0, 1);         color = tempColor;     }     public void SelectOrange2()     {         Color32 tempColor = new Color32(255, 140, 0, 1);         color = tempColor;     }     public void SelectOrange3()     {         Color32 tempColor = new Color32(255, 127, 80, 1);         color = tempColor;     }     public void SelectOrange4()     {         Color32 tempColor = new Color32(255, 99, 71, 1);         color = tempColor;     }     public void SelectOrange5()     {         Color32 tempColor = new Color32(255, 69, 0, 1);         color = tempColor;     } 
    // This method compares the two colors by comparing their red, green and blue values one by one and returns a bool result.
    public bool ColorCompare (Color32 color1, Color32 color2)     {         if (Mathf.Approximately(color1.r, color2.r) & Mathf.Approximately(color1.g, color2.g) & Mathf.Approximately(color1.b, color2.b))         {             return true;         }         else             return false;      } }  // History struct holds the clickPosition, targetColor and filled color informations for previous // changes. By using it program records all previous events. public struct HistoryItem {     public Color32 histColor { get; set; }     public Color32 histTargetColor { get; set; }     public Vector2 position { get; set; } }     