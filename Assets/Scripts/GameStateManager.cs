using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static int PlayerState;
    public static int NumSheepCollected;
    public static int GameState;
    void Start() {
        NumSheepCollected = 0;
        GameState = 2; //top menu
        PlayerState = 1; //idle
        print("GameManagerStarting...");
    }

    // Update is called once per frame
    void Update()
    {
        switch (PlayerState) {
            case 5:
                //Moving Blocked
                break;
            case 4:
                //Move Pushing
                break;
            case 3:
                //Moving
                break;
            case 2:
                //Query Move
                break;
            case 1:
                //Idle
                break;
            default:
                //something is very broken
                break;
        }

        switch (GameState) {
            case 5:
                //endingcutscene
                break;
            case 4:
                //All 100 returned
                break;
            case 3:
                //Intro Cutscene
                break;
            case 2:
                //Game Top Menu
                break;
            case 1:
              //playing sokoban
              break;
          default:
              //something is very broken
              break;
        }
        
        
        
    }
}
